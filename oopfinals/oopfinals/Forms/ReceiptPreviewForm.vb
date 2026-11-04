Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Forms/ReceiptPreviewForm.vb (M10) — the MODAL post-checkout preview.
' The cashier sees exactly what the counter printer would produce
' (byte-identical GenerateReceiptText rendering), can reprint the
' paper copy, and closes back to the POS — which then clears the cart
' and refreshes stock + the Today list (sequence §12.3).
'
' OOP concept: ENCAPSULATION — the form renders and routes only; every
' line of receipt content comes from Services/ReceiptPrinter and the
' money figures it displays come from the validated Sale model. No
' math and no SQL live here (spec §3 layering).
'
' LAYERING: Forms → Services (ReceiptPrinter) → Models (Sale).
' =====================================================================

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports oopfinals.Controls
Imports oopfinals.Models
Imports oopfinals.Services

Namespace Forms

    Partial Public Class ReceiptPreviewForm

        Private ReadOnly _sale As Sale

        ' Optional constructor keeps the WinForms designer usable
        ' (runtime always passes a recorded Sale from the POS).
        Public Sub New(Optional sale As Sale = Nothing)
            MyBase.New()
            InitializeComponent()

            _sale = sale
            ApplyRestyle()

            If _sale IsNot Nothing Then
                LoadReceipt()
            End If
        End Sub

#Region "Data presentation"

        Private Sub LoadReceipt()
            If _sale Is Nothing Then
                Return
            End If

            Me.pageHeading.Text = "Receipt preview"
            Me.pageHeading.Subtitle = String.Format(
                "{0} · {1:yyyy-MM-dd HH:mm} · Cashier: {2}",
                _sale.ReceiptNo, _sale.SaleDate,
                If(String.IsNullOrWhiteSpace(_sale.CashierName), "Cashier", _sale.CashierName))

            ' One rendering source: the same generator the printer uses.
            Me.txtReceipt.Text = ReceiptPrinter.GenerateReceiptText(_sale)
            Me.txtReceipt.SelectionStart = 0

            Me.lblTotalsValue.Text = String.Format("Total PHP {0:N2}  ·  Change PHP {1:N2}",
                                                   _sale.Total, _sale.ChangeAmount)
        End Sub

#End Region

#Region "Buttons"

        ' Print → ShowPrintPreview: the standard System.Printing preview
        ' over the identical document (spec §12.3: RCV → PRT).
        Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
            If _sale Is Nothing Then
                MessageBox.Show(Me, "No receipt is loaded to print.",
                                "Nothing to print", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Try
                ReceiptPrinter.ShowPrintPreview(_sale, Me)
            Catch printerError As Exception
                MessageBox.Show(Me,
                                "Could not reach the printer for a preview." &
                                Environment.NewLine & printerError.Message,
                                "Printer unavailable", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ' Close returns to the POS, which then clears the cart (§12.3).
        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            Me.Close()
        End Sub

#End Region

#Region "Tokens only — no ad-hoc styling"

        Private Sub ApplyRestyle()
            Me.BackColor = Theme.PageBg
            Me.lblTotalsValue.ForeColor = Theme.Ink
            Me.lblTotalsValue.Font = Theme.FontFor(ThemeWeight.Semibold, 10.0F)
            Me.lblPrintHint.ForeColor = Theme.Muted
            Me.lblPrintHint.Font = Theme.FontFor(ThemeWeight.Regular, 8.0F)

            ' Monospace with a graceful fallback for every machine.
            Me.txtReceipt.Font = ReceiptTextFont()
            Me.txtReceipt.BackColor = Theme.CardBg
        End Sub

        Public Shared Function ReceiptTextFont() As Font
            Try
                Return New Font("Consolas", 10.0F)
            Catch fontError As Exception
                Return New Font(FontFamily.GenericMonospace, 10.0F)
            End Try
        End Function

#End Region

    End Class

End Namespace
