Option Explicit On
Option Strict On

Namespace Forms

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ReceiptPreviewForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.rootCard = New Global.oopfinals.Controls.RoundedPanel()
            Me.receiptCard = New Global.oopfinals.Controls.RoundedPanel()
            Me.txtReceipt = New System.Windows.Forms.TextBox()
            Me.pageHeading = New Global.oopfinals.Controls.PageHeading()
            Me.btnBar = New System.Windows.Forms.Panel()
            Me.lblTotalsValue = New System.Windows.Forms.Label()
            Me.lblPrintHint = New System.Windows.Forms.Label()
            Me.btnCancel = New Global.oopfinals.Controls.StyledButton()
            Me.btnPrint = New Global.oopfinals.Controls.StyledButton()
            Me.rootCard.SuspendLayout()
            Me.receiptCard.SuspendLayout()
            Me.btnBar.SuspendLayout()
            Me.SuspendLayout()
            '
            'rootCard
            '
            Me.rootCard.Controls.Add(Me.receiptCard)
            Me.rootCard.Controls.Add(Me.btnBar)
            Me.rootCard.Controls.Add(Me.pageHeading)
            Me.rootCard.Dock = System.Windows.Forms.DockStyle.Fill
            Me.rootCard.Location = New System.Drawing.Point(0, 0)
            Me.rootCard.Name = "rootCard"
            Me.rootCard.Padding = New System.Windows.Forms.Padding(24)
            Me.rootCard.Size = New System.Drawing.Size(984, 661)
            Me.rootCard.TabIndex = 0
            '
            'receiptCard
            '
            Me.receiptCard.Controls.Add(Me.txtReceipt)
            Me.receiptCard.Dock = System.Windows.Forms.DockStyle.Fill
            Me.receiptCard.Location = New System.Drawing.Point(24, 144)
            Me.receiptCard.Name = "receiptCard"
            Me.receiptCard.Padding = New System.Windows.Forms.Padding(16)
            Me.receiptCard.Size = New System.Drawing.Size(936, 421)
            Me.receiptCard.TabIndex = 2
            '
            'txtReceipt
            '
            Me.txtReceipt.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtReceipt.BackColor = System.Drawing.Color.White
            Me.txtReceipt.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.txtReceipt.Font = New System.Drawing.Font("Consolas", 10.0!)
            Me.txtReceipt.Location = New System.Drawing.Point(16, 16)
            Me.txtReceipt.Multiline = True
            Me.txtReceipt.Name = "txtReceipt"
            Me.txtReceipt.ReadOnly = True
            Me.txtReceipt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.txtReceipt.Size = New System.Drawing.Size(904, 389)
            Me.txtReceipt.TabIndex = 0
            Me.txtReceipt.Text = "(The receipt appears here after a completed sale.)"
            '
            'pageHeading
            '
            Me.pageHeading.Dock = System.Windows.Forms.DockStyle.Top
            Me.pageHeading.Location = New System.Drawing.Point(24, 24)
            Me.pageHeading.Name = "pageHeading"
            Me.pageHeading.Size = New System.Drawing.Size(936, 56)
            Me.pageHeading.Subtitle = Nothing
            Me.pageHeading.TabIndex = 0
            Me.pageHeading.TabStop = False
            '
            'btnBar
            '
            Me.btnBar.BackColor = System.Drawing.Color.FromArgb(243, 245, 244)
            Me.btnBar.Controls.Add(Me.lblTotalsValue)
            Me.btnBar.Controls.Add(Me.lblPrintHint)
            Me.btnBar.Controls.Add(Me.btnCancel)
            Me.btnBar.Controls.Add(Me.btnPrint)
            Me.btnBar.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.btnBar.Location = New System.Drawing.Point(24, 573)
            Me.btnBar.Name = "btnBar"
            Me.btnBar.Size = New System.Drawing.Size(936, 64)
            Me.btnBar.TabIndex = 1
            '
            'lblTotalsValue
            '
            Me.lblTotalsValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.lblTotalsValue.AutoSize = True
            Me.lblTotalsValue.Location = New System.Drawing.Point(8, 12)
            Me.lblTotalsValue.Name = "lblTotalsValue"
            Me.lblTotalsValue.Size = New System.Drawing.Size(74, 15)
            Me.lblTotalsValue.TabIndex = 0
            Me.lblTotalsValue.Text = "Totals appear here"
            '
            'lblPrintHint
            '
            Me.lblPrintHint.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblPrintHint.AutoEllipsis = True
            Me.lblPrintHint.Location = New System.Drawing.Point(8, 36)
            Me.lblPrintHint.Name = "lblPrintHint"
            Me.lblPrintHint.Size = New System.Drawing.Size(640, 20)
            Me.lblPrintHint.TabIndex = 1
            Me.lblPrintHint.Text = "Print opens the printer preview; Close returns to the POS and clears the cart."
            '
            'btnCancel (child order: added before btnPrint so it sits left of it)
            '
            Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCancel.Kind = Global.oopfinals.Controls.ThemeButtonKind.Secondary
            Me.btnCancel.Location = New System.Drawing.Point(720, 14)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(96, 36)
            Me.btnCancel.TabIndex = 2
            Me.btnCancel.Text = "Close"
            Me.btnCancel.UseVisualStyleBackColor = False
            '
            'btnPrint
            '
            Me.btnPrint.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnPrint.Kind = Global.oopfinals.Controls.ThemeButtonKind.Primary
            Me.btnPrint.Location = New System.Drawing.Point(824, 14)
            Me.btnPrint.Name = "btnPrint"
            Me.btnPrint.Size = New System.Drawing.Size(112, 36)
            Me.btnPrint.TabIndex = 3
            Me.btnPrint.Text = "Print"
            Me.btnPrint.UseVisualStyleBackColor = False
            '
            'ReceiptPreviewForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(984, 661)
            Me.Controls.Add(Me.rootCard)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ReceiptPreviewForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Receipt Preview"
            Me.rootCard.ResumeLayout(False)
            Me.receiptCard.ResumeLayout(False)
            Me.receiptCard.PerformLayout()
            Me.btnBar.ResumeLayout(False)
            Me.btnBar.PerformLayout()
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents rootCard As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents receiptCard As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents txtReceipt As System.Windows.Forms.TextBox
        Friend WithEvents pageHeading As Global.oopfinals.Controls.PageHeading
        Friend WithEvents btnBar As System.Windows.Forms.Panel
        Friend WithEvents lblTotalsValue As System.Windows.Forms.Label
        Friend WithEvents lblPrintHint As System.Windows.Forms.Label
        Friend WithEvents btnCancel As Global.oopfinals.Controls.StyledButton
        Friend WithEvents btnPrint As Global.oopfinals.Controls.StyledButton

    End Class

End Namespace
