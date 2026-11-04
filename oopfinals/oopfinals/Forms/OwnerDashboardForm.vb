Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Forms/OwnerDashboardForm.vb — Store Owner dashboard SHELL (M4).
'
' Register-and-never-touch: the shell lands complete here (AppHeader
' + user caption, IconTabBar rail, PageHeading, read-only permission
' grid, Logout) and is only ever fleshed out by later milestones
' (Overview/Master Data M13, Staff M14, Products/Transactions M15,
' Reports M16, Nutrition M17).
'
' OOP concept (spec §6.2): the ctor binds its STRONGLY-TYPED user
' (StoreOwner) into a Private ReadOnly field — this form can safely use
' UserId/FullName without casts and needs no role branching anywhere.
' =====================================================================

Imports System
Imports System.Windows.Forms
Imports oopfinals.Controls
Imports oopfinals.Models

Namespace Forms

    Public Class OwnerDashboardForm

        Private ReadOnly _owner As StoreOwner

        ' The Optional constructor keeps the WinForms designer usable on
        ' this shell (M13–M15 need the empty shell to design against);
        ' at run-time the launcher always supplies a live Store Owner.
        Public Sub New(Optional owner As StoreOwner = Nothing)
            MyBase.New()
            InitializeComponent()

            StyledGrid.Apply(Me.permissionsGrid)   ' the ONE uniform skin
            Me.tabRail.AddTab("overview", "Overview", "overview")
            Me.tabRail.SelectedKey = "overview"

            _owner = owner
            ApplyOwner()
            ApplyRestyle()
        End Sub

        Private Sub ApplyOwner()
            If _owner Is Nothing Then
                Return   ' designer-time shell — no live user bound
            End If

            ' Caption = ToString(): "Maria Santos (Store Owner) - @owner".
            Me.appHeader.Caption = _owner.ToString()

            Dim keyCount As Integer = _owner.GetPermissions().Count
            Me.pageHeading.Text = "Overview"
            Me.pageHeading.Subtitle = keyCount.ToString() &
                " permissions granted — " & _owner.GetRoleName() &
                " role (spec §6.2/§11)"

            FillPermissionsGrid()
        End Sub

        Private Sub FillPermissionsGrid()
            Me.permissionsGrid.Rows.Clear()
            For Each key As String In _owner.GetPermissions()
                Me.permissionsGrid.Rows.Add(key)
            Next
        End Sub

        Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
            Me.Close()   ' dialog closes → back to launcher (real logout M8)
        End Sub

#Region "Figma UI restyle (M3 tokens — §10.1)"

        ' Purely visual, token consumption only — no ad-hoc colors.
        Private Sub ApplyRestyle()
            Me.BackColor = Theme.PageBg
        End Sub

#End Region

    End Class

End Namespace
