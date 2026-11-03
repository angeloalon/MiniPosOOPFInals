Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/SaleItem.vb — One frozen line-item od a receipt.
'
' OOP concept: ENCAPSULATION
'   - LineTotal is a GET-ONLY computed property
'     (Round(Quantity x UnitPrice, 2, MidpointRounding.AwayFromZero)) —
'     clients cannot corrupt the line math.
'   - Quantity must be > 0 and UnitPrice >= 0.
' =====================================================================

Namespace Models

    Public Class SaleItem

        Private _saleItemId As Integer
        Private _saleId As Integer
        Private _productId As Integer
        Private _productCode As String
        Private _productName As String
        Private _quantity As Integer
        Private _unitPrice As Decimal

        Public Sub New()
            _productCode = String.Empty
            _productName = String.Empty
        End Sub

        Public Sub New(productId As Integer,
                       productCode As String,
                       productName As String,
                       quantity As Integer,
                       unitPrice As Decimal)
            Me.New()
            Me.ProductId = productId
            Me.ProductCode = productCode
            Me.ProductName = productName
            Me.Quantity = quantity
            Me.UnitPrice = unitPrice
        End Sub

        Public Property SaleItemId As Integer
            Get
                Return _saleItemId
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("Sale item id cannot be negative.")
                End If
                _saleItemId = value
            End Set
        End Property

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

        Public Property ProductId As Integer
            Get
                Return _productId
            End Get
            Set(value As Integer)
                If value <= 0 Then
                    Throw New ArgumentException("A valid Product must be selected.")
                End If
                _productId = value
            End Set
        End Property

        Public Property ProductCode As String
            Get
                Return _productCode
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("Product code cannot be empty.")
                End If
                _productCode = value.Trim().ToUpperInvariant()
            End Set
        End Property

        Public Property ProductName As String
            Get
                Return _productName
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("Product name cannot be empty.")
                End If
                _productName = value.Trim()
            End Set
        End Property

        Public Property Quantity As Integer
            Get
                Return _quantity
            End Get
            Set(value As Integer)
                If value <= 0 Then
                    Throw New ArgumentException("Sale quantity must be greater than zero.")
                End If
                _quantity = value
            End Set
        End Property

        Public Property UnitPrice As Decimal
            Get
                Return _unitPrice
            End Get
            Set(value As Decimal)
                If value < 0D Then
                    Throw New ArgumentException("Unit price cannot be negative.")
                End If
                _unitPrice = value
            End Set
        End Property

        ' GET-ONLY: computed at the model boundary and rounded consistently.
        Public ReadOnly Property LineTotal As Decimal
            Get
                Return Math.Round(_quantity * _unitPrice, 2, MidpointRounding.AwayFromZero)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0} - {1} x{2}", _productCode, _productName, _quantity)
        End Function

    End Class

End Namespace
