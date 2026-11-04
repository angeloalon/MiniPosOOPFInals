Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/StoreOwner.vb — Concrete Store Owner of the User family.
'
' OOP concept: INHERITANCE — chains the protected base constructors
' (MyBase.New) and stamps its persistence Role key. POLYMORPHISM — each
' MustOverride member resolves this subclass's identity, permissions,
' and dashboard at run-time (the StoreOwner's 14 keys are §11 columns).
'
' SPEC §6.2 carve-out: this is the ONE deliberate Models→UI edge. No
' other Model class may construct a Form; User.vb stays UI-agnostic
' apart from the Form return type mandated by §6.1.
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

Namespace Models

    Public Class StoreOwner
        Inherits User

        ' Role per spec §8 persistence set; display naming in GetRoleName().
        Private Const RoleKey As String = "StoreOwner"
        Private Const DisplayRoleName As String = "Store Owner"

        ' 14 permission keys — exact strings from spec §6.2/§11.
        Private Shared ReadOnly _permissionKeys As String() = {
            "ManageStaff", "ManageProducts", "ManagePrices", "ManageCategories",
            "ManageSuppliers", "ViewAllTransactions", "ViewReports", "ExportReports",
            "PrintReports", "StockIn", "StockAdjustment", "ViewInventoryAlerts",
            "ViewStockMovements", "ProcessSale"}

        Public Sub New(userId As Integer,
                       username As String,
                       fullName As String,
                       isActive As Boolean)
            MyBase.New(userId, username, fullName, RoleKey, isActive)
        End Sub

        Public Overrides Function GetRoleName() As String
            Return DisplayRoleName
        End Function

        ' Fresh copy each call — a shared mutable List handed to callers
        ' would be a live privilege-escalation vector
        ' (user.GetPermissions().Add("ProcessSale")).
        Public Overrides Function GetPermissions() As List(Of String)
            Return New List(Of String)(_permissionKeys)
        End Function

        ' §6.2 — the one deliberate Models→UI edge (fleshed out M13–M15).
        Public Overrides Function GetDashboardForm() As Form
            Return New Forms.OwnerDashboardForm(Me)
        End Function

    End Class

End Namespace
