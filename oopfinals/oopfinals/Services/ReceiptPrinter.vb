Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Services/ReceiptPrinter.vb — receipt TEXT rendering + the
' System.Drawing.Printing pipeline (spec §9.5). Pure service: it knows
' Sale/SaleItem and GDI printing, nothing about screens or SQL.
'
' The text is the document of record: 48 fixed-width columns rendered
' in a monospace face on paper or in the preview — RECEIPTS ARE
' DETERMINISTIC. Reprinting one receipt any number of times produces
' byte-identical text (no print timestamps, no volatile data), which
' is exactly what the M10 verify list demands.
'
' OOP concept: ABSTRACTION — GenerateReceiptText() is the one
' definition of "what a receipt says"; ShowPrintPreview() and
' PrintDirectly() are the two output routes over the SAME document,
' chosen by the caller (preview by default, direct for physical CDC
' receipt printers behind the counter).
'
' OOP concept: ENCAPSULATION — NotInheritable + Private Sub New;
' Shared members only; nothing but parameters in, no mutable state.
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Windows.Forms
Imports oopfinals.Models

Namespace Services

    Public NotInheritable Class ReceiptPrinter

        ' Static gate: no printer instances — the receipt is a document,
        ' never a stateful device session.
        Private Sub New()
        End Sub

#Region "Format constants (spec §9.5)"

        ' The spec's fixed-width pocket-printer column count.
        Public Const ReceiptWidth As Integer = 48

        ' Store identity header exactly as the spec quotes it.
        Private Shared ReadOnly StoreHeaderLines() As String = {
            "MINI-GROCERY STORE POS",
            "University of Caloocan City - CSD",
            "Biglang Awa St., Caloocan City",
            "(02) 8310-1234"
        }

        ' Item-table columns: NAME(-20) QTY(5) PRICE(10) TOTAL(10)
        ' = 45 characters + 3 single-space separators = 48 exactly.
        Public Const NameColumnWidth As Integer = 20
        Public Const QtyColumnWidth As Integer = 5
        Public Const PriceColumnWidth As Integer = 10
        Public Const TotalColumnWidth As Integer = 10

#End Region

#Region "Receipt text — the document of record"

        ' Throws ArgumentNullException for a Nothing sale — a receipt
        ' without a document is a programming error, not a user error.
        Public Shared Function GenerateReceiptText(sale As Sale) As String
            If sale Is Nothing Then
                Throw New ArgumentNullException("sale", "A sale is required to generate receipt text.")
            End If

            Dim lines As New List(Of String)()

            ' ---- Store header (centered) + rule -----------------------
            For Each headerLine As String In StoreHeaderLines
                lines.Add(Center(headerLine))
            Next
            lines.Add(Rule("="c))

            ' ---- Document identity ------------------------------------
            ' ISO stamp mirrors the schema's TEXT date format (spec §8);
            ' a local constant keeps Services → Models the ONLY lane this
            ' file travels (Services never references Data — spec §3).
            lines.Add(LabelledValue("Receipt No", sale.ReceiptNo))
            lines.Add(LabelledValue("Date/Time",
                                    sale.SaleDate.ToString("yyyy-MM-dd HH:mm:ss")))
            lines.Add(LabelledValue("Cashier",
                                    If(String.IsNullOrWhiteSpace(sale.CashierName), "Cashier", sale.CashierName)))
            If Not String.IsNullOrWhiteSpace(sale.CustomerName) Then
                lines.Add(LabelledValue("Customer", sale.CustomerName))
            End If

            ' ---- Item table -------------------------------------------
            lines.Add(Rule("-"c))
            lines.Add(String.Format("{0,-20} {1,5} {2,10} {3,10}",
                                    "NAME", "QTY", "PRICE", "TOTAL"))
            For Each line As SaleItem In sale.Items
                ' Frozen price at sale time — the SaleItem LineTotal is
                ' get-only, so the printed column cannot drift from what
                ' was stored (spec §8 "frozen price at sale time").
                lines.Add(String.Format("{0,-20} {1,5} {2,10} {3,10}",
                                        Truncate(FitName(line.ProductName), NameColumnWidth),
                                        line.Quantity,
                                        line.UnitPrice.ToString("N2"),
                                        line.LineTotal.ToString("N2")))
            Next

            ' ---- Summary block ----------------------------------------
            lines.Add(Rule("-"c))
            lines.Add(SummaryLine("Subtotal", "PHP " & sale.Subtotal.ToString("N2")))
            If sale.DiscountAmount > 0D Then
                ' Spec wording: "Discount (Senior Citizen / PWD): −PHP …"
                ' (ASCII hyphen — pocket printers are ASCII-only here).
                lines.Add(SummaryLine(String.Format("Discount ({0})", sale.DiscountType),
                                      "-PHP " & sale.DiscountAmount.ToString("N2")))
            End If
            lines.Add(Rule("-"c))
            lines.Add(SummaryLine("TOTAL DUE", "PHP " & sale.Total.ToString("N2")))
            lines.Add(SummaryLine("Cash Tendered", "PHP " & sale.CashReceived.ToString("N2")))
            lines.Add(SummaryLine("Change", "PHP " & sale.ChangeAmount.ToString("N2")))

            ' ---- Footer ------------------------------------------------
            lines.Add(Rule("="c))
            lines.Add(Center("Thank you for shopping with us!"))
            lines.Add(Center("SALES INVOICE / OFFICIAL RECEIPT"))

            Return String.Join(Environment.NewLine, lines.ToArray())
        End Function

#End Region

#Region "Text-shaped private helpers (all width-safe)"

        ' Full-width rule of the given character, exactly 48 columns.
        Private Shared Function Rule(fillChar As Char) As String
            Return New String(fillChar, ReceiptWidth)
        End Function

        ' NOTE: centered lines are padded, NOT truncated — every literal
        ' fed here is hand-sized < 48 so the column budget holds.
        Private Shared Function Center(text As String) As String
            Dim value As String = If(text, String.Empty).Trim()
            If value.Length >= ReceiptWidth Then
                Return value
            End If
            Dim left As Integer = (ReceiptWidth - value.Length) \ 2
            Return New String(" "c, left) & value
        End Function

        ' "Label : value" — label flush left, value flush at column 14.
        Private Shared Function LabelledValue(label As String, value As String) As String
            Return String.Format("{0,-13}: {1}", Truncate(label, 12), value)
        End Function

        ' Summary rows: label flush left, "PHP x.xx" flush right within
        ' the 48 columns (longest possible money string is 14 chars,
        ' capped by ValidationRules.MaxMoney = ₱999,999.99).
        Private Shared Function SummaryLine(label As String, value As String) As String
            Return String.Format("{0,-32}{1,16}", Truncate(label, 32), value)
        End Function

        ' Too-long product names keep their ENDING with an ellipsis so
        ' the fixed columns behind them never shift.
        Private Shared Function FitName(productName As String) As String
            Dim value As String = If(productName, String.Empty).Trim()
            If value.Length > NameColumnWidth Then
                Return value.Substring(0, NameColumnWidth - 3) & "..."
            End If
            Return value
        End Function

        Private Shared Function Truncate(value As String, maxWidth As Integer) As String
            Dim text As String = If(value, String.Empty)
            If text.Length > maxWidth Then
                Return text.Substring(0, maxWidth)
            End If
            Return text
        End Function

#End Region

#Region "Printing pipeline (System.Drawing.Printing)"

        ' ShowPrintPreview(sale, owner) — PrintDocument + the standard
        ' PrintPreviewDialog with the monospace receipt renderer. The
        ' preview mirrors the paper output line-for-line.
        Public Shared Sub ShowPrintPreview(sale As Sale, owner As IWin32Window)
            Using previewDialog As New PrintPreviewDialog()
                previewDialog.Document = BuildPrintDocument(sale)
                previewDialog.StartPosition = FormStartPosition.CenterParent
                previewDialog.WindowState = FormWindowState.Normal
                previewDialog.Width = 480
                previewDialog.Height = 720
                previewDialog.MinimizeBox = False
                previewDialog.MaximizeBox = True
                previewDialog.ShowDialog(owner)
            End Using
        End Sub

        ' PrintDirectly(sale) — straight to the DEFAULT printer, the
        ' physical-receipt path (spec §9.5). Exceptions propagate so the
        ' calling form can surface a printer-specific message.
        Public Shared Sub PrintDirectly(sale As Sale)
            Using directDocument As PrintDocument = BuildPrintDocument(sale)
                directDocument.Print()
            End Using
        End Sub

        ' The shared renderer: ONE PrintDocument per call, fed by the
        ' SAME GenerateReceiptText() used for storage/previews.
        Private Shared Function BuildPrintDocument(sale As Sale) As PrintDocument
            Dim receiptText As String = GenerateReceiptText(sale)

            ' Normalize the line stream once; the page handler walks it.
            Dim receiptLines As New List(Of String)()
            For Each rawLine As String In receiptText.Replace(vbCr, vbLf).Split(ControlChars.Lf)
                receiptLines.Add(rawLine)
            Next

            Dim document As New PrintDocument()
            document.DocumentName = sale.ReceiptNo
            document.DefaultPageSettings.Margins =
                New Margins(12, 12, 16, 16)   ' narrow receipt margins

            ' The redraw offset must restart every print job (the preview
            ' may print the same document twice in one session).
            Dim nextLineIndex As Integer = 0

            AddHandler document.BeginPrint,
                Sub(sender As Object, ePrint As PrintEventArgs)
                    nextLineIndex = 0
                End Sub

            AddHandler document.PrintPage,
                Sub(sender As Object, ePrint As PrintPageEventArgs)
                    Dim graphics As Graphics = ePrint.Graphics
                    graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    graphics.TextRenderingHint =
                        Drawing.Text.TextRenderingHint.ClearTypeGridFit

                    ' Monospace face — the 48-column layout is only
                    ' honest when every glyph occupies the same width.
                    Dim receiptFont As Font = MonospaceFont()
                    Dim lineHeight As Single = receiptFont.GetHeight(graphics) + 2.0F
                    Dim left As Single = CSng(ePrint.MarginBounds.Left)
                    Dim y As Single = CSng(ePrint.MarginBounds.Top)
                    Dim bottomLimit As Single = CSng(ePrint.MarginBounds.Bottom)

                    Do While nextLineIndex < receiptLines.Count
                        Dim line As String = receiptLines(nextLineIndex)
                        Dim needed As Single =
                            graphics.MeasureString(line, receiptFont).Height
                        If y + needed > bottomLimit Then
                            ' Pocket receipts fit one page; overflow still
                            ' paginates instead of silently clipping.
                            ePrint.HasMorePages = True
                            Return
                        End If
                        graphics.DrawString(line, receiptFont, Brushes.Black, left, y)
                        y += lineHeight
                        nextLineIndex += 1
                    Loop

                    ePrint.HasMorePages = False
                End Sub

            Return document
        End Function

        ' Consolas ships with Windows Vista and later; otherwise the
        ' always-present generic monospace family is the fallback.
        Private Shared Function MonospaceFont() As Font
            Try
                Return New Font("Consolas", 9.5F)
            Catch fontError As Exception
                Return New Font(FontFamily.GenericMonospace, 9.5F)
            End Try
        End Function

#End Region

    End Class

End Namespace
