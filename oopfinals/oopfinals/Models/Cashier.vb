Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/Cashier.vb — Concrete Cashier of the User family (front-counter
' POS role). Same inheritance/polymorphism pattern as StoreOwner with
' its own 5-key permission set; POS content fleshed out M9–M10, M17.
'
' SPEC §6.2 carve-out: this is the ONE deliberate Models→UI edge. No
' other Model class may construct a Form; User.vb stays UI-agnostic
' apart from the Form return type mandated by §6.1.
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

Namespace Models

    Public Class Cashier
        Inherits User

        Public Sub New(userId As Integer,
                       username As String,
                       fullName As String,
                       isActive As Boolean)
            MyBase.New(userId, username, fullName, "Cashier", isActive)
        End Sub

        Public Overrides Function GetRoleName() As String
            Return "Cashier"
        End Function

        ' Fresh copy each call — a shared mutable List handed to callers
        ' would be a live privilege-escalation vector.
        Public Overrides Function GetPermissions() As List(Of String)
            Dim keys As String() = {
                "ProcessSale", "ViewTodayTransactions", "ApplyDiscounts",
                "PrintReceipt", "LookupProducts"}
            Return New List(Of String)(keys)
        End Function

        ' §6.2 — the one deliberate Models→UI edge (fleshed out M9–M10, M17).
        Public Overrides Function GetDashboardForm() As Form
            Return New Forms.CashierDashboardForm(Me)
        End Function

    End Class

End Namespace
