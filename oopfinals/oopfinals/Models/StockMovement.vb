Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/StockMovement.vb — Immutable inventory audit record. Every
' stock touch in the system eventually lands here: StockIn (+qty),
' Adjustment (±qty), Sale (−qty e.g. "POS Sale: RCP-20261103-0001").
'
' OOP concept: ENCAPSULATION — validated, self-consistent audit rows;
' QuantityChange carries the sign so the ledger can be replayed.
' =====================================================================

Namespace Models

    Public Class StockMovement

        Public Const MovementTypeStockIn As String = "StockIn"
        Public Const MovementTypeAdjustment As String = "Adjustment"
        Public Const MovementTypeSale As String = "Sale"

        Private _movementId As Integer
        Private _productId As Integer
        Private _productCode As String
        Private _productName As String
        Private _movementType As String
        Private _quantityChange As Integer
        Private _reason As String
        Private _supplierId As Nullable(Of Integer)
        Private _supplierName As String
        Private _userId As Integer
        Private _userName As String
        Private _movementDate As DateTime

        Public Sub New()
            _productCode = String.Empty
            _productName = String.Empty
            _movementType = MovementTypeAdjustment
            _quantityChange = 0
            _reason = String.Empty
            _supplierName = String.Empty
            _userName = String.Empty
            _movementDate = DateTime.Now
        End Sub

        Public Property MovementId As Integer
            Get
                Return _movementId
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("Movement id cannot be negative.")
                End If
                _movementId = value
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

        Public Property MovementType As String
            Get
                Return _movementType
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("Movement type cannot be empty.")
                End If
                Dim normalized As String = value.Trim()
                If normalized <> MovementTypeStockIn AndAlso
                   normalized <> MovementTypeAdjustment AndAlso
                   normalized <> MovementTypeSale Then
                    Throw New ArgumentException("Movement type must be StockIn, Adjustment, or Sale.")
                End If
                _movementType = normalized
            End Set
        End Property

        Public Property QuantityChange As Integer
            Get
                Return _quantityChange
            End Get
            Set(value As Integer)
                If value = 0 Then
                    Throw New ArgumentException("Quantity change cannot be zero.")
                End If
                _quantityChange = value
            End Set
        End Property

        Public Property Reason As String
            Get
                Return _reason
            End Get
            Set(value As String)
                _reason = If(value, String.Empty).Trim()
            End Set
        End Property

        Public Property SupplierId As Nullable(Of Integer)
            Get
                Return _supplierId
            End Get
            Set(value As Nullable(Of Integer))
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

        Public Property UserId As Integer
            Get
                Return _userId
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("User id cannot be negative.")
                End If
                _userId = value
            End Set
        End Property

        Public Property UserName As String
            Get
                Return _userName
            End Get
            Set(value As String)
                _userName = If(value, String.Empty).Trim()
            End Set
        End Property

        Public Property MovementDate As DateTime
            Get
                Return _movementDate
            End Get
            Set(value As DateTime)
                _movementDate = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Dim sign As String = If(_quantityChange > 0, "+", "")
            Return String.Format("{0:yyyy-MM-dd HH:mm} {1} {2} {3}{4} ({5})",
                                 _movementDate, _productCode, _movementType, sign, _quantityChange, _reason)
        End Function

    End Class

End Namespace
