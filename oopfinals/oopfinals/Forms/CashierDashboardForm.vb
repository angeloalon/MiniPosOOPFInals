Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Forms/CashierDashboardForm.vb — Cashier dashboard SHELL (M4).
'
' Register-and-never-touch: the shell lands complete here and is only
' ever fleshed out by later milestones (POS stage 1 M9, checkout +
' Today M10, nutrition card M17).
'
' OOP concept (spec §6.2): the ctor binds its STRONGLY-TYPED user
' (Cashier) into a Private ReadOnly field — safe UserId/FullName access
' without casts, no role branching anywhere.
' =====================================================================

Imports System
Imports System.Windows.Forms
Imports oopfinals.Controls
Imports oopfinals.Models

Namespace Forms

    Public Class CashierDashboardForm

        Private ReadOnly _cashier As Cashier

        ' Optional constructor keeps the WinForms designer usable (M9+).
        Public Sub New(Optional cashier As Cashier = Nothing)
            MyBase.New()
            InitializeComponent()

            StyledGrid.Apply(Me.permissionsGrid)
            Me.tabRail.AddTab("pos", "POS", "pos")
            Me.tabRail.SelectedKey = "pos"

            _cashier = cashier
            ApplyUser()
            ApplyRestyle()
        End Sub

        Private Sub ApplyUser()
            If _cashier Is Nothing Then
                Return   ' designer-time shell — no live user bound
            End If

            Me.appHeader.Caption = _cashier.ToString()

            Dim keyCount As Integer = _cashier.GetPermissions().Count
            Me.pageHeading.Text = "POS"
            Me.pageHeading.Subtitle = keyCount.ToString() &
                " permissions granted — " & _cashier.GetRoleName() &
                " role (spec §6.2/§11)"

            FillPermissionsGrid()
        End Sub

        Private Sub FillPermissionsGrid()
            Me.permissionsGrid.Rows.Clear()
            For Each key As String In _cashier.GetPermissions()
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
