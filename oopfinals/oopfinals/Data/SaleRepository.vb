Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Data/SaleRepository.vb — THE atomic checkout (spec §7.5, the money
' path). Every POS sale lands here as ONE SQLite transaction:
'
'     BEGIN
'       ① INSERT INTO Sales … → last_insert_rowid() = saleId
'       ② per cart line:
'            INSERT INTO SaleItems
'            UPDATE Products SET StockQty = StockQty - @qty
'                   WHERE ProductId = @pid AND StockQty >= @qty
'                   → 0 rows affected ⇒ THROW (oversell guard against
'                     cart/DB stock races — the cart pre-check cannot
'                     win every race, SQL answers the final question)
'            INSERT INTO StockMovements ('Sale', −qty,
'                   reason "POS Sale: RCP-…")
'     COMMIT      any failure ⇒ ROLLBACK + RETHROW THE ORIGINAL ERROR
'
' The whole sale commits or NOTHING does: a half-written receipt can
' never exist. The guarded stock update is the single source of truth
' for "is there still stock" — decrements are unconditional math on
' the row the transaction locks, not on a stale UI snapshot.
'
' OOP concept: ENCAPSULATION
'   NotInheritable + Private Sub New — static surface only. Entry
'   validation rejects empty carts, non-positive totals and subtotals
'   that drift from Σ LineTotal, so a mathematically broken receipt is
'   structurally impossible to persist; the read side re-hydrates
'   validated Sale/SaleItem models through their real setters.
'
' LAYERING: Data → Models only (Sale / SaleItem). No Services, no UI.
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Data.SQLite
Imports oopfinals.Models

Namespace Data

    Public NotInheritable Class SaleRepository

        ' Static gate: repositories are stateless, never instantiated.
        Private Sub New()
        End Sub

#Region "Receipt numbering (spec §7.5)"

        ' RCP-{yyyyMMdd}-{count of today's receipts + 1, padded D4}.
        ' Counting keys on the ReceiptNo PREFIX (not SaleDate) so the
        ' number stream stays strictly sequential within a day even if
        ' a row were ever back-dated. The UNIQUE(ReceiptNo) constraint
        ' is the last-resort collision referee: an exact tie (two
        ' terminals racing the same SELECT) fails the INSERT, rolls the
        ' losing transaction back and surfaces an error — one receipt,
        ' never two.
        Public Shared Function GenerateReceiptNumber() As String
            Dim todayPart As String = DateTime.Today.ToString("yyyyMMdd")
            Dim prefix As String = String.Format("RCP-{0}-", todayPart)

            Dim todayCount As Long = 0
            Using connection As SQLiteConnection = DatabaseHelper.GetConnection()
                Using command As New SQLiteCommand(
                        "SELECT COUNT(*) FROM Sales WHERE ReceiptNo LIKE @prefixPattern;",
                        connection)
                    command.Parameters.AddWithValue("@prefixPattern", prefix & "%")
                    todayCount = Convert.ToInt64(command.ExecuteScalar())
                End Using
            End Using

            Return String.Format("{0}{1:D4}", prefix, todayCount + 1L)
        End Function

#End Region

#Region "RecordSale — the transactional core"

        ' Writes one receipt atomically and returns the new Sales.SaleId.
        '
        ' ENTRY GUARDS (all BEFORE any SQL runs):
        '   • sale ≠ Nothing,
        '   • at least one line item (both the frozen cart snapshot AND
        '     the in-memory receipt),
        '   • Total > 0,
        '   • Subtotal must equal the rounded Σ LineTotal — defense in
        '     depth against cart/service math drift, re-checked at the
        '     persistence boundary.
        Public Shared Function RecordSale(sale As Sale,
                                          items As IReadOnlyList(Of SaleItem)) As Integer
            If sale Is Nothing Then
                Throw New ArgumentNullException("sale", "A sale record is required to record a transaction.")
            End If
            If items Is Nothing OrElse items.Count = 0 Then
                Throw New ArgumentException(
                    "The cart is empty. At least one line item is required to record a sale.")
            End If
            If sale.Items Is Nothing OrElse sale.Items.Count = 0 Then
                Throw New ArgumentException(
                    "The receipt has no line items. At least one line item is required to record a sale.")
            End If
            If sale.Total <= 0D Then
                Throw New ArgumentException("The sale total must be greater than zero.")
            End If

            Dim computedSubtotal As Decimal = SumLineTotals(items)
            If sale.Subtotal <> computedSubtotal OrElse sale.Subtotal <> sale.GetLineTotalSum() Then
                Throw New InvalidOperationException(
                    "The cart subtotal does not match the computed line totals. Sale cancelled for safety.")
            End If

            ' ISO-8601 stamp for the TEXT date column (spec §8). A sale
            ' built without an explicit SaleDate stamps itself here.
            Dim stamp As DateTime = sale.SaleDate
            If stamp < New DateTime(2000, 1, 1) Then
                stamp = DateTime.Now
            End If
            Dim saleDateIso As String = stamp.ToString(DatabaseHelper.IsoDateTimeFormat)
            Dim movementReason As String = "POS Sale: " & sale.ReceiptNo

            Using connection As SQLiteConnection = DatabaseHelper.GetConnection()
                Using transaction As SQLiteTransaction = connection.BeginTransaction()
                    Try
                        ' ① Sales header row.
                        Using insertSale As SQLiteCommand = CreateTransactionCommand(
                                connection, transaction,
                                "INSERT INTO Sales (ReceiptNo, SaleDate, CashierId, CustomerId, Subtotal, " &
                                "DiscountType, DiscountAmount, Total, CashReceived, ChangeAmount) " &
                                "VALUES (@receiptNo, @saleDate, @cashierId, @customerId, @subtotal, " &
                                "@discountType, @discountAmount, @total, @cashReceived, @changeAmount);")
                            insertSale.Parameters.AddWithValue("@receiptNo", sale.ReceiptNo)
                            insertSale.Parameters.AddWithValue("@saleDate", saleDateIso)
                            insertSale.Parameters.AddWithValue("@cashierId", sale.CashierId)
                            If sale.CustomerId.HasValue Then
                                insertSale.Parameters.AddWithValue("@customerId", sale.CustomerId.Value)
                            Else
                                insertSale.Parameters.AddWithValue("@customerId", DBNull.Value)
                            End If
                            insertSale.Parameters.AddWithValue("@subtotal", Math.Round(sale.Subtotal, 2, MidpointRounding.AwayFromZero))
                            insertSale.Parameters.AddWithValue("@discountType", sale.DiscountType)
                            insertSale.Parameters.AddWithValue("@discountAmount", Math.Round(sale.DiscountAmount, 2, MidpointRounding.AwayFromZero))
                            insertSale.Parameters.AddWithValue("@total", Math.Round(sale.Total, 2, MidpointRounding.AwayFromZero))
                            insertSale.Parameters.AddWithValue("@cashReceived", Math.Round(sale.CashReceived, 2, MidpointRounding.AwayFromZero))
                            insertSale.Parameters.AddWithValue("@changeAmount", Math.Round(sale.ChangeAmount, 2, MidpointRounding.AwayFromZero))
                            insertSale.ExecuteNonQuery()
                        End Using

                        Dim saleId As Integer = ReadLastRowId(connection, transaction)
                        sale.SaleId = saleId ' ≥ 0 — the model setter accepts it

                        ' ② The line-item pass: receipt rows, guarded stock
                        '    decrements and audit movements, in lock-step.
                        For Each line As SaleItem In items
                            Using insertItem As SQLiteCommand = CreateTransactionCommand(
                                    connection, transaction,
                                    "INSERT INTO SaleItems (SaleId, ProductId, Quantity, UnitPrice, LineTotal) " &
                                    "VALUES (@saleId, @productId, @quantity, @unitPrice, @lineTotal);")
                                insertItem.Parameters.AddWithValue("@saleId", saleId)
                                insertItem.Parameters.AddWithValue("@productId", line.ProductId)
                                insertItem.Parameters.AddWithValue("@quantity", line.Quantity)
                                insertItem.Parameters.AddWithValue("@unitPrice", Math.Round(line.UnitPrice, 2, MidpointRounding.AwayFromZero))
                                insertItem.Parameters.AddWithValue("@lineTotal", Math.Round(line.LineTotal, 2, MidpointRounding.AwayFromZero))
                                insertItem.ExecuteNonQuery()
                            End Using

                            ' THE race guard: the decrement only lands if the
                            ' row still holds the requested stock. Zero rows
                            ' affected = someone else already sold it — the
                            ' whole receipt dies below.
                            Using updateStock As SQLiteCommand = CreateTransactionCommand(
                                    connection, transaction,
                                    "UPDATE Products SET StockQty = StockQty - @qty " &
                                    "WHERE ProductId = @productId AND StockQty >= @qty;")
                                updateStock.Parameters.AddWithValue("@qty", line.Quantity)
                                updateStock.Parameters.AddWithValue("@productId", line.ProductId)
                                Dim rowsAffected As Integer = updateStock.ExecuteNonQuery()

                                If rowsAffected = 0 Then
                                    Throw New InvalidOperationException(String.Format(
                                        "Insufficient stock for {0} ({1}). {2} unit(s) available. Transaction cancelled.",
                                        line.ProductName, line.ProductCode, CurrentStock(connection, transaction, line.ProductId)))
                                End If
                            End Using

                            ' ③ Immutable audit row — a NEGATIVE quantity change
                            '    with the receipt in the reason (spec §6.5).
                            Using insertMovement As SQLiteCommand = CreateTransactionCommand(
                                    connection, transaction,
                                    "INSERT INTO StockMovements (ProductId, MovementType, QuantityChange, Reason, SupplierId, UserId, MovementDate) " &
                                    "VALUES (@productId, @movementType, @quantityChange, @reason, @supplierId, @userId, @movementDate);")
                                insertMovement.Parameters.AddWithValue("@productId", line.ProductId)
                                insertMovement.Parameters.AddWithValue("@movementType", StockMovement.MovementTypeSale)
                                insertMovement.Parameters.AddWithValue("@quantityChange", -line.Quantity)
                                insertMovement.Parameters.AddWithValue("@reason", movementReason)
                                insertMovement.Parameters.AddWithValue("@supplierId", DBNull.Value)
                                insertMovement.Parameters.AddWithValue("@userId", sale.CashierId)
                                insertMovement.Parameters.AddWithValue("@movementDate", saleDateIso)
                                insertMovement.ExecuteNonQuery()
                            End Using
                        Next

                        transaction.Commit()
                        Return saleId
                    Catch saleError As Exception
                        ' Best-effort rollback guarded in its own Try/Catch:
                        ' if the rollback itself faults, THAT error must not
                        ' become the rethrown one. The ORIGINAL error is
                        ' rethrown either way (never swallowed).
                        Try
                            transaction.Rollback()
                        Catch
                            ' Rollback failure observed and deliberately set aside.
                        End Try
                        Throw saleError
                    End Try
                End Using
            End Using
        End Function

        ' Rounded Σ LineTotal for the guard — the same AwayFromZero
        ' rounding every other money value uses (spec §13).
        Private Shared Function SumLineTotals(items As IReadOnlyList(Of SaleItem)) As Decimal
            Dim sum As Decimal = 0D
            For Each line As SaleItem In items
                sum += line.LineTotal
            Next
            Return Math.Round(sum, 2, MidpointRounding.AwayFromZero)
        End Function

        ' Stock remaining ON THE ROW this transaction just failed to
        ' decrement — the over-stock message quotes the live figure.
        Private Shared Function CurrentStock(connection As SQLiteConnection,
                                             transaction As SQLiteTransaction,
                                             productId As Integer) As Integer
            Using probe As SQLiteCommand = CreateTransactionCommand(
                    connection, transaction,
                    "SELECT StockQty FROM Products WHERE ProductId = @productId;")
                probe.Parameters.AddWithValue("@productId", productId)
                Dim found As Object = probe.ExecuteScalar()
                If found Is Nothing OrElse TypeOf found Is DBNull Then
                    Return 0
                End If
                Return Convert.ToInt32(found)
            End Using
        End Function

        ' Transaction-scoped command factory — every statement inside the
        ' transaction is created here and stays fully parameterized.
        Private Shared Function CreateTransactionCommand(connection As SQLiteConnection,
                                                         transaction As SQLiteTransaction,
                                                         sql As String) As SQLiteCommand
            Dim command As SQLiteCommand = connection.CreateCommand()
            command.Transaction = transaction
            command.CommandText = sql
            Return command
        End Function

        Private Shared Function ReadLastRowId(connection As SQLiteConnection,
                                              transaction As SQLiteTransaction) As Integer
            Using idCommand As SQLiteCommand = CreateTransactionCommand(
                    connection, transaction, "SELECT last_insert_rowid();")
                Return Convert.ToInt32(idCommand.ExecuteScalar())
            End Using
        End Function

#End Region

#Region "Read side — receipt hydration"

        ' One SELECT shape for every single-receipt read: LEFT JOINs pull
        ' the display names; COALESCE supplies the spec defaults
        ' ("Cashier" for an unknown cashier, "Walk-In" for no customer).
        Private Const BaseSaleSelectSql As String =
            "SELECT s.SaleId, s.ReceiptNo, s.SaleDate, s.CashierId, s.CustomerId, " &
            "s.Subtotal, s.DiscountType, s.DiscountAmount, s.Total, s.CashReceived, s.ChangeAmount, " &
            "COALESCE(cu.FullName, 'Cashier') AS CashierName, " &
            "pu.FullName AS CustomerName " &
            "FROM Sales s " &
            "LEFT JOIN Users cu ON cu.UserId = s.CashierId " &
            "LEFT JOIN Users pu ON pu.UserId = s.CustomerId "

        Private Shared Function MapRowToSale(reader As SQLiteDataReader) As Sale
            Dim sale As New Sale()
            sale.SaleId = Convert.ToInt32(reader("SaleId"))
            sale.ReceiptNo = reader("ReceiptNo").ToString()

            Dim saleDate As DateTime
            If DateTime.TryParse(reader("SaleDate").ToString(), saleDate) Then
                sale.SaleDate = saleDate
            End If

            sale.CashierId = Convert.ToInt32(reader("CashierId"))
            sale.CashierName = reader("CashierName").ToString()

            If reader.IsDBNull(reader.GetOrdinal("CustomerId")) Then
                sale.CustomerId = Nothing   ' walk-in receipt
            Else
                sale.CustomerId = Convert.ToInt32(reader("CustomerId"))
            End If

            Dim customerName As String = reader("CustomerName").ToString()
            sale.CustomerName = If(customerName.Length > 0, customerName, "Walk-In")
            sale.Subtotal = Convert.ToDecimal(reader("Subtotal"))
            sale.DiscountType = reader("DiscountType").ToString()
            sale.DiscountAmount = Convert.ToDecimal(reader("DiscountAmount"))
            sale.Total = Convert.ToDecimal(reader("Total"))
            sale.CashReceived = Convert.ToDecimal(reader("CashReceived"))
            sale.ChangeAmount = Convert.ToDecimal(reader("ChangeAmount"))
            Return sale
        End Function

        ' Shared list-fill loop for the multi-row reads.
        Private Shared Sub FillSaleList(command As SQLiteCommand, target As List(Of Sale))
            Using reader As SQLiteDataReader = command.ExecuteReader()
                Do While reader.Read()
                    target.Add(MapRowToSale(reader))
                Loop
            End Using
        End Sub

        Public Shared Function GetById(saleId As Integer) As Sale
            If saleId <= 0 Then
                Return Nothing
            End If

            Using connection As SQLiteConnection = DatabaseHelper.GetConnection()
                Using command As New SQLiteCommand(BaseSaleSelectSql & "WHERE s.SaleId = @saleId;", connection)
                    command.Parameters.AddWithValue("@saleId", saleId)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapRowToSale(reader)
                        End If
                    End Using
                End Using
            End Using

            Return Nothing
        End Function

        Public Shared Function GetByReceiptNo(receiptNo As String) As Sale
            Dim lookup As String = If(receiptNo, String.Empty).Trim()
            If lookup.Length = 0 Then
                Return Nothing
            End If

            Using connection As SQLiteConnection = DatabaseHelper.GetConnection()
                Using command As New SQLiteCommand(BaseSaleSelectSql & "WHERE s.ReceiptNo = @receiptNo;", connection)
                    command.Parameters.AddWithValue("@receiptNo", lookup)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapRowToSale(reader)
                        End If
                    End Using
                End Using
            End Using

            Return Nothing
        End Function

        ' The line items of one receipt, oldest-insert first. Joined from
        ' Products so codes/names still resolve after a later deactivate
        ' or price change — a receipt is a frozen document.
        Public Shared Function GetSaleItems(saleId As Integer) As List(Of SaleItem)
            Dim items As New List(Of SaleItem)()
            If saleId <= 0 Then
                Return items
            End If

            Using connection As SQLiteConnection = DatabaseHelper.GetConnection()
                Using command As New SQLiteCommand(
                        "SELECT si.SaleItemId, si.SaleId, si.ProductId, si.Quantity, si.UnitPrice, si.LineTotal, " &
                        "COALESCE(p.ProductCode, 'UNKNOWN') AS ProductCode, " &
                        "COALESCE(p.Name, 'Product no longer exists') AS ProductName " &
                        "FROM SaleItems si " &
                        "LEFT JOIN Products p ON p.ProductId = si.ProductId " &
                        "WHERE si.SaleId = @saleId " &
                        "ORDER BY si.SaleItemId;",
                        connection)
                    command.Parameters.AddWithValue("@saleId", saleId)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        Do While reader.Read()
                            Dim line As New SaleItem()
                            line.SaleItemId = Convert.ToInt32(reader("SaleItemId"))
                            line.SaleId = Convert.ToInt32(reader("SaleId"))
                            line.ProductId = Convert.ToInt32(reader("ProductId"))
                            line.ProductCode = reader("ProductCode").ToString()
                            line.ProductName = reader("ProductName").ToString()
                            line.Quantity = Convert.ToInt32(reader("Quantity"))
                            line.UnitPrice = Convert.ToDecimal(reader("UnitPrice"))
                            items.Add(line)
                        Loop
                    End Using
                End Using
            End Using

            Return items
        End Function

#End Region

#Region "History reads (Today / Owner transactions / Customer history)"

        ' ISO-text date columns compare through the first 10 characters
        ' (yyyy-MM-dd). Comparison STRINGS travel as parameters; only the
        ' fixed SQL structure is ever literal.

        ' Today's list (sales of all cashiers on the given day) — the
        ' cashier "Today" tab query (spec §7.5).
        Public Shared Function GetSalesByDate(day As DateTime) As List(Of Sale)
            Return GetSalesWhere("substr(s.SaleDate, 1, 10) = @day",
                                 Sub(command) command.Parameters.AddWithValue("@day", day.ToString("yyyy-MM-dd")))
        End Function

        ' Inclusive date range — the Owner Transactions browser (M15).
        Public Shared Function GetSalesByDateRange(startDate As DateTime, endDate As DateTime) As List(Of Sale)
            Dim startDay As DateTime = startDate.Date
            Dim endDay As DateTime = endDate.Date
            If endDay < startDay Then
                ' UI-belt normalization: an inverted range still returns
                ' the span, smallest first.
                Dim swap As DateTime = startDay
                startDay = endDay
                endDay = swap
            End If

            Return GetSalesWhere("substr(s.SaleDate, 1, 10) >= @startDay AND substr(s.SaleDate, 1, 10) <= @endDay",
                                 Sub(command)
                                     command.Parameters.AddWithValue("@startDay", startDay.ToString("yyyy-MM-dd"))
                                     command.Parameters.AddWithValue("@endDay", endDay.ToString("yyyy-MM-dd"))
                                 End Sub)
        End Function

        ' Personal purchase history — the Customer History tab (M12).
        Public Shared Function GetSalesByCustomer(customerId As Integer) As List(Of Sale)
            Return GetSalesWhere("s.CustomerId = @customerId",
                                 Sub(command) command.Parameters.AddWithValue("@customerId", customerId))
        End Function

        ' Per-cashier reconciliation — today's receipts of ONE cashier.
        Public Shared Function GetSalesByCashierAndDate(cashierId As Integer, day As DateTime) As List(Of Sale)
            Return GetSalesWhere("s.CashierId = @cashierId AND substr(s.SaleDate, 1, 10) = @day",
                                 Sub(command)
                                     command.Parameters.AddWithValue("@cashierId", cashierId)
                                     command.Parameters.AddWithValue("@day", day.ToString("yyyy-MM-dd"))
                                 End Sub)
        End Function

        ' Shared runner for the four filters above: one WHERE clause
        ' string, one parameter-filling action, one ORDER BY.
        Private Shared Function GetSalesWhere(whereClause As String,
                                              bindParameters As Action(Of SQLiteCommand)) As List(Of Sale)
            Dim sales As New List(Of Sale)()

            Using connection As SQLiteConnection = DatabaseHelper.GetConnection()
                Using command As New SQLiteCommand(
                        BaseSaleSelectSql &
                        "WHERE " & whereClause & " " &
                        "ORDER BY s.SaleDate DESC, s.SaleId DESC;",
                        connection)
                    bindParameters(command)
                    FillSaleList(command, sales)
                End Using
            End Using

            Return sales
        End Function

#End Region

    End Class

End Namespace
