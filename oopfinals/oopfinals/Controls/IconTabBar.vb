Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Controls/IconTabBar.vb — Icon + label tab strip for the four
' dashboards (vertical sidebar or horizontal strip). Each tab is a
' self-painting child button: child windows always cover the bar's own
' surface, so the strip itself can never paint through its children —
' each tab draws its own active pill, hover tint, icon, and label.
' Selection raises SelectionChanged(key); SelectedKey is settable for
' stateful refresh flows.
'
' OOP concept: POLYMORPHISM — the tab buttons are one class whose paint
' behavior differs only through measured state (selected/hovered);
' INHERITANCE — TabButton extends Button, the bar extends Panel.
' =====================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Namespace Controls

    Public Enum IconTabOrientation
        Vertical
        Horizontal
    End Enum

    Public Class IconTabBar
        Inherits Panel

        ' One tab = one self-painting button. Encapsulated in the bar.
        Private Class TabButton
            Inherits Button

            Public Key As String = String.Empty
            Public IconKey As String = String.Empty
            Public IconPixel As Integer = 20

            Private ReadOnly _bar As IconTabBar
            Private _hover As Boolean
            Private _selectedImage As Bitmap

            Public Sub New(bar As IconTabBar)
                MyBase.New()
                _bar = bar
                Me.FlatStyle = FlatStyle.Flat
                Me.FlatAppearance.BorderSize = 0
                Me.DoubleBuffered = True
                Me.Cursor = Cursors.Hand
                Me.Font = Theme.FontFor(ThemeWeight.Semibold, 9.5F)
                Me.UseVisualStyleBackColor = False
                Me.BackColor = Color.Transparent
                Me.ForeColor = Theme.SidebarLow
            End Sub

            ' Build the selected glyph eagerly, outside the paint handler:
            ' IconArt.GetIcon is too expensive to call from inside OnPaint.
            Protected Overrides Sub OnHandleCreated(e As EventArgs)
                MyBase.OnHandleCreated(e)
                If IconKey <> String.Empty AndAlso _selectedImage Is Nothing Then
                    _selectedImage = IconArt.GetIcon(IconKey, IconPixel, Theme.HeaderGreen)
                End If
            End Sub

            ' The resting state draws the light glyph (Me.Image); the
            ' active state draws the ink glyph so it survives the white pill.
            Public ReadOnly Property SelectedImage As Image
                Get
                    If _selectedImage Is Nothing AndAlso IconKey <> String.Empty Then
                        _selectedImage = IconArt.GetIcon(IconKey, IconPixel, Theme.HeaderGreen)
                    End If
                    Return _selectedImage
                End Get
            End Property

            Protected Overrides Sub OnMouseEnter(e As EventArgs)
                MyBase.OnMouseEnter(e)
                _hover = True
                Invalidate()
            End Sub

            Protected Overrides Sub OnMouseLeave(e As EventArgs)
                MyBase.OnMouseLeave(e)
                _hover = False
                Invalidate()
            End Sub

            Protected Overrides Sub OnPaint(pe As PaintEventArgs)
                Dim g As Graphics = pe.Graphics
                g.SmoothingMode = SmoothingMode.AntiAlias

                Dim selected As Boolean = _bar IsNot Nothing AndAlso _bar.SelectedKey = Key
                Dim pill As New Rectangle(2, 2, Math.Max(0, Me.Width - 4), Math.Max(0, Me.Height - 4))

                If selected Then
                    Using b As New SolidBrush(Color.White)
                        Theme.FillRounded(g, b, pill, 10)
                    End Using
                ElseIf _hover Then
                    Using b As New SolidBrush(Theme.GreenHover)
                        Theme.FillRounded(g, b, pill, 10)
                    End Using
                End If

                Dim icon As Image = If(selected, SelectedImage, Me.Image)
                If icon IsNot Nothing Then
                    g.DrawImage(icon, 10, (Me.Height - icon.Height) \ 2)
                End If

                Dim fore As Color = If(selected, Theme.HeaderGreen, Theme.SidebarLow)
                Dim textX As Integer = 10 + IconPixel + 6
                Dim textRect As New Rectangle(textX, 0, Math.Max(0, Me.Width - textX - 6), Me.Height)
                TextRenderer.DrawText(g, Me.Text, Me.Font, textRect, fore,
                                      TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

                ' Keyboard-focus cue: dashed inset ring, drawn last so it sits on top.
                If Me.Focused AndAlso Me.ShowFocusCues Then
                    Dim ring As New Rectangle(2, 2, Me.Width - 5, Me.Height - 5)
                    If ring.Width > 0 AndAlso ring.Height > 0 Then
                        Using p As New Pen(If(selected, Theme.HeaderGreen, Color.White))
                            p.DashStyle = DashStyle.Dash
                            Theme.DrawRounded(g, p, ring, 10)
                        End Using
                    End If
                End If
            End Sub

            Protected Overrides Sub OnHandleDestroyed(e As EventArgs)
                If _selectedImage IsNot Nothing Then
                    _selectedImage.Dispose()
                    _selectedImage = Nothing
                End If
                MyBase.OnHandleDestroyed(e)
            End Sub

            ' WinForms does not own Button.Image; dispose the resting glyph.
            Protected Overrides Sub Dispose(disposing As Boolean)
                If disposing AndAlso Me.Image IsNot Nothing Then
                    Dim restIcon As Image = Me.Image
                    Me.Image = Nothing          ' clear first, so no repaint can see a disposed bitmap
                    restIcon.Dispose()
                End If
                MyBase.Dispose(disposing)
            End Sub

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
            Dim btn As New TabButton(Me)
            btn.Key = key
            btn.Text = If(text, String.Empty)
            btn.IconKey = If(iconKey <> String.Empty, iconKey, If(key, String.Empty))
            btn.IconPixel = _iconPixel
            btn.Image = IconArt.GetIcon(btn.IconKey, _iconPixel)   ' resting (light) glyph
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

        ' Tabs own their own chrome now; refreshing the pipeline only needs
        ' the children to repaint against the bar's current SelectedKey.
        Public Sub RefreshTabColors()
            For Each tab As TabButton In _tabs
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
                    tab.SetBounds(pad, top, Math.Max(0, Me.ClientSize.Width - pad * 2), 44)
                    top += 48
                Next
            Else
                For Each tab As TabButton In _tabs
                    Dim textWidth As Integer = TextRenderer.MeasureText(tab.Text, tab.Font).Width
                    Dim tabWidth As Integer = textWidth + tab.IconPixel + 24
                    tab.SetBounds(pad, top, tabWidth, Math.Max(0, Me.ClientSize.Height - pad * 2))
                    pad += tabWidth + 4
                Next
            End If
        End Sub

    End Class

End Namespace
