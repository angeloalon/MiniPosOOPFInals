Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Forms/PastReceiptForm.vb (M10) — the historical receipt dialog. It is
' deliberately role-blind: ANY dashboard hands it a recorded Sale and
' it renders the frozen document —
'   • Cashier  → "Today" tab, View details (M10),
'   • Owner    → "Transactions" browser, View details (M15),
'   • Customer → History, View receipt details (M12).
'
' One definition of "past receipt" = zero dialog drift between roles.
'
' OOP concept: ABSTRACTION / POLYMORPHISM-free by construction — the
' dialog consumes the Sale/SaleItem MODELS only; who opened it and
' from which screen is irrelevant to what it shows (total processing
' path identical for all callers, including reprint).
'
' OOP concept: ENCAPSULATION — SQL lives in SaleRepository (this form
' hydrates its line items through GetSaleItems), receipt text in
' ReceiptPrinter; this file holds navigation glue only (spec §3).
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports oopfinals.Controls
Imports oopfinals.Data
Imports oopfinals.Models
Imports oopfinals.Services

Namespace Forms

    Partial Public Class PastReceiptForm

        Private ReadOnly _sale As Sale
        Private ReadOnly _lineItems As List(Of SaleItem)

        ' Optional constructor keeps the WinForms designer usable
        ' (runtime always passes a Sale straight from a repository read).
        Public Sub New(Optional sale As Sale = Nothing)
            MyBase.New()
            InitializeComponent()

            _sale = sale
            _lineItems = New List(Of SaleItem)()

            StyledGrid.Apply(Me.detailsGrid)

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

            Me.pageHeading.Text = "Past receipt"
            Me.pageHeading.Subtitle = _sale.ReceiptNo

            ' Header strip — the document identity block.
            Me.lblReceiptValue.Text = _sale.ReceiptNo
            Me.lblDateTimeValue.Text = _sale.SaleDate.ToString(DatabaseHelper.IsoDateTimeFormat)
            Me.lblCashierValue.Text = If(String.IsNullOrWhiteSpace(_sale.CashierName), "Cashier", _sale.CashierName)
            Me.lblCustomerValue.Text = If(String.IsNullOrWhiteSpace(_sale.CustomerName), "Walk-In", _sale.CustomerName)

            ' Totals strip — the Discount caption swells to include the
            ' strategy name when a discount actually applied.
            If _sale.DiscountAmount > 0D Then
                Me.lblDiscountCaption.Text = String.Format("Discount ({0})", _sale.DiscountType)
                Me.lblDiscountValue.Text = String.Format("- PHP {0:N2}", _sale.DiscountAmount)
                Me.lblDiscountValue.ForeColor = Theme.Green
            Else
                Me.lblDiscountCaption.Text = "Discount"
                Me.lblDiscountValue.Text = "PHP 0.00"
                Me.lblDiscountValue.ForeColor = Theme.Muted
            End If
            Me.lblSubtotalValue.Text = String.Format("PHP {0:N2}", _sale.Subtotal)
            Me.lblTotalValue.Text = String.Format("PHP {0:N2}", _sale.Total)
            Me.lblCashValue.Text = String.Format("PHP {0:N2}", _sale.CashReceived)
            Me.lblChangeValue.Text = String.Format("PHP {0:N2}", _sale.ChangeAmount)

            ' Line items — hydrated through the repository (a receipt is
            ' a frozen document: items carry codes + names as recorded).
            Try
                _lineItems.AddRange(SaleRepository.GetSaleItems(_sale.SaleId))
            Catch dataError As Exception
                ShowDataError("Could not load the receipt line items.", dataError)
            End Try

            For Each line As SaleItem In _lineItems
                Dim rowIndex As Integer = Me.detailsGrid.Rows.Add()
                Dim row As DataGridViewRow = Me.detailsGrid.Rows(rowIndex)
                row.Cells(0).Value = String.Format("{0} · {1}", line.ProductCode, line.ProductName)
                row.Cells(1).Value = line.Quantity
                row.Cells(2).Value = String.Format("PHP {0:N2}", line.UnitPrice)
                row.Cells(3).Value = String.Format("PHP {0:N2}", line.LineTotal)
                row.Tag = line.ProductId
            Next
        End Sub

        ' Reprinting must render the IDENTICAL receipt (M10 verify list):
        ' the printer consumes a fresh validated Sale populated with the
        ' frozen line items — no print timestamps ever enter the text.
        Private Function BuildPrintableSale() As Sale
            Dim printable As New Sale(_sale.ReceiptNo, _sale.CashierId, _sale.Subtotal, _sale.Total)
            printable.SaleId = _sale.SaleId
            printable.SaleDate = _sale.SaleDate
            printable.CashierName = _sale.CashierName
            printable.CustomerId = _sale.CustomerId
            printable.CustomerName = _sale.CustomerName
            printable.DiscountType = _sale.DiscountType
            printable.DiscountAmount = _sale.DiscountAmount
            printable.CashReceived = _sale.CashReceived
            printable.ChangeAmount = _sale.ChangeAmount
            For Each line As SaleItem In _lineItems
                printable.AddItem(line)
            Next
            Return printable
        End Function

#End Region

#Region "Buttons"

        Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
            If _sale Is Nothing OrElse _lineItems.Count = 0 Then
                MessageBox.Show(Me, "No receipt is loaded to print.",
                                "Nothing to print", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Try
                ReceiptPrinter.ShowPrintPreview(BuildPrintableSale(), Me)
            Catch printerError As Exception
                MessageBox.Show(Me,
                                "Could not reach the printer for a preview." &
                                Environment.NewLine & printerError.Message,
                                "Printer unavailable", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

#End Region

#Region "Tokens only — no ad-hoc styling"

        Private Sub ApplyRestyle()
            Me.BackColor = Theme.PageBg

            Me.lblReceiptCaption.ForeColor = Theme.Muted
            Me.lblDateTimeCaption.ForeColor = Theme.Muted
            Me.lblCashierCaption.ForeColor = Theme.Muted
            Me.lblCustomerCaption.ForeColor = Theme.Muted
            Me.lblSubtotalCaption.ForeColor = Theme.Muted
            Me.lblDiscountCaption.ForeColor = Theme.Muted
            Me.lblCashCaption.ForeColor = Theme.Muted
            Me.lblChangeCaption.ForeColor = Theme.Muted

            Me.lblReceiptValue.ForeColor = Theme.Ink
            Me.lblDateTimeValue.ForeColor = Theme.Ink
            Me.lblCashierValue.ForeColor = Theme.Ink
            Me.lblCustomerValue.ForeColor = Theme.Ink
            Me.lblSubtotalValue.ForeColor = Theme.Ink
            Me.lblTotalValue.ForeColor = Theme.Ink
            Me.lblCashValue.ForeColor = Theme.Ink
            Me.lblChangeValue.ForeColor = Theme.Ink
            Me.lblTotalValue.Font = Theme.FontFor(ThemeWeight.Bold, 11.0F)
        End Sub

        ' Data-load failures surface with context, never swallowed.
        Private Sub ShowDataError(context As String, dbError As Exception)
            MessageBox.Show(Me,
                            context & Environment.NewLine & dbError.Message,
                            "Data error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

#End Region

    End Class

End Namespace
