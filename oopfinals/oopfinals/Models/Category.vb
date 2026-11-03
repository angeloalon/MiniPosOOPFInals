Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/Category.vb — Product grouping entity (7-table schema, safe
' SQL id). Encapsulation: the name is validated on every set.
' =====================================================================

Namespace Models

    Public Class Category

        Private _categoryId As Integer
        Private _name As String

        Public Sub New()
            _name = String.Empty
        End Sub

        Public Sub New(categoryId As Integer, name As String)
            Me.New()
            _categoryId = categoryId
            Me.Name = name
        End Sub

        Public Property CategoryId As Integer
            Get
                Return _categoryId
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("Category id cannot be negative.")
                End If
                _categoryId = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("Category name cannot be empty.")
                End If
                _name = value.Trim()
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return _name
        End Function

    End Class

End Namespace
