Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/Customer.vb — Concrete Customer of the User family (self-service
' portal: browse, purchase history, profile). AuthService.RegisterCustomer
' (M6) seeds identity; PurchaseHistory reads Sales.CustomerId from M10.
'
' SPEC §6.2 carve-out: this is the ONE deliberate Models→UI edge. No
' other Model class may construct a Form; User.vb stays UI-agnostic
' apart from the Form return type mandated by §6.1.
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

Namespace Models

    Public Class Customer
        Inherits User

        Public Sub New(userId As Integer,
                       username As String,
                       fullName As String,
                       isActive As Boolean)
            MyBase.New(userId, username, fullName, "Customer", isActive)
        End Sub

        Public Overrides Function GetRoleName() As String
            Return "Customer"
        End Function

        ' Fresh copy each call — a shared mutable List handed to callers
        ' would be a live privilege-escalation vector.
        Public Overrides Function GetPermissions() As List(Of String)
            Dim keys As String() = {
                "ViewProducts", "ViewPurchaseHistory", "ManageProfile"}
            Return New List(Of String)(keys)
        End Function

        ' §6.2 — the one deliberate Models→UI edge (fleshed out at M12).
        Public Overrides Function GetDashboardForm() As Form
            Return New Forms.CustomerDashboardForm(Me)
        End Function

    End Class

End Namespace
