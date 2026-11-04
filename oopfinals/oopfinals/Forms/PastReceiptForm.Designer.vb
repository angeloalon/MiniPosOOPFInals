Option Explicit On
Option Strict On

Namespace Forms

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class PastReceiptForm
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
            Me.detailsCard = New Global.oopfinals.Controls.RoundedPanel()
            Me.detailsGrid = New System.Windows.Forms.DataGridView()
            Me.colDetailItem = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colDetailQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colDetailUnit = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colDetailLine = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.headerPanel = New System.Windows.Forms.Panel()
            Me.lblReceiptCaption = New System.Windows.Forms.Label()
            Me.lblReceiptValue = New System.Windows.Forms.Label()
            Me.lblDateTimeCaption = New System.Windows.Forms.Label()
            Me.lblDateTimeValue = New System.Windows.Forms.Label()
            Me.lblCashierCaption = New System.Windows.Forms.Label()
            Me.lblCashierValue = New System.Windows.Forms.Label()
            Me.lblCustomerCaption = New System.Windows.Forms.Label()
            Me.lblCustomerValue = New System.Windows.Forms.Label()
            Me.totalsPanel = New System.Windows.Forms.Panel()
            Me.lblSubtotalCaption = New System.Windows.Forms.Label()
            Me.lblSubtotalValue = New System.Windows.Forms.Label()
            Me.lblDiscountCaption = New System.Windows.Forms.Label()
            Me.lblDiscountValue = New System.Windows.Forms.Label()
            Me.lblTotalCaption = New System.Windows.Forms.Label()
            Me.lblTotalValue = New System.Windows.Forms.Label()
            Me.lblCashCaption = New System.Windows.Forms.Label()
            Me.lblCashValue = New System.Windows.Forms.Label()
            Me.lblChangeCaption = New System.Windows.Forms.Label()
            Me.lblChangeValue = New System.Windows.Forms.Label()
            Me.pageHeading = New Global.oopfinals.Controls.PageHeading()
            Me.btnBar = New System.Windows.Forms.Panel()
            Me.btnClose = New Global.oopfinals.Controls.StyledButton()
            Me.btnPrint = New Global.oopfinals.Controls.StyledButton()
            Me.rootCard.SuspendLayout()
            Me.detailsCard.SuspendLayout()
            CType(Me.detailsGrid, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.headerPanel.SuspendLayout()
            Me.totalsPanel.SuspendLayout()
            Me.btnBar.SuspendLayout()
            Me.SuspendLayout()
            '
            'rootCard
            '
            Me.rootCard.Controls.Add(Me.detailsCard)
            Me.rootCard.Controls.Add(Me.btnBar)
            Me.rootCard.Controls.Add(Me.pageHeading)
            Me.rootCard.Dock = System.Windows.Forms.DockStyle.Fill
            Me.rootCard.Location = New System.Drawing.Point(0, 0)
            Me.rootCard.Name = "rootCard"
            Me.rootCard.Padding = New System.Windows.Forms.Padding(24)
            Me.rootCard.Size = New System.Drawing.Size(984, 661)
            Me.rootCard.TabIndex = 0
            '
            'detailsCard
            '
            Me.detailsCard.Controls.Add(Me.detailsGrid)
            Me.detailsCard.Controls.Add(Me.totalsPanel)
            Me.detailsCard.Controls.Add(Me.headerPanel)
            Me.detailsCard.Dock = System.Windows.Forms.DockStyle.Fill
            Me.detailsCard.Location = New System.Drawing.Point(24, 144)
            Me.detailsCard.Name = "detailsCard"
            Me.detailsCard.Padding = New System.Windows.Forms.Padding(8)
            Me.detailsCard.Size = New System.Drawing.Size(936, 421)
            Me.detailsCard.TabIndex = 2
            '
            'headerPanel (identity strip: receipt no, date, cashier, customer)
            '
            Me.headerPanel.BackColor = System.Drawing.Color.White
            Me.headerPanel.Controls.Add(Me.lblReceiptCaption)
            Me.headerPanel.Controls.Add(Me.lblReceiptValue)
            Me.headerPanel.Controls.Add(Me.lblDateTimeCaption)
            Me.headerPanel.Controls.Add(Me.lblDateTimeValue)
            Me.headerPanel.Controls.Add(Me.lblCashierCaption)
            Me.headerPanel.Controls.Add(Me.lblCashierValue)
            Me.headerPanel.Controls.Add(Me.lblCustomerCaption)
            Me.headerPanel.Controls.Add(Me.lblCustomerValue)
            Me.headerPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me.headerPanel.Location = New System.Drawing.Point(8, 8)
            Me.headerPanel.Name = "headerPanel"
            Me.headerPanel.Size = New System.Drawing.Size(920, 66)
            Me.headerPanel.TabIndex = 1
            '
            'lblReceiptCaption
            '
            Me.lblReceiptCaption.AutoSize = True
            Me.lblReceiptCaption.Location = New System.Drawing.Point(12, 6)
            Me.lblReceiptCaption.Name = "lblReceiptCaption"
            Me.lblReceiptCaption.Size = New System.Drawing.Size(52, 15)
            Me.lblReceiptCaption.TabIndex = 0
            Me.lblReceiptCaption.Text = "Receipt:"
            '
            'lblReceiptValue
            '
            Me.lblReceiptValue.AutoSize = True
            Me.lblReceiptValue.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!)
            Me.lblReceiptValue.Location = New System.Drawing.Point(92, 5)
            Me.lblReceiptValue.Name = "lblReceiptValue"
            Me.lblReceiptValue.Size = New System.Drawing.Size(60, 19)
            Me.lblReceiptValue.TabIndex = 1
            Me.lblReceiptValue.Text = "RCP-…"
            '
            'lblDateTimeCaption
            '
            Me.lblDateTimeCaption.AutoSize = True
            Me.lblDateTimeCaption.Location = New System.Drawing.Point(330, 6)
            Me.lblDateTimeCaption.Name = "lblDateTimeCaption"
            Me.lblDateTimeCaption.Size = New System.Drawing.Size(53, 15)
            Me.lblDateTimeCaption.TabIndex = 2
            Me.lblDateTimeCaption.Text = "Date/time:"
            '
            'lblDateTimeValue
            '
            Me.lblDateTimeValue.AutoSize = True
            Me.lblDateTimeValue.Location = New System.Drawing.Point(410, 6)
            Me.lblDateTimeValue.Name = "lblDateTimeValue"
            Me.lblDateTimeValue.Size = New System.Drawing.Size(120, 15)
            Me.lblDateTimeValue.TabIndex = 3
            Me.lblDateTimeValue.Text = "yyyy-MM-dd HH:mm:ss"
            '
            'lblCashierCaption
            '
            Me.lblCashierCaption.AutoSize = True
            Me.lblCashierCaption.Location = New System.Drawing.Point(12, 38)
            Me.lblCashierCaption.Name = "lblCashierCaption"
            Me.lblCashierCaption.Size = New System.Drawing.Size(47, 15)
            Me.lblCashierCaption.TabIndex = 4
            Me.lblCashierCaption.Text = "Cashier:"
            '
            'lblCashierValue
            '
            Me.lblCashierValue.AutoSize = True
            Me.lblCashierValue.Location = New System.Drawing.Point(92, 38)
            Me.lblCashierValue.Name = "lblCashierValue"
            Me.lblCashierValue.Size = New System.Drawing.Size(40, 15)
            Me.lblCashierValue.TabIndex = 5
            Me.lblCashierValue.Text = "Cashier"
            '
            'lblCustomerCaption
            '
            Me.lblCustomerCaption.AutoSize = True
            Me.lblCustomerCaption.Location = New System.Drawing.Point(330, 38)
            Me.lblCustomerCaption.Name = "lblCustomerCaption"
            Me.lblCustomerCaption.Size = New System.Drawing.Size(58, 15)
            Me.lblCustomerCaption.TabIndex = 6
            Me.lblCustomerCaption.Text = "Customer:"
            '
            'lblCustomerValue
            '
            Me.lblCustomerValue.AutoSize = True
            Me.lblCustomerValue.Location = New System.Drawing.Point(410, 38)
            Me.lblCustomerValue.Name = "lblCustomerValue"
            Me.lblCustomerValue.Size = New System.Drawing.Size(52, 15)
            Me.lblCustomerValue.TabIndex = 7
            Me.lblCustomerValue.Text = "Walk-In"
            '
            'totalsPanel (money strip across the bottom of the card)
            '
            Me.totalsPanel.BackColor = System.Drawing.Color.White
            Me.totalsPanel.Controls.Add(Me.lblSubtotalCaption)
            Me.totalsPanel.Controls.Add(Me.lblSubtotalValue)
            Me.totalsPanel.Controls.Add(Me.lblDiscountCaption)
            Me.totalsPanel.Controls.Add(Me.lblDiscountValue)
            Me.totalsPanel.Controls.Add(Me.lblTotalCaption)
            Me.totalsPanel.Controls.Add(Me.lblTotalValue)
            Me.totalsPanel.Controls.Add(Me.lblCashCaption)
            Me.totalsPanel.Controls.Add(Me.lblCashValue)
            Me.totalsPanel.Controls.Add(Me.lblChangeCaption)
            Me.totalsPanel.Controls.Add(Me.lblChangeValue)
            Me.totalsPanel.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.totalsPanel.Location = New System.Drawing.Point(8, 335)
            Me.totalsPanel.Name = "totalsPanel"
            Me.totalsPanel.Size = New System.Drawing.Size(920, 78)
            Me.totalsPanel.TabIndex = 2
            '
            'lblSubtotalCaption
            '
            Me.lblSubtotalCaption.AutoSize = True
            Me.lblSubtotalCaption.Location = New System.Drawing.Point(12, 8)
            Me.lblSubtotalCaption.Name = "lblSubtotalCaption"
            Me.lblSubtotalCaption.Size = New System.Drawing.Size(50, 15)
            Me.lblSubtotalCaption.TabIndex = 0
            Me.lblSubtotalCaption.Text = "Subtotal"
            '
            'lblSubtotalValue (money labels stay AutoSize)
            '
            Me.lblSubtotalValue.AutoSize = True
            Me.lblSubtotalValue.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!)
            Me.lblSubtotalValue.Location = New System.Drawing.Point(12, 24)
            Me.lblSubtotalValue.Name = "lblSubtotalValue"
            Me.lblSubtotalValue.Size = New System.Drawing.Size(60, 19)
            Me.lblSubtotalValue.TabIndex = 1
            Me.lblSubtotalValue.Text = "PHP 0.00"
            '
            'lblDiscountCaption
            '
            Me.lblDiscountCaption.AutoSize = True
            Me.lblDiscountCaption.Location = New System.Drawing.Point(180, 8)
            Me.lblDiscountCaption.Name = "lblDiscountCaption"
            Me.lblDiscountCaption.Size = New System.Drawing.Size(56, 15)
            Me.lblDiscountCaption.TabIndex = 2
            Me.lblDiscountCaption.Text = "Discount"
            '
            'lblDiscountValue
            '
            Me.lblDiscountValue.AutoSize = True
            Me.lblDiscountValue.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!)
            Me.lblDiscountValue.Location = New System.Drawing.Point(180, 24)
            Me.lblDiscountValue.Name = "lblDiscountValue"
            Me.lblDiscountValue.Size = New System.Drawing.Size(60, 19)
            Me.lblDiscountValue.TabIndex = 3
            Me.lblDiscountValue.Text = "PHP 0.00"
            '
            'lblTotalCaption
            '
            Me.lblTotalCaption.AutoSize = True
            Me.lblTotalCaption.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
            Me.lblTotalCaption.Location = New System.Drawing.Point(360, 8)
            Me.lblTotalCaption.Name = "lblTotalCaption"
            Me.lblTotalCaption.Size = New System.Drawing.Size(66, 17)
            Me.lblTotalCaption.TabIndex = 4
            Me.lblTotalCaption.Text = "TOTAL DUE"
            '
            'lblTotalValue
            '
            Me.lblTotalValue.AutoSize = True
            Me.lblTotalValue.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
            Me.lblTotalValue.Location = New System.Drawing.Point(360, 24)
            Me.lblTotalValue.Name = "lblTotalValue"
            Me.lblTotalValue.Size = New System.Drawing.Size(85, 24)
            Me.lblTotalValue.TabIndex = 5
            Me.lblTotalValue.Text = "PHP 0.00"
            '
            'lblCashCaption
            '
            Me.lblCashCaption.AutoSize = True
            Me.lblCashCaption.Location = New System.Drawing.Point(560, 8)
            Me.lblCashCaption.Name = "lblCashCaption"
            Me.lblCashCaption.Size = New System.Drawing.Size(80, 15)
            Me.lblCashCaption.TabIndex = 6
            Me.lblCashCaption.Text = "Cash received"
            '
            'lblCashValue
            '
            Me.lblCashValue.AutoSize = True
            Me.lblCashValue.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!)
            Me.lblCashValue.Location = New System.Drawing.Point(560, 24)
            Me.lblCashValue.Name = "lblCashValue"
            Me.lblCashValue.Size = New System.Drawing.Size(60, 19)
            Me.lblCashValue.TabIndex = 7
            Me.lblCashValue.Text = "PHP 0.00"
            '
            'lblChangeCaption
            '
            Me.lblChangeCaption.AutoSize = True
            Me.lblChangeCaption.Location = New System.Drawing.Point(740, 8)
            Me.lblChangeCaption.Name = "lblChangeCaption"
            Me.lblChangeCaption.Size = New System.Drawing.Size(46, 15)
            Me.lblChangeCaption.TabIndex = 8
            Me.lblChangeCaption.Text = "Change"
            '
            'lblChangeValue
            '
            Me.lblChangeValue.AutoSize = True
            Me.lblChangeValue.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!)
            Me.lblChangeValue.Location = New System.Drawing.Point(740, 24)
            Me.lblChangeValue.Name = "lblChangeValue"
            Me.lblChangeValue.Size = New System.Drawing.Size(60, 19)
            Me.lblChangeValue.TabIndex = 9
            Me.lblChangeValue.Text = "PHP 0.00"
            '
            'detailsGrid
            '
            Me.detailsGrid.AllowUserToAddRows = False
            Me.detailsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            Me.detailsGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colDetailItem, Me.colDetailQty, Me.colDetailUnit, Me.colDetailLine})
            Me.detailsGrid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.detailsGrid.Location = New System.Drawing.Point(8, 74)
            Me.detailsGrid.MultiSelect = False
            Me.detailsGrid.Name = "detailsGrid"
            Me.detailsGrid.ReadOnly = True
            Me.detailsGrid.RowHeadersVisible = False
            Me.detailsGrid.Size = New System.Drawing.Size(920, 261)
            Me.detailsGrid.TabIndex = 0
            '
            'colDetailItem
            '
            Me.colDetailItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            Me.colDetailItem.HeaderText = "Item"
            Me.colDetailItem.Name = "colDetailItem"
            Me.colDetailItem.ReadOnly = True
            '
            'colDetailQty
            '
            Me.colDetailQty.HeaderText = "Qty"
            Me.colDetailQty.Name = "colDetailQty"
            Me.colDetailQty.ReadOnly = True
            Me.colDetailQty.Width = 56
            '
            'colDetailUnit
            '
            Me.colDetailUnit.HeaderText = "Unit price"
            Me.colDetailUnit.Name = "colDetailUnit"
            Me.colDetailUnit.ReadOnly = True
            Me.colDetailUnit.Width = 96
            '
            'colDetailLine
            '
            Me.colDetailLine.HeaderText = "Line total"
            Me.colDetailLine.Name = "colDetailLine"
            Me.colDetailLine.ReadOnly = True
            Me.colDetailLine.Width = 104
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
            Me.btnBar.Controls.Add(Me.btnClose)
            Me.btnBar.Controls.Add(Me.btnPrint)
            Me.btnBar.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.btnBar.Location = New System.Drawing.Point(24, 573)
            Me.btnBar.Name = "btnBar"
            Me.btnBar.Padding = New System.Windows.Forms.Padding(8)
            Me.btnBar.Size = New System.Drawing.Size(936, 64)
            Me.btnBar.TabIndex = 1
            '
            'btnClose
            '
            Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnClose.Kind = Global.oopfinals.Controls.ThemeButtonKind.Secondary
            Me.btnClose.Location = New System.Drawing.Point(712, 14)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(96, 36)
            Me.btnClose.TabIndex = 0
            Me.btnClose.Text = "Close"
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'btnPrint
            '
            Me.btnPrint.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnPrint.Kind = Global.oopfinals.Controls.ThemeButtonKind.Primary
            Me.btnPrint.Location = New System.Drawing.Point(816, 14)
            Me.btnPrint.Name = "btnPrint"
            Me.btnPrint.Size = New System.Drawing.Size(120, 36)
            Me.btnPrint.TabIndex = 1
            Me.btnPrint.Text = "Print receipt"
            Me.btnPrint.UseVisualStyleBackColor = False
            '
            'PastReceiptForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(984, 661)
            Me.Controls.Add(Me.rootCard)
            Me.Name = "PastReceiptForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Past Receipt"
            Me.rootCard.ResumeLayout(False)
            Me.detailsCard.ResumeLayout(False)
            CType(Me.detailsGrid, System.ComponentModel.ISupportInitialize).EndInit()
            Me.headerPanel.ResumeLayout(False)
            Me.headerPanel.PerformLayout()
            Me.totalsPanel.ResumeLayout(False)
            Me.totalsPanel.PerformLayout()
            Me.btnBar.ResumeLayout(False)
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents rootCard As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents detailsCard As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents detailsGrid As System.Windows.Forms.DataGridView
        Friend WithEvents colDetailItem As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colDetailQty As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colDetailUnit As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colDetailLine As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents headerPanel As System.Windows.Forms.Panel
        Friend WithEvents lblReceiptCaption As System.Windows.Forms.Label
        Friend WithEvents lblReceiptValue As System.Windows.Forms.Label
        Friend WithEvents lblDateTimeCaption As System.Windows.Forms.Label
        Friend WithEvents lblDateTimeValue As System.Windows.Forms.Label
        Friend WithEvents lblCashierCaption As System.Windows.Forms.Label
        Friend WithEvents lblCashierValue As System.Windows.Forms.Label
        Friend WithEvents lblCustomerCaption As System.Windows.Forms.Label
        Friend WithEvents lblCustomerValue As System.Windows.Forms.Label
        Friend WithEvents totalsPanel As System.Windows.Forms.Panel
        Friend WithEvents lblSubtotalCaption As System.Windows.Forms.Label
        Friend WithEvents lblSubtotalValue As System.Windows.Forms.Label
        Friend WithEvents lblDiscountCaption As System.Windows.Forms.Label
        Friend WithEvents lblDiscountValue As System.Windows.Forms.Label
        Friend WithEvents lblTotalCaption As System.Windows.Forms.Label
        Friend WithEvents lblTotalValue As System.Windows.Forms.Label
        Friend WithEvents lblCashCaption As System.Windows.Forms.Label
        Friend WithEvents lblCashValue As System.Windows.Forms.Label
        Friend WithEvents lblChangeCaption As System.Windows.Forms.Label
        Friend WithEvents lblChangeValue As System.Windows.Forms.Label
        Friend WithEvents pageHeading As Global.oopfinals.Controls.PageHeading
        Friend WithEvents btnBar As System.Windows.Forms.Panel
        Friend WithEvents btnClose As Global.oopfinals.Controls.StyledButton
        Friend WithEvents btnPrint As Global.oopfinals.Controls.StyledButton

    End Class

End Namespace
