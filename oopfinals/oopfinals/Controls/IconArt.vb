Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Controls/IconArt.vb — GDI-drawn bitmap icon library. A complete glyph
' set with zero icon-font dependencies; unknown keys degrade gracefully
' to a letter badge so no screen ever ships a broken image.
'
' OOP concept: ENCAPSULATION — private glyph painters, one public cache
' entry point (NotInheritable + Private Sub New, Shared only).
' =====================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace Controls

    Public NotInheritable Class IconArt

        Private Sub New()
        End Sub

        Public Shared Function GetIcon(keyName As String, Optional size As Integer = 24) As Bitmap
            Dim bmp As New Bitmap(size, size)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.SmoothingMode = SmoothingMode.AntiAlias
                Dim fore As Color = Color.White
                Using pen As New Pen(fore, Math.Max(1.6F, size / 16.0F))
                    pen.StartCap = LineCap.Round
                    pen.EndCap = LineCap.Round
                    PaintGlyph(g, pen, If(keyName, String.Empty).Trim().ToLowerInvariant(), size)
                End Using
            End Using
            Return bmp
        End Function

        Private Shared Sub PaintGlyph(g As Graphics, pen As Pen, key As String, size As Integer)
            Dim s As Single = size / 24.0F
            Dim m As Single = 3.0F * s
            Dim w As Single = size - m * 2.0F

            Select Case key
                Case "overview"
                    g.DrawRectangle(pen, m, m, w, w)
                    g.DrawLine(pen, m, m + w * 0.5F, m + w, m + w * 0.5F)
                    g.DrawLine(pen, m + w * 0.5F, m, m + w * 0.5F, m + w)
                Case "staff"
                    g.DrawEllipse(pen, m, m, w * 0.62F, w * 0.45F)
                    g.DrawArc(pen, m - w * 0.12F, m + w * 0.46F, w * 0.85F, w * 0.62F, 180, 180)
                Case "products"
                    g.DrawLine(pen, m, m + w * 0.25F, m + w * 0.5F, m)
                    g.DrawLine(pen, m + w * 0.5F, m, m + w, m + w * 0.25F)
                    g.DrawLine(pen, m, m + w * 0.25F, m, m + w * 0.75F)
                    g.DrawLine(pen, m + w, m + w * 0.25F, m + w, m + w * 0.75F)
                    g.DrawRectangle(pen, CSng(m), CSng(m + w * 0.25F), CSng(w), CSng(w * 0.75F))
                    g.DrawLine(pen, m, m + w * 0.75F, m + w * 0.5F, m + w)
                    g.DrawLine(pen, m + w * 0.5F, m + w, m + w, m + w * 0.75F)
                Case "masterdata"
                    g.DrawLine(pen, m, m + w * 0.22F, m + w, m + w * 0.22F)
                    g.DrawLine(pen, m, m + w * 0.5F, m + w, m + w * 0.5F)
                    g.DrawLine(pen, m, m + w * 0.78F, m + w, m + w * 0.78F)
                    g.DrawEllipse(pen, m + w * 0.55F, m - 1.0F * s, w * 0.4F, w * 0.4F)
                Case "transactions"
                    g.DrawRectangle(pen, m, m, w, w)
                    g.DrawLine(pen, m + w * 0.3F, m + w * 0.35F, m + w * 0.7F, m + w * 0.65F)
                    g.DrawLine(pen, m + w * 0.7F, m + w * 0.35F, m + w * 0.7F, m + w * 0.65F)
                    g.DrawLine(pen, m + w * 0.7F, m + w * 0.65F, m + w * 0.3F, m + w * 0.65F)
                Case "reports"
                    g.DrawRectangle(pen, m, m, w, w)
                    g.DrawLine(pen, m + w * 0.25F, m + w * 0.75F, m + w * 0.25F, m + w * 0.55F)
                    g.DrawLine(pen, m + w * 0.5F, m + w * 0.75F, m + w * 0.5F, m + w * 0.35F)
                    g.DrawLine(pen, m + w * 0.75F, m + w * 0.75F, m + w * 0.75F, m + w * 0.45F)
                Case "nutrition"
                    g.DrawEllipse(pen, m + w * 0.06F, m, w * 0.88F, w)
                    g.DrawLine(pen, m + w * 0.5F, m + w * 0.28F, m + w * 0.5F, m + w * 0.78F)
                Case "pos"
                    g.DrawRectangle(pen, m, m + w * 0.2F, w, w * 0.72F)
                    g.DrawRectangle(pen, m + w * 0.12F, m, w * 0.76F, w * 0.2F)
                    g.DrawLine(pen, m + w * 0.3F, m + w * 0.55F, m + w * 0.7F, m + w * 0.55F)
                Case "today"
                    g.DrawRectangle(pen, m, m, w, w)
                    g.DrawLine(pen, m, m + w * 0.3F, m + w, m + w * 0.3F)
                    g.DrawLine(pen, m + w * 0.5F, m + w * 0.5F, m + w * 0.5F, m + w * 0.68F)
                Case "alerts"
                    g.DrawArc(pen, m + w * 0.1F, m + w * 0.14F, w * 0.8F, w * 0.8F, 180, 180)
                    g.DrawLine(pen, m + w * 0.1F, m + w * 0.54F, m + w * 0.9F, m + w * 0.54F)
                    g.DrawLine(pen, m + w * 0.44F, m + w * 0.78F, m + w * 0.56F, m + w * 0.78F)
                Case "stockin"
                    Dim boxY As Single = m + w * 0.45F
                    g.DrawRectangle(pen, m, boxY, w, size - boxY - m)
                    g.DrawLine(pen, m + w * 0.5F, m, m + w * 0.5F, boxY + (size - boxY - m) * 0.4F)
                    g.DrawLine(pen, m + w * 0.32F, boxY + (size - boxY - m) * 0.2F, m + w * 0.5F, boxY + (size - boxY - m) * 0.4F)
                    g.DrawLine(pen, m + w * 0.68F, boxY + (size - boxY - m) * 0.2F, m + w * 0.5F, boxY + (size - boxY - m) * 0.4F)
                Case "adjustment"
                    g.DrawLine(pen, m + w * 0.25F, m + w * 0.3F, m + w * 0.25F, m + w * 0.75F)
                    g.DrawLine(pen, m + w * 0.16F, m + w * 0.42F, m + w * 0.25F, m + w * 0.3F)
                    g.DrawLine(pen, m + w * 0.34F, m + w * 0.42F, m + w * 0.25F, m + w * 0.3F)
                    g.DrawLine(pen, m + w * 0.75F, m + w * 0.7F, m + w * 0.75F, m + w * 0.25F)
                    g.DrawLine(pen, m + w * 0.66F, m + w * 0.58F, m + w * 0.75F, m + w * 0.7F)
                    g.DrawLine(pen, m + w * 0.84F, m + w * 0.58F, m + w * 0.75F, m + w * 0.7F)
                Case "movements"
                    g.DrawLine(pen, m + w * 0.3F, m, m + w * 0.3F, m + w)
                    g.DrawLine(pen, m + w * 0.62F, m, m + w * 0.62F, m + w)
                    Dim arr As Single() = {m + w * 0.12F, m + w * 0.3F, m + w * 0.48F, m + w * 0.72F}
                    For Each x As Single In arr
                        g.DrawLine(pen, x - 1.6F * s, m + w * 0.38F, x, m + w * 0.5F)
                        g.DrawLine(pen, x, m + w * 0.5F, x - 1.6F * s, m + w * 0.62F)
                    Next
                Case "catalog"
                    g.DrawRectangle(pen, m, m, w, w * 0.34F)
                    g.DrawRectangle(pen, m, m + w * 0.52F, w, w * 0.34F)
                Case "browse"
                    g.DrawEllipse(pen, m, m, w * 0.66F, w * 0.66F)
                    g.DrawLine(pen, m + w * 0.56F, m + w * 0.56F, m + w, m + w)
                Case "history"
                    g.DrawEllipse(pen, m, m, w, w)
                    g.DrawLine(pen, m + w * 0.5F, m + w * 0.25F, m + w * 0.5F, m + w * 0.5F)
                    g.DrawLine(pen, m + w * 0.5F, m + w * 0.5F, m + w * 0.68F, m + w * 0.6F)
                Case "profile"
                    g.DrawEllipse(pen, m, m, w, w)
                    g.DrawEllipse(pen, m + w * 0.3F, m + w * 0.16F, w * 0.4F, w * 0.34F)
                    g.DrawArc(pen, m + w * 0.2F, m + w * 0.48F, w * 0.6F, w * 0.55F, 180, 180)
                Case "logout"
                    g.DrawArc(pen, m + w * 0.15F, m, w * 0.7F, w, 90, 180)
                    g.DrawLine(pen, m + w * 0.15F, m, m + w * 0.15F, m + w)
                    g.DrawLine(pen, m, m + w * 0.5F, m + w * 0.62F, m + w * 0.5F)
                    g.DrawLine(pen, m + w * 0.42F, m + w * 0.3F, m + w * 0.62F, m + w * 0.5F)
                    g.DrawLine(pen, m + w * 0.42F, m + w * 0.7F, m + w * 0.62F, m + w * 0.5F)
                Case "refresh"
                    g.DrawArc(pen, m, m, w, w, -80, 300)
                    g.DrawLine(pen, m + w * 0.86F, m + w * 0.22F, m + w * 0.86F, m + w * 0.48F)
                    g.DrawLine(pen, m + w * 0.86F, m + w * 0.22F, m + w * 0.6F, m + w * 0.22F)
                Case "print"
                    g.DrawRectangle(pen, m + w * 0.12F, m, w * 0.76F, w * 0.34F)
                    g.DrawRectangle(pen, m + w * 0.12F, m + w * 0.56F, w * 0.76F, w * 0.34F)
                    g.DrawRectangle(pen, m, m + w * 0.28F, w, w * 0.4F)
                Case "export"
                    g.DrawRectangle(pen, m, m, w, w)
                    g.DrawLine(pen, m + w * 0.5F, m + w * 0.15F, m + w * 0.5F, m + w * 0.7F)
                    g.DrawLine(pen, m + w * 0.36F, m + w * 0.56F, m + w * 0.5F, m + w * 0.7F)
                    g.DrawLine(pen, m + w * 0.64F, m + w * 0.56F, m + w * 0.5F, m + w * 0.7F)
                Case "search"
                    g.DrawEllipse(pen, m, m, w * 0.7F, w * 0.7F)
                    g.DrawLine(pen, m + w * 0.6F, m + w * 0.6F, m + w, m + w)
                Case "add"
                    g.DrawEllipse(pen, m, m, w, w)
                    g.DrawLine(pen, m + w * 0.5F, m + w * 0.28F, m + w * 0.5F, m + w * 0.72F)
                    g.DrawLine(pen, m + w * 0.28F, m + w * 0.5F, m + w * 0.72F, m + w * 0.5F)
                Case "remove"
                    g.DrawEllipse(pen, m, m, w, w)
                    g.DrawLine(pen, m + w * 0.28F, m + w * 0.5F, m + w * 0.72F, m + w * 0.5F)
                Case Else
                    ' Graceful fallback: rounded badge with the key's initial.
                    FallBackLetter(g, pen, If(key, String.Empty), m, w, size)
            End Select
        End Sub

        Private Shared Sub FallBackLetter(g As Graphics, pen As Pen, key As String, m As Single, w As Single, size As Integer)
            g.DrawRectangle(pen, m, m, w, w)
            Dim letter As String = If(key.Length > 0, key.Substring(0, 1).ToUpperInvariant(), "?")
            Using fnt As Font = New Font("Segoe UI", size * 0.42F, FontStyle.Bold)
                Using sf As New StringFormat()
                    sf.Alignment = StringAlignment.Center
                    sf.LineAlignment = StringAlignment.Center
                    g.DrawString(letter, fnt, New SolidBrush(pen.Color),
                                 New RectangleF(m, m, w, w), sf)
                End Using
            End Using
        End Sub

    End Class

End Namespace
