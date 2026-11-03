Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Controls/StyledControls.vb — Restyled base widgets built on the
' Theme tokens: flat rounded StyledButton (hover/press per kind),
' rounded card panels, styled textboxes/combos, and status chips.
' No form should hard-code button colors — use these.
'
' OOP concept: INHERITANCE (overrides of Paint/hover behavior) and
' POLYMORPHISM (each button kind paints itself differently through the
' same render loop).
' =====================================================================

Imports System.Drawing
Imports System.Windows.Forms

Namespace Controls

    ' Flat, rounded button with hover/press feedback driven by ThemeButtonKind.
    Public Class StyledButton
        Inherits Button

        Private _kind As ThemeButtonKind = ThemeButtonKind.Primary
        Private _hover As Boolean
        Private _pressed As Boolean

        Public Sub New()
            MyBase.New()
            Me.FlatStyle = FlatStyle.Flat
            Me.FlatAppearance.BorderSize = 0
            Me.FlatAppearance.MouseOverBackColor = Color.Transparent
            Me.FlatAppearance.MouseDownBackColor = Color.Transparent
            Me.Cursor = Cursors.Hand
            Me.Font = Theme.FontFor(ThemeWeight.Semibold, 9.5F)
            Me.Height = 36
            Me.UseVisualStyleBackColor = False
        End Sub

        Public Property Kind As ThemeButtonKind
            Get
                Return _kind
            End Get
            Set(value As ThemeButtonKind)
                _kind = value
                Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnMouseEnter(e As EventArgs)
            MyBase.OnMouseEnter(e)
            _hover = True
            Invalidate()
        End Sub

        Protected Overrides Sub OnMouseLeave(e As EventArgs)
            MyBase.OnMouseLeave(e)
            _hover = False
            _pressed = False
            Invalidate()
        End Sub

        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
            MyBase.OnMouseDown(e)
            _pressed = True
            Invalidate()
        End Sub

        Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
            MyBase.OnMouseUp(e)
            _pressed = False
            Invalidate()
        End Sub

        Protected Overrides Sub OnPaint(pe As PaintEventArgs)
            Dim g As Graphics = pe.Graphics
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

            Dim bg As Color
            Dim fore As Color
            If _pressed Then
                Theme.GetButtonColors(_kind, False, bg, fore)
            ElseIf _hover Then
                Theme.GetButtonColors(_kind, False, bg, fore)
                If _kind = ThemeButtonKind.Secondary OrElse _kind = ThemeButtonKind.Ghost Then
                    ' Ghost hover is a tint; normal/secondary hover is the gray wash.
                End If
            Else
                Theme.GetButtonColors(_kind, True, bg, fore)
            End If

            If _pressed AndAlso (_kind = ThemeButtonKind.Primary OrElse _kind = ThemeButtonKind.Secondary) Then
                bg = If(_kind = ThemeButtonKind.Primary, Theme.GreenPress, Theme.SecondaryHover)
                fore = If(_kind = ThemeButtonKind.Primary, Color.White, Theme.Ink)
            End If

            Dim rect As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
            If _kind = ThemeButtonKind.Ghost AndAlso Not _hover Then
                Me.BackColor = Color.Transparent
            Else
                Me.BackColor = bg
            End If

            Using b As New SolidBrush(bg)
                Theme.FillRounded(g, b, rect, 9)
            End Using

            If _kind = ThemeButtonKind.Secondary OrElse _kind = ThemeButtonKind.Ghost Then
                Using p As New Pen(Theme.SecondaryBorder)
                    Theme.DrawRounded(g, p, rect, 9)
                End Using
            End If

            If Me.Image IsNot Nothing Then
                Dim iconSize As Integer = Math.Min(18, Me.Height - 10)
                Dim iconRect As New Rectangle(8, (Me.Height - iconSize) \ 2, iconSize, iconSize)
                g.DrawImage(Me.Image, iconRect)
            End If

            TextRenderer.DrawText(g, Me.Text, Me.Font,
                                  If(Me.Image IsNot Nothing, New Rectangle(30, 0, Me.Width - 32, Me.Height),
                                                        New Rectangle(0, 0, Me.Width, Me.Height)),
                                  fore, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        End Sub

    End Class

    ' White rounded card (radius 12) with a hairline border.
    Public Class RoundedPanel
        Inherits Panel

        Private _radius As Integer = 12

        Public Sub New()
            MyBase.New()
            Me.DoubleBuffered = True
            Me.BackColor = Theme.CardBg
        End Sub

        Public Property CornerRadius As Integer
            Get
                Return _radius
            End Get
            Set(value As Integer)
                _radius = Math.Max(0, value)
                Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            Dim g As Graphics = e.Graphics
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            Dim rect As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
            Using path As Drawing2D.GraphicsPath = Theme.RoundedPath(rect, _radius)
                Using b As New SolidBrush(Theme.CardBg)
                    g.FillPath(b, path)
                End Using
                Using p As New Pen(Theme.BorderLine)
                    g.DrawPath(p, path)
                End Using
            End Using
        End Sub

    End Class

    ' Rounded pill label for statuses (Low Stock / Verified / Needs review / ...).
    Public Class StyledChip
        Inherits Label

        Private _tone As ThemeTone = ThemeTone.Neutral

        Public Sub New()
            MyBase.New()
            Me.AutoSize = True
            Me.Padding = New Padding(8, 3, 8, 3)
            Me.Font = Theme.FontFor(ThemeWeight.Semibold, 8.0F)
        End Sub

        Public Property Tone As ThemeTone
            Get
                Return _tone
            End Get
            Set(value As ThemeTone)
                _tone = value
                Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            Dim g As Graphics = e.Graphics
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

            Dim fore As Color
            Dim back As Color
            Theme.GetToneColors(_tone, fore, back)

            Dim rect As New Rectangle(0, 0, Me.Width, Me.Height - 1)
            Using b As New SolidBrush(back)
                Theme.FillRounded(g, b, rect, (Me.Height) \ 2)
            End Using

            TextRenderer.DrawText(g, Me.Text, Me.Font, rect, fore,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End Sub

    End Class

    ' Shared input/combobox skinning for any stock control on a dashboard.
    Public NotInheritable Class StyledInputs

        Private Sub New()
        End Sub

        Public Shared Sub StyleTextBox(box As TextBox)
            box.BorderStyle = BorderStyle.FixedSingle
            box.Font = Theme.BodyFont()
        End Sub

        Public Shared Sub StyleComboBox(combo As ComboBox)
            combo.DropDownStyle = ComboBoxStyle.DropDownList
            combo.FlatStyle = FlatStyle.Flat
            combo.Font = Theme.BodyFont()
        End Sub

    End Class

End Namespace
