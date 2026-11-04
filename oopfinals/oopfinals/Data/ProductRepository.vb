Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Data/ProductRepository.vb — catalog persistence + stock/expiry alert
' queries (spec §7.4). The ONLY file allowed to touch the Products
' table alongside DatabaseHelper's seed; every statement is
' parameterized (@param + Parameters.AddWithValue) — zero data built
' into SQL text.
'
' OOP concept: POLYMORPHISM (data side)
'   Every read hands back fully-hydrated Product instances; the SAME
'   Product objects then consume different behavior downstream purely
'   by virtual dispatch (IsLowStock/IsExpired/IsExpiringSoon drive POS
'   rows, Clerk alerts, and Owner reports through one definition).
'
' OOP concept: ENCAPSULATION
'   NotInheritable + Private Sub New — the visible surface is Shared
'   members only. Add/Update restate the friendly uniqueness messages
'   at the persistence boundary; DeleteOrDeactivate prefers the SOFT
'   route (IsActive=0) so historical receipts keep resolving their
'   FKs. The alert SQL intentionally mirrors the model's predicates
'   exactly (qty ≤ reorder expiry < today 0 ≤ days-left ≤ 30) so the
'   single source of truth is echoed, never re-invented, at SQL level.
'
' LAYERING: Data → Models only (Product). No Services, no Forms.
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Data.SQLite
Imports oopfinals.Models

Namespace Data

    Public NotInheritable Class ProductRepository

        ' Static gate: repositories are stateless, never instantiated.
        Private Sub New()
        End Sub

#Region "Connection + row mapping"

        ' DatabaseHelper owns the canonical path/connection string
        ' (spec §7.1); every operation opens a fresh, short-lived one.
        Private Shared Function CreateConnection() As SQLiteConnection
            Return DatabaseHelper.GetConnection()
        End Function

        ' SELECT shape shared by every read: the JOINs hydrate the
        ' display props (CategoryName / SupplierName) that grids, combos
        ' and cards show without further lookups.
        Private Const BaseSelectSql As String =
            "SELECT p.ProductId, p.ProductCode, p.Name, p.CategoryId, c.Name AS CategoryName, " &
            "p.SupplierId, s.Name AS SupplierName, p.Price, p.CostPrice, p.StockQty, " &
            "p.ReorderLevel, p.ExpiryDate, p.IsActive " &
            "FROM Products p " &
            "INNER JOIN Categories c ON c.CategoryId = p.CategoryId " &
            "INNER JOIN Suppliers s ON s.SupplierId = p.SupplierId "

        ' Materializes the current reader row into a validated Product.
        ' Money comes back through Convert.ToDecimal and the setters
        ' re-validate, so a corrupted row reports the model's message.
        Private Shared Function MapRowToProduct(reader As SQLiteDataReader) As Product
            Dim product As New Product()
            product.ProductId = Convert.ToInt32(reader("ProductId"))
            product.ProductCode = reader("ProductCode").ToString()
            product.Name = reader("Name").ToString()
            product.CategoryId = Convert.ToInt32(reader("CategoryId"))
            product.CategoryName = reader("CategoryName").ToString()
            product.SupplierId = Convert.ToInt32(reader("SupplierId"))
            product.SupplierName = reader("SupplierName").ToString()
            product.Price = Convert.ToDecimal(reader("Price"))
            product.CostPrice = Convert.ToDecimal(reader("CostPrice"))
            product.StockQty = Convert.ToInt32(reader("StockQty"))
            product.ReorderLevel = Convert.ToInt32(reader("ReorderLevel"))
            product.ExpiryDate = ReadNullableDateTime(reader, "ExpiryDate")
            product.IsActive = Convert.ToInt32(reader("IsActive")) = 1
            Return product
        End Function

        ' The ExpiryDate column is TEXT ISO stamps or NULL (spec §8);
        ' DBNull flows through as Nothing (the "no expiry" path).
        Private Shared Function ReadNullableDateTime(reader As SQLiteDataReader,
                                                     columnName As String) As Nullable(Of DateTime)
            If reader.IsDBNull(reader.GetOrdinal(columnName)) Then
                Return Nothing
            End If
            Dim parsed As DateTime
            If DateTime.TryParse(reader(columnName).ToString(), parsed) Then
                Return parsed
            End If
            Return Nothing
        End Function

        ' Shared list-fill loop for every multi-row read.
        Private Shared Sub FillProductList(command As SQLiteCommand, target As List(Of Product))
            Using reader As SQLiteDataReader = command.ExecuteReader()
                Do While reader.Read()
                    target.Add(MapRowToProduct(reader))
                Loop
            End Using
        End Sub

#End Region

#Region "Reads"

        ' Full catalog, optionally active-only (POS/customer want active
        ' rows; the Owner history path reads everything). @isActive is
        ' only bound on the active-only branch, matching the WHERE.
        Public Shared Function GetAll(Optional activeOnly As Boolean = True) As List(Of Product)
            Dim products As New List(Of Product)()

            Using connection As SQLiteConnection = CreateConnection()
                Dim sql As String = BaseSelectSql
                If activeOnly Then
                    sql &= "WHERE p.IsActive = @isActive "
                End If
                sql &= "ORDER BY p.Name;"

                Using command As New SQLiteCommand(sql, connection)
                    If activeOnly Then
                        command.Parameters.AddWithValue("@isActive", 1)
                    End If
                    FillProductList(command, products)
                End Using
            End Using

            Return products
        End Function

        ' LIKE search on code/name with an optional category filter
        ' (categoryId ≤ 0 = All categories — the POS combo's sentinel
        ' row). Only FIXED structure strings are concatenated; every
        ' user-controlled datum travels as a parameter.
        Public Shared Function Search(Optional keyword As String = "",
                                      Optional categoryId As Integer = 0,
                                      Optional activeOnly As Boolean = True) As List(Of Product)
            Dim products As New List(Of Product)()
            Dim lookup As String = If(keyword, String.Empty).Trim()

            Using connection As SQLiteConnection = CreateConnection()
                Dim clauses As New List(Of String)()
                Dim sql As String = BaseSelectSql

                If activeOnly Then
                    clauses.Add("p.IsActive = @isActive")
                End If
                If lookup.Length > 0 Then
                    clauses.Add("(p.ProductCode LIKE @likePattern OR p.Name LIKE @likePattern)")
                End If
                If categoryId > 0 Then
                    clauses.Add("p.CategoryId = @categoryId")
                End If

                If clauses.Count > 0 Then
                    sql &= "WHERE " & String.Join(" AND ", clauses.ToArray()) & " "
                End If
                sql &= "ORDER BY p.Name;"

                Using command As New SQLiteCommand(sql, connection)
                    If activeOnly Then
                        command.Parameters.AddWithValue("@isActive", 1)
                    End If
                    If lookup.Length > 0 Then
                        ' Parameterized wildcard — the keyword itself
                        ' never becomes part of the SQL text.
                        command.Parameters.AddWithValue("@likePattern", "%" & lookup & "%")
                    End If
                    If categoryId > 0 Then
                        command.Parameters.AddWithValue("@categoryId", categoryId)
                    End If
                    FillProductList(command, products)
                End Using
            End Using

            Return products
        End Function

        Public Shared Function GetById(productId As Integer) As Product
            If productId <= 0 Then
                Return Nothing
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(BaseSelectSql & "WHERE p.ProductId = @productId;", connection)
                    command.Parameters.AddWithValue("@productId", productId)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapRowToProduct(reader)
                        End If
                    End Using
                End Using
            End Using

            Return Nothing
        End Function

        Public Shared Function GetByCode(productCode As String) As Product
            Dim lookup As String = If(productCode, String.Empty).Trim().ToUpperInvariant()
            If lookup.Length = 0 Then
                Return Nothing
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(BaseSelectSql & "WHERE UPPER(p.ProductCode) = @productCode;", connection)
                    command.Parameters.AddWithValue("@productCode", lookup)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapRowToProduct(reader)
                        End If
                    End Using
                End Using
            End Using

            Return Nothing
        End Function

#End Region

#Region "Writes"

        ' Friendly pre-check for the UNIQUE(ProductCode) constraint: the
        ' duplication message surfaces BEFORE the raw constraint error.
        ' excludeProductId = 0 checks for any other row (insert path).
        Private Shared Function ProductCodeExists(productCode As String, excludeProductId As Integer) As Boolean
            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(
                        "SELECT COUNT(1) FROM Products WHERE UPPER(ProductCode) = @productCode AND ProductId <> @productId;",
                        connection)
                    command.Parameters.AddWithValue("@productCode", productCode)
                    command.Parameters.AddWithValue("@productId", excludeProductId)
                    Return Convert.ToInt64(command.ExecuteScalar()) > 0
                End Using
            End Using
        End Function

        ' Insert one catalog row; returns the new Products.ProductId.
        Public Shared Function Add(product As Product) As Integer
            If product Is Nothing Then
                Throw New ArgumentNullException(NameOf(product))
            End If

            ' Fence: the validating setters are the usual wall, but a
            ' Product assembled without them could carry a blank code —
            ' restated here with the model's own message.
            If String.IsNullOrWhiteSpace(product.ProductCode) Then
                Throw New ArgumentException("Product code cannot be empty.")
            End If
            If ProductCodeExists(product.ProductCode, 0) Then
                Throw New InvalidOperationException(
                    "Product code '" & product.ProductCode & "' already exists.")
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(
                        "INSERT INTO Products (ProductCode, Name, CategoryId, SupplierId, Price, CostPrice, StockQty, ReorderLevel, ExpiryDate, IsActive) " &
                        "VALUES (@productCode, @name, @categoryId, @supplierId, @price, @costPrice, @stockQty, @reorderLevel, @expiryDate, @isActive);",
                        connection)
                    AddProductParameters(command, product)
                    command.ExecuteNonQuery()
                End Using

                Using idCommand As New SQLiteCommand("SELECT last_insert_rowid();", connection)
                    Return Convert.ToInt32(idCommand.ExecuteScalar())
                End Using
            End Using
        End Function

        ' Full edit (M14/M15 Products tab). A rename that grabs another
        ' product's unique code surfaces as the friendly message.
        Public Shared Function Update(product As Product) As Boolean
            If product Is Nothing Then
                Throw New ArgumentNullException(NameOf(product))
            End If
            If String.IsNullOrWhiteSpace(product.ProductCode) Then
                Throw New ArgumentException("Product code cannot be empty.")
            End If
            If ProductCodeExists(product.ProductCode, product.ProductId) Then
                Throw New InvalidOperationException(
                    "Product code '" & product.ProductCode & "' already exists.")
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(
                        "UPDATE Products SET ProductCode = @productCode, Name = @name, CategoryId = @categoryId, " &
                        "SupplierId = @supplierId, Price = @price, CostPrice = @costPrice, StockQty = @stockQty, " &
                        "ReorderLevel = @reorderLevel, ExpiryDate = @expiryDate, IsActive = @isActive " &
                        "WHERE ProductId = @productId;",
                        connection)
                    AddProductParameters(command, product)
                    command.Parameters.AddWithValue("@productId", product.ProductId)
                    Return command.ExecuteNonQuery() > 0
                End Using
            End Using
        End Function

        ' Dedicated price-edit path (Owner quick re-price). Narrowed to
        ' the two money columns so stock/categories cannot drift, and
        ' money is rounded at the boundary exactly like the model does.
        Public Shared Function UpdatePrice(productId As Integer, newPrice As Decimal, newCostPrice As Decimal) As Boolean
            If newPrice < 0D OrElse newCostPrice < 0D Then
                Throw New ArgumentException("Prices cannot be negative.")
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(
                        "UPDATE Products SET Price = @price, CostPrice = @costPrice WHERE ProductId = @productId;",
                        connection)
                    command.Parameters.AddWithValue("@price", Math.Round(newPrice, 2, MidpointRounding.AwayFromZero))
                    command.Parameters.AddWithValue("@costPrice", Math.Round(newCostPrice, 2, MidpointRounding.AwayFromZero))
                    command.Parameters.AddWithValue("@productId", productId)
                    Return command.ExecuteNonQuery() > 0
                End Using
            End Using
        End Function

        ' SOFT delete preferred (spec §7.4): deactivate keeps historical
        ' SaleItems/StockMovements resolvable through their FK. The hard
        ' DELETE only runs when NO child rows exist; otherwise the soft
        ' path deactivates. Either way a missing id reports False.
        Public Shared Function DeleteOrDeactivate(productId As Integer) As Boolean
            Using connection As SQLiteConnection = CreateConnection()
                Using transaction As SQLiteTransaction = connection.BeginTransaction()
                    Try
                        Dim childCount As Long = 0

                        ' FK-safety probe: any receipt or audit row pins
                        ' this product to history.
                        Using probe As New SQLiteCommand(
                                "SELECT (SELECT COUNT(*) FROM SaleItems WHERE ProductId = @productId) " &
                                "+ (SELECT COUNT(*) FROM StockMovements WHERE ProductId = @productId);",
                                connection, transaction)
                            probe.Parameters.AddWithValue("@productId", productId)
                            childCount = Convert.ToInt64(probe.ExecuteScalar())
                        End Using

                        Dim updated As Integer
                        If childCount > 0 Then
                            Using soft As New SQLiteCommand(
                                    "UPDATE Products SET IsActive = 0 WHERE ProductId = @productId;",
                                    connection, transaction)
                                soft.Parameters.AddWithValue("@productId", productId)
                                updated = soft.ExecuteNonQuery()
                            End Using
                        Else
                            Using hard As New SQLiteCommand(
                                    "DELETE FROM Products WHERE ProductId = @productId;",
                                    connection, transaction)
                                hard.Parameters.AddWithValue("@productId", productId)
                                updated = hard.ExecuteNonQuery()
                            End Using
                        End If

                        transaction.Commit()
                        Return updated > 0
                    Catch deleteError As Exception
                        ' Best-effort rollback guarded in its own Try/Catch;
                        ' the ORIGINAL error is rethrown either way.
                        Try
                            transaction.Rollback()
                        Catch
                            ' Rollback failure observed and deliberately set aside.
                        End Try
                        Throw deleteError
                    End Try
                End Using
            End Using
        End Function

        ' One parameter stanza shared by Add and Update so the two can
        ' never drift apart. Both @expiryDate branches bind EXACTLY once.
        Private Shared Sub AddProductParameters(command As SQLiteCommand, product As Product)
            command.Parameters.AddWithValue("@productCode", product.ProductCode)
            command.Parameters.AddWithValue("@name", product.Name)
            command.Parameters.AddWithValue("@categoryId", product.CategoryId)
            command.Parameters.AddWithValue("@supplierId", product.SupplierId)
            command.Parameters.AddWithValue("@price", Math.Round(product.Price, 2, MidpointRounding.AwayFromZero))
            command.Parameters.AddWithValue("@costPrice", Math.Round(product.CostPrice, 2, MidpointRounding.AwayFromZero))
            command.Parameters.AddWithValue("@stockQty", product.StockQty)
            command.Parameters.AddWithValue("@reorderLevel", product.ReorderLevel)
            command.Parameters.AddWithValue("@isActive", If(product.IsActive, 1, 0))

            If product.ExpiryDate.HasValue Then
                command.Parameters.AddWithValue("@expiryDate", product.ExpiryDate.Value.ToString(DatabaseHelper.IsoDateTimeFormat))
            Else
                command.Parameters.AddWithValue("@expiryDate", DBNull.Value)
            End If
        End Sub

#End Region

#Region "Alert queries (single source of truth stays Product)"

        ' These run against ALL products (active OR deactivated) —
        ' deactivated goods still hold shelf stock worth alerting on.
        ' Each WHERE predicate deliberately mirrors one Product model
        ' helper: IsLowStock / IsExpired / IsExpiringSoon(30).
        '
        ' ISO TEXT dates compare through SQLite's date() extractor;
        ' 'now','localtime' matches the DateTime.Today the model uses.

        ' qty ≤ reorder level (Product.IsLowStock).
        Public Shared Function GetLowStockProducts() As List(Of Product)
            Dim products As New List(Of Product)()

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(
                        BaseSelectSql &
                        "WHERE p.StockQty <= p.ReorderLevel " &
                        "ORDER BY p.StockQty, p.Name;", connection)
                    FillProductList(command, products)
                End Using
            End Using

            Return products
        End Function

        ' Expiring WITHIN daysThreshold but not yet expired — same
        ' window as Product.IsExpiringSoon(daysThreshold) (default 30 d
        ' per spec §7.4; the Clerk Alerts tab and Owner KPIs consume it).
        ' @days travels as a parameter, concatenated by SQLite itself.
        Public Shared Function GetExpiringProducts(Optional daysThreshold As Integer = 30) As List(Of Product)
            Dim products As New List(Of Product)()

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(
                        BaseSelectSql &
                        "WHERE p.ExpiryDate IS NOT NULL " &
                        "AND date(p.ExpiryDate) >= date('now','localtime') " &
                        "AND date(p.ExpiryDate) <= date('now','localtime','+' || @days || ' days') " &
                        "ORDER BY p.ExpiryDate, p.Name;", connection)
                    command.Parameters.AddWithValue("@days", daysThreshold)
                    FillProductList(command, products)
                End Using
            End Using

            Return products
        End Function

        ' Already expired (expiry strictly in the past), worst first.
        Public Shared Function GetExpiredProducts() As List(Of Product)
            Dim products As New List(Of Product)()

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(
                        BaseSelectSql &
                        "WHERE p.ExpiryDate IS NOT NULL " &
                        "AND date(p.ExpiryDate) < date('now','localtime') " &
                        "ORDER BY p.ExpiryDate, p.Name;", connection)
                    FillProductList(command, products)
                End Using
            End Using

            Return products
        End Function

#End Region

    End Class

End Namespace
