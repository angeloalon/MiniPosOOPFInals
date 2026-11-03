Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/Sale.vb — The receipt aggregate (one row per receipt).
'
' OOP concept: AGGREGATION / COMPOSITION — a receipt HAS line items
' (Items As List(Of SaleItem)).
' OOP concept: ENCAPSULATION — every money setter rejects negatives;
' DiscountAmount carries the human-readable discount name in DiscountType.
' CustomerId is nullable to support walk-in customers (no account).
' =====================================================================

Namespace Models

    Public Class Sale

        Private _saleId As Integer
        Private _receiptNo As String
        Private _saleDate As DateTime
        Private _cashierId As Integer
        Private _cashierName As String
        Private _customerId As Nullable(Of Integer)
        Private _customerName As String
        Private _subtotal As Decimal
        Private _discountType As String
        Private _discountAmount As Decimal
        Private _total As Decimal
        Private _cashReceived As Decimal
        Private _changeAmount As Decimal
        Private _items As List(Of SaleItem)

        Public Sub New()
            _receiptNo = String.Empty
            _cashierName = String.Empty
            _discountType = "None"
            _items = New List(Of SaleItem)()
        End Sub

        Public Sub New(receiptNo As String, cashierId As Integer, subtotal As Decimal, total As Decimal)
            Me.New()
            Me.ReceiptNo = receiptNo
            Me.CashierId = cashierId
            Me.Subtotal = subtotal
            Me.Total = total
        End Sub

#Region "Identity"

        Public Property SaleId As Integer
            Get
                Return _saleId
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("Sale id cannot be negative.")
                End If
                _saleId = value
            End Set
        End Property

        Public Property ReceiptNo As String
            Get
                Return _receiptNo
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("Receipt number cannot be empty.")
                End If
                _receiptNo = value.Trim()
            End Set
        End Property

        Public Property SaleDate As DateTime
            Get
                Return _saleDate
            End Get
            Set(value As DateTime)
                _saleDate = value
            End Set
        End Property

#End Region

#Region "Actors"

        Public Property CashierId As Integer
            Get
                Return _cashierId
            End Get
            Set(value As Integer)
                If value <= 0 Then
                    Throw New ArgumentException("A valid Cashier must be selected.")
                End If
                _cashierId = value
            End Set
        End Property

        Public Property CashierName As String
            Get
                Return _cashierName
            End Get
            Set(value As String)
                _cashierName = If(value, String.Empty).Trim()
            End Set
        End Property

        Public Property CustomerId As Nullable(Of Integer)
            Get
                Return _customerId
            End Get
            Set(value As Nullable(Of Integer))
                _customerId = value
            End Set
        End Property

        Public Property CustomerName As String
            Get
                Return _customerName
            End Get
            Set(value As String)
                _customerName = If(value, String.Empty).Trim()
            End Set
        End Property

#End Region

#Region "Money (all reject negatives)"

        Public Property Subtotal As Decimal
            Get
                Return _subtotal
            End Get
            Set(value As Decimal)
                If value < 0D Then
                    Throw New ArgumentException("Subtotal cannot be negative.")
                End If
                _subtotal = value
            End Set
        End Property

        Public Property DiscountType As String
            Get
                Return _discountType
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    _discountType = "None"
                Else
                    _discountType = value.Trim()
                End If
            End Set
        End Property

        Public Property DiscountAmount As Decimal
            Get
                Return _discountAmount
            End Get
            Set(value As Decimal)
                If value < 0D Then
                    Throw New ArgumentException("Discount amount cannot be negative.")
                End If
                _discountAmount = value
            End Set
        End Property

        Public Property Total As Decimal
            Get
                Return _total
            End Get
            Set(value As Decimal)
                If value < 0D Then
                    Throw New ArgumentException("Total cannot be negative.")
                End If
                _total = value
            End Set
        End Property

        Public Property CashReceived As Decimal
            Get
                Return _cashReceived
            End Get
            Set(value As Decimal)
                If value < 0D Then
                    Throw New ArgumentException("Cash received cannot be negative.")
                End If
                _cashReceived = value
            End Set
        End Property

        Public Property ChangeAmount As Decimal
            Get
                Return _changeAmount
            End Get
            Set(value As Decimal)
                If value < 0D Then
                    Throw New ArgumentException("Change amount cannot be negative.")
                End If
                _changeAmount = value
            End Set
        End Property

#End Region

#Region "Aggregated line items"

        Public Property Items As List(Of SaleItem)
            Get
                Return _items
            End Get
            Set(value As List(Of SaleItem))
                If value Is Nothing Then
                    Throw New ArgumentNullException("value", "Sale items cannot be Nothing.")
                End If
                _items = value
            End Set
        End Property

        Public Sub AddItem(item As SaleItem)
            If item Is Nothing Then
                Throw New ArgumentNullException("item", "A sale must have a valid line item.")
            End If
            _items.Add(item)
        End Sub

        Public Function GetLineTotalSum() As Decimal
            Dim sum As Decimal = 0D
            For Each item As SaleItem In _items
                sum += item.LineTotal
            Next
            Return Math.Round(sum, 2, MidpointRounding.AwayFromZero)
        End Function

#End Region

    End Class

End Namespace
