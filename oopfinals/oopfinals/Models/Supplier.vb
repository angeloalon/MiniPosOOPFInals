Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/Supplier.vb — Master-data entity backing stock-in deliveries.
' Encapsulation: the supplier name is validated on every set; contact
' number formatting is enforced by Services/ValidationRules at the UI
' boundary, so the model keeps the raw values (display concern only).
' =====================================================================

Namespace Models

    Public Class Supplier

        Private _supplierId As Integer
        Private _name As String
        Private _contactNumber As String
        Private _address As String

        Public Sub New()
            _name = String.Empty
            _contactNumber = String.Empty
            _address = String.Empty
        End Sub

        Public Sub New(supplierId As Integer, name As String, contactNumber As String, address As String)
            Me.New()
            _supplierId = supplierId
            Me.Name = name
            _contactNumber = If(contactNumber, String.Empty).Trim()
            _address = If(address, String.Empty).Trim()
        End Sub

        Public Property SupplierId As Integer
            Get
                Return _supplierId
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("Supplier id cannot be negative.")
                End If
                _supplierId = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("Supplier name cannot be empty.")
                End If
                _name = value.Trim()
            End Set
        End Property

        Public Property ContactNumber As String
            Get
                Return _contactNumber
            End Get
            Set(value As String)
                _contactNumber = If(value, String.Empty).Trim()
            End Set
        End Property

        Public Property Address As String
            Get
                Return _address
            End Get
            Set(value As String)
                _address = If(value, String.Empty).Trim()
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return _name
        End Function

    End Class

End Namespace
