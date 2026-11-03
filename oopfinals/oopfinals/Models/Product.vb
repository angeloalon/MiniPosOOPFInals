Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/Product.vb — Core inventory entity.
'
' OOP concept: ENCAPSULATION
'   - Private backing fields; every property setter validates and throws
'     so an invalid object state is impossible to construct.
'   - Display-only properties (CategoryName / SupplierName) carry no
'     validation because they are presentation data filled by JOINs.
'
' OOP concept: ABSTRACTION of stock & expiry classification:
'   - IsLowStock / IsExpired / IsExpiringSoon / GetStockStatus are the one
'     definition reused by Clerk alerts, Owner reports, and the Customer
'     catalog — no duplicate status logic in any screen or query.
' =====================================================================

Namespace Models

    Public Class Product

#Region "Backing fields"

        Private _productId As Integer
        Private _productCode As String
        Private _name As String
        Private _categoryId As Integer
        Private _categoryName As String
        Private _supplierId As Integer
        Private _supplierName As String
        Private _price As Decimal
        Private _costPrice As Decimal
        Private _stockQty As Integer
        Private _reorderLevel As Integer
        Private _expiryDate As Nullable(Of DateTime)
        Private _isActive As Boolean

#End Region

#Region "Constructors"

        Public Sub New()
            _productCode = String.Empty
            _name = String.Empty
            _categoryId = 0
            _supplierId = 0
            _price = 0D
            _costPrice = 0D
            _stockQty = 0
            _reorderLevel = 10
            _isActive = True
        End Sub

        Public Sub New(productCode As String,
                       name As String,
                       categoryId As Integer,
                       supplierId As Integer,
                       price As Decimal,
                       costPrice As Decimal,
                       stockQty As Integer,
                       reorderLevel As Integer,
                       Optional expiryDate As Nullable(Of DateTime) = Nothing)
            Me.New()
            Me.ProductCode = productCode
            Me.Name = name
            Me.CategoryId = categoryId
            Me.SupplierId = supplierId
            Me.Price = price
            Me.CostPrice = costPrice
            Me.StockQty = stockQty
            Me.ReorderLevel = reorderLevel
            Me.ExpiryDate = expiryDate
        End Sub

#End Region

#Region "Validated properties"

        Public Property ProductId As Integer
            Get
                Return _productId
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("Product id cannot be negative.")
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

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("Product name cannot be empty.")
                End If
                _name = value.Trim()
            End Set
        End Property

        Public Property CategoryId As Integer
            Get
                Return _categoryId
            End Get
            Set(value As Integer)
                If value <= 0 Then
                    Throw New ArgumentException("A valid Category must be selected.")
                End If
                _categoryId = value
            End Set
        End Property

        Public Property CategoryName As String
            Get
                Return _categoryName
            End Get
            Set(value As String)
                _categoryName = If(value, String.Empty).Trim()
            End Set
        End Property

        Public Property SupplierId As Integer
            Get
                Return _supplierId
            End Get
            Set(value As Integer)
                If value <= 0 Then
                    Throw New ArgumentException("A valid Supplier must be selected.")
                End If
                _supplierId = value
            End Set
        End Property

        Public Property SupplierName As String
            Get
                Return _supplierName
            End Get
            Set(value As String)
                _supplierName = If(value, String.Empty).Trim()
            End Set
        End Property

        Public Property Price As Decimal
            Get
                Return _price
            End Get
            Set(value As Decimal)
                If value < 0D Then
                    Throw New ArgumentException("Price cannot be negative.")
                End If
                _price = value
            End Set
        End Property

        Public Property CostPrice As Decimal
            Get
                Return _costPrice
            End Get
            Set(value As Decimal)
                If value < 0D Then
                    Throw New ArgumentException("Cost price cannot be negative.")
                End If
                _costPrice = value
            End Set
        End Property

        Public Property StockQty As Integer
            Get
                Return _stockQty
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("Stock quantity cannot be negative.")
                End If
                _stockQty = value
            End Set
        End Property

        Public Property ReorderLevel As Integer
            Get
                Return _reorderLevel
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("Reorder level cannot be negative.")
                End If
                _reorderLevel = value
            End Set
        End Property

        Public Property ExpiryDate As Nullable(Of DateTime)
            Get
                Return _expiryDate
            End Get
            Set(value As Nullable(Of DateTime))
                _expiryDate = value
            End Set
        End Property

        Public Property IsActive As Boolean
            Get
                Return _isActive
            End Get
            Set(value As Boolean)
                _isActive = value
            End Set
        End Property

#End Region

#Region "Stock & expiry classification (single source of truth)"

        Public Function IsLowStock() As Boolean
            Return _stockQty <= _reorderLevel
        End Function

        Public Function IsExpired() As Boolean
            If Not _expiryDate.HasValue Then
                Return False
            End If
            Return _expiryDate.Value.Date < DateTime.Today
        End Function

        Public Function IsExpiringSoon(Optional daysThreshold As Integer = 30) As Boolean
            If Not _expiryDate.HasValue Then
                Return False
            End If
            Dim daysLeft As Integer = CInt(Math.Floor((_expiryDate.Value.Date - DateTime.Today).TotalDays))
            Return daysLeft >= 0 AndAlso daysLeft <= daysThreshold
        End Function

        Public Function GetStockStatus() As String
            If _stockQty <= 0 Then
                Return "Out of Stock"
            End If
            If IsLowStock() Then
                Return "Low Stock"
            End If
            Return "In Stock"
        End Function

#End Region

#Region "Presentation"

        ' Renders as "PRD001 - Dinorado Special Rice 5kg (PHP 280.00)" so
        ' combo boxes and grids stay readable with no extra plumbing.
        Public Overrides Function ToString() As String
            Return String.Format("{0} - {1} (PHP {2:N2})", _productCode, _name, _price)
        End Function

#End Region

    End Class

End Namespace
