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
            Me.DoubleBuffered = True
            Me.ApplyChrome()
        End Sub

        Public Property Kind As ThemeButtonKind
            Get
                Return _kind
            End Get
            Set(value As ThemeButtonKind)
                If _kind = value Then
                    Return
                End If
                _kind = value
                ApplyChrome()
                Invalidate()
            End Set
        End Property

        ' Resolves BackColor/ForeColor once per state change instead of
        ' inside OnPaint — the paint loop only reads, never mutates, its
        ' own properties. Disabled buttons no longer look clickable.
        Private Sub ApplyChrome()
            Dim bg As Color
            Dim fore As Color
            If Not Me.Enabled Then
                bg = Theme.SecondaryHover
                fore = Theme.Muted
            ElseIf _pressed Then
                Theme.GetButtonColors(_kind, False, bg, fore)
                If _kind = ThemeButtonKind.Primary Then
                    bg = Theme.GreenPress
                ElseIf _kind = ThemeButtonKind.Secondary Then
                    bg = Theme.SecondaryHover
                    fore = Theme.Ink
                End If
            ElseIf _hover Then
                Theme.GetButtonColors(_kind, False, bg, fore)
            Else
                Theme.GetButtonColors(_kind, True, bg, fore)
            End If
            Me.BackColor = bg
            Me.ForeColor = fore
        End Sub

        Protected Overrides Sub OnMouseEnter(e As EventArgs)
            MyBase.OnMouseEnter(e)
            _hover = True
            ApplyChrome()
            Invalidate()
        End Sub

        Protected Overrides Sub OnMouseLeave(e As EventArgs)
            MyBase.OnMouseLeave(e)
            _hover = False
            _pressed = False
            ApplyChrome()
            Invalidate()
        End Sub

        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
            MyBase.OnMouseDown(e)
            _pressed = True
            ApplyChrome()
            Invalidate()
        End Sub

        Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
            MyBase.OnMouseUp(e)
            _pressed = False
            ApplyChrome()
            Invalidate()
        End Sub

        Protected Overrides Sub OnEnabledChanged(e As EventArgs)
            MyBase.OnEnabledChanged(e)
            _hover = False
            _pressed = False
            ApplyChrome()
            Invalidate()
        End Sub

        ' Chrome (background/border/text colors) is resolved by ApplyChrome;
        ' painting just renders the current state.
        Protected Overrides Sub OnPaint(pe As PaintEventArgs)
            Dim g As Graphics = pe.Graphics
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

            Dim rect As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
            Using b As New SolidBrush(Me.BackColor)
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
                                  Me.ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

            ' Keyboard-focus cue: dashed inset ring, drawn last so it sits on top.
            If Me.Focused AndAlso Me.ShowFocusCues Then
                Dim ring As New Rectangle(2, 2, Me.Width - 5, Me.Height - 5)
                If ring.Width > 0 AndAlso ring.Height > 0 Then
                    Dim ringColor As Color = If(Me.Enabled AndAlso
                                                (_kind = ThemeButtonKind.Primary OrElse _kind = ThemeButtonKind.Destructive OrElse _kind = ThemeButtonKind.Danger),
                                                Color.White, Theme.Ink)
                    Using p As New Pen(ringColor)
                        p.DashStyle = Drawing2D.DashStyle.Dash
                        Theme.DrawRounded(g, p, ring, 9)
                    End Using
                End If
            End If
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

            Dim rect As New Rectangle(0, 0, Me.Width, Me.Height)
            Using b As New SolidBrush(back)
                Theme.FillRounded(g, b, rect, Me.Height \ 2)
            End Using

            TextRenderer.DrawText(g, Me.Text, Me.Font, rect, fore,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End Sub

    End Class

    ' Shared input/combobox skinning for any stock control on a dashboard.
    ' The focus ring is part of the M3 deliverable: a styled input tints
    ' its background while it holds keyboard focus and restores the card
    ' color on leave. A Tag marker guards against double-wiring when a
    ' form styles the same input twice.
    Public NotInheritable Class StyledInputs

        Private Sub New()
        End Sub

        Public Shared Sub StyleTextBox(box As TextBox)
            If box Is Nothing Then
                Throw New ArgumentNullException("box", "StyleTextBox requires a text box.")
            End If
            box.BorderStyle = BorderStyle.FixedSingle
            box.Font = Theme.BodyFont()
            box.BackColor = Theme.CardBg
            AttachFocusRing(box)
        End Sub

        Public Shared Sub StyleComboBox(combo As ComboBox)
            If combo Is Nothing Then
                Throw New ArgumentNullException("combo", "StyleComboBox requires a combo box.")
            End If
            combo.DropDownStyle = ComboBoxStyle.DropDownList
            combo.FlatStyle = FlatStyle.Flat
            combo.Font = Theme.BodyFont()
            combo.BackColor = Theme.CardBg
            AttachFocusRing(combo)
        End Sub

        Private Shared Sub AttachFocusRing(input As Control)
            If TypeOf input.Tag Is FocusRingMarker Then
                Return
            End If
            input.Tag = New FocusRingMarker()
            AddHandler input.GotFocus, AddressOf OnFocusGained
            AddHandler input.LostFocus, AddressOf OnFocusLost
        End Sub

        Private Shared Sub OnFocusGained(sender As Object, e As EventArgs)
            Dim input As Control = TryCast(sender, Control)
            If input IsNot Nothing Then
                input.BackColor = Theme.InfoTint
            End If
        End Sub

        Private Shared Sub OnFocusLost(sender As Object, e As EventArgs)
            Dim input As Control = TryCast(sender, Control)
            If input IsNot Nothing Then
                input.BackColor = Theme.CardBg
            End If
        End Sub

        Private Class FocusRingMarker
        End Class

    End Class

End Namespace
