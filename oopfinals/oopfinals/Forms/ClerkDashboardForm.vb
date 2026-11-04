Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Forms/ClerkDashboardForm.vb — Inventory Clerk dashboard SHELL (M4).
'
' Register-and-never-touch: the shell lands complete here and is only
' ever fleshed out at M11 (Alerts / Stock-In / Adjustment / Movements /
' Catalog).
'
' OOP concept (spec §6.2): the ctor binds its STRONGLY-TYPED user
' (InventoryClerk) into a Private ReadOnly field — safe UserId/FullName
' access without casts, no role branching anywhere.
' =====================================================================

Imports System
Imports System.Windows.Forms
Imports oopfinals.Controls
Imports oopfinals.Models

Namespace Forms

    Public Class ClerkDashboardForm

        Private ReadOnly _clerk As InventoryClerk

        ' Optional constructor keeps the WinForms designer usable (M11).
        Public Sub New(Optional clerk As InventoryClerk = Nothing)
            MyBase.New()
            InitializeComponent()

            StyledGrid.Apply(Me.permissionsGrid)
            Me.tabRail.AddTab("alerts", "Alerts", "alerts")
            Me.tabRail.SelectedKey = "alerts"

            _clerk = clerk
            ApplyUser()
            ApplyRestyle()
        End Sub

        Private Sub ApplyUser()
            If _clerk Is Nothing Then
                Return   ' designer-time shell — no live user bound
            End If

            Me.appHeader.Caption = _clerk.ToString()

            Dim keyCount As Integer = _clerk.GetPermissions().Count
            Me.pageHeading.Text = "Alerts"
            Me.pageHeading.Subtitle = keyCount.ToString() &
                " permissions granted — " & _clerk.GetRoleName() &
                " role (spec §6.2/§11)"

            FillPermissionsGrid()
        End Sub

        Private Sub FillPermissionsGrid()
            Me.permissionsGrid.Rows.Clear()
            For Each key As String In _clerk.GetPermissions()
                Me.permissionsGrid.Rows.Add(key)
            Next
        End Sub

        Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
            Me.Close()
        End Sub

#Region "Figma UI restyle (M3 tokens — §10.1)"

        ' Purely visual, token consumption only — no ad-hoc colors.
        Private Sub ApplyRestyle()
            Me.BackColor = Theme.PageBg
        End Sub

#End Region

    End Class

End Namespace
