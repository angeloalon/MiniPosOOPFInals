Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Controls/IconTabBar.vb — Icon + label tab strip used by the four
' dashboards (vertical sidebar or horizontal strip) with an active pill.
' Selection raises SelectionChanged(key); SelectedKey is settable for
' stateful refresh flows.
'
' OOP concept: POLYMORPHISM — the tab buttons are one internal class;
' their paint behavior differs only through measured state (selected).
' =====================================================================

Imports System.Drawing
Imports System.Windows.Forms

Namespace Controls

    Public Enum IconTabOrientation
        Vertical
        Horizontal
    End Enum

    Public Class IconTabBar
        Inherits Panel

        ' One tab = one self-painting button.  Encapsulated in the bar.
        Private Class TabButton
            Inherits Button

            Public Key As String = String.Empty

            Public Sub New()
                MyBase.New()
                Me.FlatStyle = FlatStyle.Flat
                Me.FlatAppearance.BorderSize = 0
                Me.FlatAppearance.MouseOverBackColor = Color.FromArgb(43, 111, 78)
                Me.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 100, 70)
                Me.TextAlign = ContentAlignment.MiddleLeft
                Me.ImageAlign = ContentAlignment.MiddleLeft
                Me.TextImageRelation = TextImageRelation.ImageBeforeText
                Me.Cursor = Cursors.Hand
                Me.Font = Theme.FontFor(ThemeWeight.Semibold, 9.5F)
                Me.UseVisualStyleBackColor = False
            End Sub

            Protected Overrides Sub OnPaint(pe As PaintEventArgs)
            End Sub ' painting is handled entirely by the bar (pill highlights, text colors)

        End Class

        Private ReadOnly _tabs As New List(Of TabButton)()
        Private _orientation As IconTabOrientation = IconTabOrientation.Vertical
        Private _selectedKey As String = String.Empty
        Private _iconPixel As Integer = 20

        Public Event SelectionChanged(key As String)

        Public Sub New()
            MyBase.New()
            Me.DoubleBuffered = True
            Me.BackColor = Theme.HeaderGreen
            Me.Padding = New Padding(8)
        End Sub

        Public Property Orientation As IconTabOrientation
            Get
                Return _orientation
            End Get
            Set(value As IconTabOrientation)
                _orientation = value
                RecalculateLayout()
                Invalidate()
            End Set
        End Property

        Public Property SelectedKey As String
            Get
                Return _selectedKey
            End Get
            Set(value As String)
                SelectTab(value)
            End Set
        End Property

        Public Sub AddTab(key As String, text As String, Optional iconKey As String = "")
            Dim btn As New TabButton()
            btn.Key = key
            btn.Text = "   " & text
            Dim icon As String = If(iconKey <> String.Empty, iconKey, key)
            btn.Image = IconArt.GetIcon(icon, _iconPixel)
            AddHandler btn.Click, AddressOf OnTabClick
            _tabs.Add(btn)
            Me.Controls.Add(btn)
            RecalculateLayout()
        End Sub

        Public Sub SelectTab(key As String)
            If _selectedKey = key Then
                Return
            End If
            _selectedKey = key
            RefreshTabColors()
            RaiseEvent SelectionChanged(key)
        End Sub

        Public Sub RefreshTabColors()
            For Each tab As TabButton In _tabs
                If tab.Key = _selectedKey Then
                    tab.BackColor = Color.White
                    tab.ForeColor = Theme.HeaderGreen
                Else
                    tab.BackColor = Color.Transparent
                    tab.ForeColor = Theme.SidebarLow
                End If
                tab.Invalidate()
            Next
        End Sub

        Private Sub OnTabClick(sender As Object, e As EventArgs)
            Dim tab As TabButton = TryCast(sender, TabButton)
            If tab IsNot Nothing Then
                SelectTab(tab.Key)
            End If
        End Sub

        Protected Overrides Sub OnResize(e As EventArgs)
            MyBase.OnResize(e)
            RecalculateLayout()
        End Sub

        Private Sub RecalculateLayout()
            Dim pad As Integer = Me.Padding.Left
            Dim top As Integer = Me.Padding.Top
            If _orientation = IconTabOrientation.Vertical Then
                For Each tab As TabButton In _tabs
                    tab.SetBounds(pad, top, Me.ClientSize.Width - pad * 2, 44)
                    tab.TextAlign = ContentAlignment.MiddleLeft
                    top += 48
                Next
            Else
                For Each tab As TabButton In _tabs
                    Dim textWidth As Integer = TextRenderer.MeasureText(tab.Text, tab.Font).Width
                    tab.SetBounds(pad, top, textWidth + _iconPixel + 18, Me.ClientSize.Height - pad * 2)
                    pad += textWidth + _iconPixel + 18 + 4
                Next
            End If
        End Sub

        Protected Overrides Sub OnPaint(pe As PaintEventArgs)
            MyBase.OnPaint(pe)
            For Each tab As TabButton In _tabs
                Dim rect As New Rectangle(tab.Bounds.X + 2, tab.Bounds.Y + 2, tab.Width - 4, tab.Height - 4)
                Dim g As Graphics = pe.Graphics
                g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                Dim back As Color = If(tab.Key = _selectedKey, Color.White, Color.Transparent)
                Using b As New SolidBrush(back)
                    Theme.FillRounded(g, b, rect, 10)
                End Using
                tab.BackColor = back
                Dim fore As Color = If(tab.Key = _selectedKey, Theme.HeaderGreen, Theme.SidebarLow)
                TextRenderer.DrawText(pe.Graphics, tab.Text, tab.Font, tab.Bounds, fore,
                                      TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
                If tab.Image IsNot Nothing Then
                    pe.Graphics.DrawImage(tab.Image, tab.Bounds.X + 10, tab.Bounds.Y + (tab.Height - tab.Image.Height) \ 2)
                End If
            Next
        End Sub

    End Class

End Namespace
