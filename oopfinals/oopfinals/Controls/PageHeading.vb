Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Controls/PageHeading.vb — In-page title + subtitle bar used at the
' top of every dashboard tab. Owner of all typography for page titles
' so no screen duplicates it.
' =====================================================================

Imports System.Drawing
Imports System.Windows.Forms

Namespace Controls

    Public Class PageHeading
        Inherits Control

        Private _subtitle As String = String.Empty

        Public Sub New()
            MyBase.New()
            Me.DoubleBuffered = True
            Me.Height = 56
            Me.Text = "Page"
        End Sub

        Public Property Subtitle As String
            Get
                Return _subtitle
            End Get
            Set(value As String)
                _subtitle = If(value, String.Empty)
                Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            Dim g As Graphics = e.Graphics
            Dim typedTitle As String = If(Me.Text, String.Empty)

            Using titleFont As Font = Theme.FontFor(ThemeWeight.Semibold, 15.0F)
                TextRenderer.DrawText(g, typedTitle, titleFont,
                                      New Rectangle(0, 0, Me.Width, 28), Theme.Ink,
                                      TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
            End Using

            Using subFont As Font = Theme.FontFor(ThemeWeight.Regular, 9.0F)
                TextRenderer.DrawText(g, _subtitle, subFont,
                                      New Rectangle(0, 28, Me.Width, 24), Theme.Muted,
                                      TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
            End Using
        End Sub

    End Class

End Namespace
