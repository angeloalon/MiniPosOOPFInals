Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Controls/AppHeader.vb — Shared green page header: store title on the
' left, current-user caption on the right. Every dashboard hosts one so
' identity is always visible (logout lives as a child button on the form).
' =====================================================================

Imports System.Drawing
Imports System.Windows.Forms

Namespace Controls

    Public Class AppHeader
        Inherits Panel

        Private _caption As String = String.Empty

        Public Sub New()
            MyBase.New()
            Me.DoubleBuffered = True
            Me.Dock = DockStyle.Top
            Me.Height = 64
            Me.BackColor = Theme.HeaderGreen
        End Sub

        ' Who is logged in, e.g. "Maria Santos (Store Owner) - @owner".
        Public Property Caption As String
            Get
                Return _caption
            End Get
            Set(value As String)
                _caption = If(value, String.Empty)
                Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            Dim g As Graphics = e.Graphics

            Using titleFont As Font = Theme.FontFor(ThemeWeight.Bold, 12.5F)
                TextRenderer.DrawText(g, "MINI-GROCERY STORE POS", titleFont,
                                      New Rectangle(16, 10, Math.Max(0, Me.ClientSize.Width - 200), 26), Color.White,
                                      TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
            End Using

            Using capFont As Font = Theme.FontFor(ThemeWeight.Regular, 8.5F)
                TextRenderer.DrawText(g, _caption, capFont,
                                      New Rectangle(16, 34, Me.ClientSize.Width - 32, 22), Theme.SidebarLow,
                                      TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
            End Using

            Using p As New Pen(Theme.GreenPress)
                g.DrawLine(p, 0, Me.Height - 1, Me.Width, Me.Height - 1)
            End Using
        End Sub

    End Class

End Namespace
