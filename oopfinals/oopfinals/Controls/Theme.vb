Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Controls/Theme.vb — Design-token single source of truth (Figma restyle).
' Every form pulls colors/fonts/shapes from here; no form hard-codes
' colors. Utility class: NotInheritable + Private Sub New, Shared only.
'
' OOP concept: ENCAPSULATION (utility class, compile-time instantiation
' is an error) and ABSTRACTION (paint helpers hide GDI plumbing).
' =====================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Namespace Controls

    Public Enum ThemeWeight
        Regular
        Semibold
        Bold
    End Enum

    Public Enum ThemeButtonKind
        Primary
        Secondary
        Destructive
        Danger
        Ghost
    End Enum

    Public Enum ThemeTone
        Neutral
        Success
        Info
        Warn
        [Error]
    End Enum

    Public NotInheritable Class Theme

        Private Sub New()
        End Sub

#Region "Color tokens"

        Public Shared ReadOnly Property PageBg As Color
            Get
                Return Color.FromArgb(243, 245, 244)          ' #F3F5F4
            End Get
        End Property

        Public Shared ReadOnly Property CardBg As Color
            Get
                Return Color.White
            End Get
        End Property

        Public Shared ReadOnly Property HeaderGreen As Color
            Get
                Return Color.FromArgb(31, 92, 63)             ' #1F5C3F
            End Get
        End Property

        Public Shared ReadOnly Property Green As Color
            Get
                Return Color.FromArgb(46, 125, 79)            ' #2E7D4F
            End Get
        End Property

        Public Shared ReadOnly Property GreenHover As Color
            Get
                Return Color.FromArgb(38, 107, 65)            ' #266B41
            End Get
        End Property

        Public Shared ReadOnly Property GreenPress As Color
            Get
                Return Color.FromArgb(31, 92, 63)             ' #1F5C3F
            End Get
        End Property

        Public Shared ReadOnly Property GreenTint As Color
            Get
                Return Color.FromArgb(232, 243, 236)          ' #E8F3EC
            End Get
        End Property

        Public Shared ReadOnly Property Ink As Color
            Get
                Return Color.FromArgb(26, 31, 28)             ' #1A1F1C
            End Get
        End Property

        Public Shared ReadOnly Property Muted As Color
            Get
                Return Color.FromArgb(107, 114, 128)          ' #6B7280
            End Get
        End Property

        Public Shared ReadOnly Property BorderLine As Color
            Get
                Return Color.FromArgb(227, 232, 229)          ' #E3E8E5
            End Get
        End Property

        Public Shared ReadOnly Property SecondaryBg As Color
            Get
                Return Color.White
            End Get
        End Property

        Public Shared ReadOnly Property SecondaryBorder As Color
            Get
                Return Color.FromArgb(213, 219, 216)          ' #D5DBD8
            End Get
        End Property

        Public Shared ReadOnly Property SecondaryHover As Color
            Get
                Return Color.FromArgb(241, 244, 242)          ' #F1F4F2
            End Get
        End Property

        Public Shared ReadOnly Property Destructive As Color
            Get
                Return Color.FromArgb(179, 38, 30)            ' #B3261E
            End Get
        End Property

        Public Shared ReadOnly Property LogoutRed As Color
            Get
                Return Color.FromArgb(198, 40, 40)            ' #C62828
            End Get
        End Property

        Public Shared ReadOnly Property InfoBlue As Color
            Get
                Return Color.FromArgb(29, 78, 216)            ' #1D4ED8
            End Get
        End Property

        Public Shared ReadOnly Property InfoTint As Color
            Get
                Return Color.FromArgb(231, 239, 252)          ' #E7EFFC
            End Get
        End Property

        Public Shared ReadOnly Property WarnAmber As Color
            Get
                Return Color.FromArgb(180, 83, 9)             ' #B45309
            End Get
        End Property

        Public Shared ReadOnly Property WarnTint As Color
            Get
                Return Color.FromArgb(252, 243, 227)          ' #FCF3E3
            End Get
        End Property

        Public Shared ReadOnly Property ErrorRed As Color
            Get
                Return Color.FromArgb(179, 38, 30)            ' #B3261E
            End Get
        End Property

        Public Shared ReadOnly Property ErrorTint As Color
            Get
                Return Color.FromArgb(251, 234, 233)          ' #FBEAE9
            End Get
        End Property

        Public Shared ReadOnly Property SuccessGreen As Color
            Get
                Return Color.FromArgb(31, 122, 61)            ' #1F7A3D
            End Get
        End Property

        Public Shared ReadOnly Property SuccessTint As Color
            Get
                Return Color.FromArgb(232, 243, 236)          ' #E8F3EC
            End Get
        End Property

        Public Shared ReadOnly Property SidebarLow As Color
            Get
                Return Color.FromArgb(217, 229, 220)          ' light text on green sidebar
            End Get
        End Property

#End Region

#Region "Font tokens (Inter-style family with Segoe UI fallback)"

        Public Shared Function FontFor(weight As ThemeWeight, size As Single) As Font
            Try
                Select Case weight
                    Case ThemeWeight.Semibold
                        Return New Font("Segoe UI Semibold", size, FontStyle.Regular)
                    Case ThemeWeight.Bold
                        Return New Font("Segoe UI", size, FontStyle.Bold)
                    Case Else
                        Return New Font("Segoe UI", size, FontStyle.Regular)
                End Select
            Catch ex As Exception
                Return New Font(FontFamily.GenericSansSerif, size)
            End Try
        End Function

        Public Shared Function BodyFont() As Font
            Return FontFor(ThemeWeight.Regular, 9.0F)
        End Function

#End Region

#Region "Shared paint helpers"

        Public Shared Function RoundedPath(rect As Rectangle, radius As Integer) As GraphicsPath
            Dim path As New GraphicsPath()
            Dim d As Integer = Math.Min(Math.Min(rect.Width, rect.Height), radius * 2)
            If d <= 0 Then
                path.AddRectangle(rect)
                Return path
            End If
            path.AddArc(rect.X, rect.Y, d, d, 180, 90)
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90)
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90)
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90)
            path.CloseFigure()
            Return path
        End Function

        Public Shared Sub FillRounded(g As Graphics, brush As Brush, rect As Rectangle, radius As Integer)
            Using path As GraphicsPath = RoundedPath(rect, radius)
                g.FillPath(brush, path)
            End Using
        End Sub

        Public Shared Sub DrawRounded(g As Graphics, pen As Pen, rect As Rectangle, radius As Integer)
            Using path As GraphicsPath = RoundedPath(rect, radius)
                g.DrawPath(pen, path)
            End Using
        End Sub

        ' Tone → (fore, back) pair for status chips/badges.
        Public Shared Sub GetToneColors(tone As ThemeTone, ByRef fore As Color, ByRef back As Color)
            Select Case tone
                Case ThemeTone.Success
                    fore = SuccessGreen
                    back = SuccessTint
                Case ThemeTone.Info
                    fore = InfoBlue
                    back = InfoTint
                Case ThemeTone.Warn
                    fore = WarnAmber
                    back = WarnTint
                Case ThemeTone.[Error]
                    fore = ErrorRed
                    back = ErrorTint
                Case Else
                    fore = Muted
                    back = Color.FromArgb(238, 241, 240)
            End Select
        End Sub

        ' Sensible button palette per kind.
        Public Shared Sub GetButtonColors(kind As ThemeButtonKind, stateNormal As Boolean, ByRef bg As Color, ByRef fore As Color)
            Select Case kind
                Case ThemeButtonKind.Primary
                    If stateNormal Then
                        bg = Green
                    Else
                        bg = GreenHover
                    End If
                    fore = Color.White
                Case ThemeButtonKind.Secondary
                    If stateNormal Then
                        bg = SecondaryBg
                    Else
                        bg = SecondaryHover
                    End If
                    fore = Ink
                Case ThemeButtonKind.Destructive
                    If stateNormal Then
                        bg = Destructive
                    Else
                        bg = Color.FromArgb(155, 30, 23)
                    End If
                    fore = Color.White
                Case ThemeButtonKind.Danger
                    If stateNormal Then
                        bg = LogoutRed
                    Else
                        bg = Color.FromArgb(168, 33, 33)
                    End If
                    fore = Color.White
                Case Else ' Ghost
                    If stateNormal Then
                        bg = Color.Transparent
                    Else
                        bg = GreenTint
                    End If
                    fore = Green
            End Select
        End Sub

#End Region

    End Class

End Namespace
