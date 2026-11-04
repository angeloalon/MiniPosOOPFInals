Option Explicit On
Option Strict On

Namespace Forms

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class CashierDashboardForm
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
            Me.appHeader = New Global.oopfinals.Controls.AppHeader()
            Me.btnLogout = New Global.oopfinals.Controls.StyledButton()
            Me.tabRail = New Global.oopfinals.Controls.IconTabBar()
            Me.contentCard = New Global.oopfinals.Controls.RoundedPanel()
            Me.viewHost = New System.Windows.Forms.Panel()
            Me.permissionsView = New Global.oopfinals.Controls.RoundedPanel()
            Me.permissionsGrid = New System.Windows.Forms.DataGridView()
            Me.colPermission = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.todayView = New Global.oopfinals.Controls.RoundedPanel()
            Me.todayGrid = New System.Windows.Forms.DataGridView()
            Me.colTodayReceipt = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colTodayTime = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colTodayCashier = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colTodayTotal = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.todayHeader = New System.Windows.Forms.Panel()
            Me.lblTodayTitle = New System.Windows.Forms.Label()
            Me.lblTodayCount = New System.Windows.Forms.Label()
            Me.btnTodayDetails = New Global.oopfinals.Controls.StyledButton()
            Me.btnTodayRefresh = New Global.oopfinals.Controls.StyledButton()
            Me.posContainer = New Global.oopfinals.Controls.RoundedPanel()
            Me.cartPanel = New System.Windows.Forms.Panel()
            Me.cartHeader = New System.Windows.Forms.Panel()
            Me.lblCartTitle = New System.Windows.Forms.Label()
            Me.lblCartCount = New System.Windows.Forms.Label()
            Me.lblScanCaption = New System.Windows.Forms.Label()
            Me.txtScan = New System.Windows.Forms.TextBox()
            Me.btnScanAdd = New Global.oopfinals.Controls.StyledButton()
            Me.btnQtyMinus = New Global.oopfinals.Controls.StyledButton()
            Me.btnQtyPlus = New Global.oopfinals.Controls.StyledButton()
            Me.btnRemoveItem = New Global.oopfinals.Controls.StyledButton()
            Me.btnClearCart = New Global.oopfinals.Controls.StyledButton()
            Me.cartGrid = New System.Windows.Forms.DataGridView()
            Me.colCartItem = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colCartQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colCartUnit = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colCartLine = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.productsPanel = New System.Windows.Forms.Panel()
            Me.productsHeader = New System.Windows.Forms.Panel()
            Me.lblProductsTitle = New System.Windows.Forms.Label()
            Me.lblGridHint = New System.Windows.Forms.Label()
            Me.lblSearchCaption = New System.Windows.Forms.Label()
            Me.txtSearch = New System.Windows.Forms.TextBox()
            Me.lblCategoryCaption = New System.Windows.Forms.Label()
            Me.cmbCategory = New System.Windows.Forms.ComboBox()
            Me.posGrid = New System.Windows.Forms.DataGridView()
            Me.colProduct = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colStock = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.paymentRail = New System.Windows.Forms.Panel()
            Me.lblPaymentTitle = New System.Windows.Forms.Label()
            Me.lblDiscountCaption = New System.Windows.Forms.Label()
            Me.cmbDiscount = New System.Windows.Forms.ComboBox()
            Me.lblCashCaption = New System.Windows.Forms.Label()
            Me.txtCash = New System.Windows.Forms.TextBox()
            Me.lblTotalsTitle = New System.Windows.Forms.Label()
            Me.lblSubtotalCaption = New System.Windows.Forms.Label()
            Me.lblSubtotalValue = New System.Windows.Forms.Label()
            Me.lblDiscountTotalCaption = New System.Windows.Forms.Label()
            Me.lblDiscountValue = New System.Windows.Forms.Label()
            Me.separatorPanel = New System.Windows.Forms.Panel()
            Me.lblTotalDueCaption = New System.Windows.Forms.Label()
            Me.lblTotalValue = New System.Windows.Forms.Label()
            Me.lblChangeCaption = New System.Windows.Forms.Label()
            Me.lblChangeValue = New System.Windows.Forms.Label()
            Me.btnCheckout = New Global.oopfinals.Controls.StyledButton()
            Me.lblCheckoutHint = New System.Windows.Forms.Label()
            Me.pageHeading = New Global.oopfinals.Controls.PageHeading()
            CType(Me.permissionsGrid, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.cartGrid, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.posGrid, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.todayGrid, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.permissionsView.SuspendLayout()
            Me.todayView.SuspendLayout()
            Me.todayHeader.SuspendLayout()
            Me.posContainer.SuspendLayout()
            Me.cartPanel.SuspendLayout()
            Me.cartHeader.SuspendLayout()
            Me.productsPanel.SuspendLayout()
            Me.productsHeader.SuspendLayout()
            Me.paymentRail.SuspendLayout()
            Me.viewHost.SuspendLayout()
            Me.contentCard.SuspendLayout()
            Me.SuspendLayout()
            '
            'appHeader
            '
            Me.appHeader.Controls.Add(Me.btnLogout)
            Me.appHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.appHeader.Location = New System.Drawing.Point(0, 0)
            Me.appHeader.Name = "appHeader"
            Me.appHeader.Size = New System.Drawing.Size(984, 64)
            Me.appHeader.TabIndex = 2
            '
            'btnLogout (child of appHeader → always paints above it)
            '
            Me.btnLogout.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnLogout.Kind = Global.oopfinals.Controls.ThemeButtonKind.Danger
            Me.btnLogout.Location = New System.Drawing.Point(848, 14)
            Me.btnLogout.Name = "btnLogout"
            Me.btnLogout.Size = New System.Drawing.Size(120, 36)
            Me.btnLogout.TabIndex = 0
            Me.btnLogout.Text = "Logout"
            Me.btnLogout.UseVisualStyleBackColor = False
            '
            'tabRail
            '
            Me.tabRail.Dock = System.Windows.Forms.DockStyle.Left
            Me.tabRail.Location = New System.Drawing.Point(0, 64)
            Me.tabRail.Name = "tabRail"
            Me.tabRail.Size = New System.Drawing.Size(220, 597)
            Me.tabRail.TabIndex = 1
            '
            'contentCard
            '
            Me.contentCard.Controls.Add(Me.viewHost)
            Me.contentCard.Controls.Add(Me.pageHeading)
            Me.contentCard.Dock = System.Windows.Forms.DockStyle.Fill
            Me.contentCard.Location = New System.Drawing.Point(220, 64)
            Me.contentCard.Name = "contentCard"
            Me.contentCard.Padding = New System.Windows.Forms.Padding(24)
            Me.contentCard.Size = New System.Drawing.Size(764, 597)
            Me.contentCard.TabIndex = 0
            '
            'pageHeading
            '
            Me.pageHeading.Dock = System.Windows.Forms.DockStyle.Top
            Me.pageHeading.Location = New System.Drawing.Point(24, 24)
            Me.pageHeading.Name = "pageHeading"
            Me.pageHeading.Size = New System.Drawing.Size(716, 56)
            Me.pageHeading.Subtitle = Nothing
            Me.pageHeading.TabIndex = 1
            Me.pageHeading.TabStop = False
            '
            'viewHost (swaps POS ↔ Today ↔ Permissions by visibility)
            '
            Me.viewHost.BackColor = System.Drawing.Color.FromArgb(243, 245, 244)
            Me.viewHost.Controls.Add(Me.permissionsView)
            Me.viewHost.Controls.Add(Me.todayView)
            Me.viewHost.Controls.Add(Me.posContainer)
            Me.viewHost.Dock = System.Windows.Forms.DockStyle.Fill
            Me.viewHost.Location = New System.Drawing.Point(24, 80)
            Me.viewHost.Name = "viewHost"
            Me.viewHost.Size = New System.Drawing.Size(716, 493)
            Me.viewHost.TabIndex = 0
            '
            'permissionsView
            '
            Me.permissionsView.Controls.Add(Me.permissionsGrid)
            Me.permissionsView.Dock = System.Windows.Forms.DockStyle.Fill
            Me.permissionsView.Location = New System.Drawing.Point(0, 0)
            Me.permissionsView.Name = "permissionsView"
            Me.permissionsView.Padding = New System.Windows.Forms.Padding(8)
            Me.permissionsView.Size = New System.Drawing.Size(716, 493)
            Me.permissionsView.TabIndex = 1
            Me.permissionsView.Visible = False
            '
            'permissionsGrid
            '
            Me.permissionsGrid.AllowUserToAddRows = False
            Me.permissionsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
            Me.permissionsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            Me.permissionsGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colPermission})
            Me.permissionsGrid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.permissionsGrid.Location = New System.Drawing.Point(8, 8)
            Me.permissionsGrid.MultiSelect = False
            Me.permissionsGrid.Name = "permissionsGrid"
            Me.permissionsGrid.ReadOnly = True
            Me.permissionsGrid.RowHeadersVisible = False
            Me.permissionsGrid.Size = New System.Drawing.Size(700, 477)
            Me.permissionsGrid.TabIndex = 0
            '
            'colPermission
            '
            Me.colPermission.HeaderText = "Permission key"
            Me.colPermission.Name = "colPermission"
            Me.colPermission.ReadOnly = True
            '
            'todayView (Today tab host — swaps with posContainer)
            '
            Me.todayView.Controls.Add(Me.todayGrid)
            Me.todayView.Controls.Add(Me.todayHeader)
            Me.todayView.Dock = System.Windows.Forms.DockStyle.Fill
            Me.todayView.Location = New System.Drawing.Point(0, 0)
            Me.todayView.Name = "todayView"
            Me.todayView.Padding = New System.Windows.Forms.Padding(8)
            Me.todayView.Size = New System.Drawing.Size(716, 493)
            Me.todayView.TabIndex = 2
            Me.todayView.Visible = False
            '
            'todayHeader
            '
            Me.todayHeader.BackColor = System.Drawing.Color.White
            Me.todayHeader.Controls.Add(Me.lblTodayTitle)
            Me.todayHeader.Controls.Add(Me.lblTodayCount)
            Me.todayHeader.Controls.Add(Me.btnTodayDetails)
            Me.todayHeader.Controls.Add(Me.btnTodayRefresh)
            Me.todayHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.todayHeader.Location = New System.Drawing.Point(8, 8)
            Me.todayHeader.Name = "todayHeader"
            Me.todayHeader.Size = New System.Drawing.Size(700, 64)
            Me.todayHeader.TabIndex = 1
            '
            'lblTodayTitle
            '
            Me.lblTodayTitle.AutoSize = True
            Me.lblTodayTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 11.0!)
            Me.lblTodayTitle.ForeColor = System.Drawing.Color.FromArgb(26, 31, 28)
            Me.lblTodayTitle.Location = New System.Drawing.Point(8, 6)
            Me.lblTodayTitle.Name = "lblTodayTitle"
            Me.lblTodayTitle.Size = New System.Drawing.Size(188, 24)
            Me.lblTodayTitle.TabIndex = 0
            Me.lblTodayTitle.Text = "Today's transactions"
            '
            'lblTodayCount
            '
            Me.lblTodayCount.AutoSize = True
            Me.lblTodayCount.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128)
            Me.lblTodayCount.Location = New System.Drawing.Point(12, 34)
            Me.lblTodayCount.Name = "lblTodayCount"
            Me.lblTodayCount.Size = New System.Drawing.Size(108, 15)
            Me.lblTodayCount.TabIndex = 1
            Me.lblTodayCount.Text = "0 receipt(s) today"
            '
            'btnTodayDetails
            '
            Me.btnTodayDetails.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnTodayDetails.Enabled = False
            Me.btnTodayDetails.Kind = Global.oopfinals.Controls.ThemeButtonKind.Secondary
            Me.btnTodayDetails.Location = New System.Drawing.Point(476, 16)
            Me.btnTodayDetails.Name = "btnTodayDetails"
            Me.btnTodayDetails.Size = New System.Drawing.Size(110, 32)
            Me.btnTodayDetails.TabIndex = 2
            Me.btnTodayDetails.Text = "View details"
            Me.btnTodayDetails.UseVisualStyleBackColor = False
            '
            'btnTodayRefresh
            '
            Me.btnTodayRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnTodayRefresh.Kind = Global.oopfinals.Controls.ThemeButtonKind.Secondary
            Me.btnTodayRefresh.Location = New System.Drawing.Point(594, 16)
            Me.btnTodayRefresh.Name = "btnTodayRefresh"
            Me.btnTodayRefresh.Size = New System.Drawing.Size(106, 32)
            Me.btnTodayRefresh.TabIndex = 3
            Me.btnTodayRefresh.Text = "Refresh"
            Me.btnTodayRefresh.UseVisualStyleBackColor = False
            '
            'todayGrid
            '
            Me.todayGrid.AllowUserToAddRows = False
            Me.todayGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            Me.todayGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colTodayReceipt, Me.colTodayTime, Me.colTodayCashier, Me.colTodayTotal})
            Me.todayGrid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.todayGrid.Location = New System.Drawing.Point(8, 72)
            Me.todayGrid.MultiSelect = False
            Me.todayGrid.Name = "todayGrid"
            Me.todayGrid.ReadOnly = True
            Me.todayGrid.RowHeadersVisible = False
            Me.todayGrid.Size = New System.Drawing.Size(700, 413)
            Me.todayGrid.TabIndex = 0
            '
            'colTodayReceipt
            '
            Me.colTodayReceipt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            Me.colTodayReceipt.HeaderText = "Receipt No"
            Me.colTodayReceipt.Name = "colTodayReceipt"
            Me.colTodayReceipt.ReadOnly = True
            '
            'colTodayTime
            '
            Me.colTodayTime.HeaderText = "Time"
            Me.colTodayTime.Name = "colTodayTime"
            Me.colTodayTime.ReadOnly = True
            Me.colTodayTime.Width = 90
            '
            'colTodayCashier
            '
            Me.colTodayCashier.HeaderText = "Cashier"
            Me.colTodayCashier.Name = "colTodayCashier"
            Me.colTodayCashier.ReadOnly = True
            Me.colTodayCashier.Width = 180
            '
            'colTodayTotal
            '
            Me.colTodayTotal.HeaderText = "Total"
            Me.colTodayTotal.Name = "colTodayTotal"
            Me.colTodayTotal.ReadOnly = True
            Me.colTodayTotal.Width = 110
            '
            'posContainer
            '
            Me.posContainer.Controls.Add(Me.cartPanel)
            Me.posContainer.Controls.Add(Me.productsPanel)
            Me.posContainer.Controls.Add(Me.paymentRail)
            Me.posContainer.Dock = System.Windows.Forms.DockStyle.Fill
            Me.posContainer.Location = New System.Drawing.Point(0, 0)
            Me.posContainer.Name = "posContainer"
            Me.posContainer.Padding = New System.Windows.Forms.Padding(8)
            Me.posContainer.Size = New System.Drawing.Size(716, 493)
            Me.posContainer.TabIndex = 0
            '
            'cartPanel (Dock.Fill middle column)
            '
            Me.cartPanel.BackColor = System.Drawing.Color.White
            Me.cartPanel.Controls.Add(Me.cartGrid)
            Me.cartPanel.Controls.Add(Me.cartHeader)
            Me.cartPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.cartPanel.Location = New System.Drawing.Point(8, 8)
            Me.cartPanel.Name = "cartPanel"
            Me.cartPanel.Padding = New System.Windows.Forms.Padding(8)
            Me.cartPanel.Size = New System.Drawing.Size(60, 477)
            Me.cartPanel.TabIndex = 2
            '
            'cartHeader
            '
            Me.cartHeader.BackColor = System.Drawing.Color.White
            Me.cartHeader.Controls.Add(Me.lblCartTitle)
            Me.cartHeader.Controls.Add(Me.lblCartCount)
            Me.cartHeader.Controls.Add(Me.lblScanCaption)
            Me.cartHeader.Controls.Add(Me.txtScan)
            Me.cartHeader.Controls.Add(Me.btnScanAdd)
            Me.cartHeader.Controls.Add(Me.btnQtyMinus)
            Me.cartHeader.Controls.Add(Me.btnQtyPlus)
            Me.cartHeader.Controls.Add(Me.btnRemoveItem)
            Me.cartHeader.Controls.Add(Me.btnClearCart)
            Me.cartHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.cartHeader.Location = New System.Drawing.Point(8, 8)
            Me.cartHeader.Name = "cartHeader"
            Me.cartHeader.Size = New System.Drawing.Size(44, 124)
            Me.cartHeader.TabIndex = 1
            '
            'lblCartTitle
            '
            Me.lblCartTitle.AutoSize = True
            Me.lblCartTitle.BackColor = System.Drawing.Color.White
            Me.lblCartTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 11.0!)
            Me.lblCartTitle.ForeColor = System.Drawing.Color.FromArgb(26, 31, 28)
            Me.lblCartTitle.Location = New System.Drawing.Point(8, 4)
            Me.lblCartTitle.Name = "lblCartTitle"
            Me.lblCartTitle.Size = New System.Drawing.Size(72, 24)
            Me.lblCartTitle.TabIndex = 0
            Me.lblCartTitle.Text = "Cart"
            '
            'lblCartCount
            '
            Me.lblCartCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblCartCount.BackColor = System.Drawing.Color.White
            Me.lblCartCount.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128)
            Me.lblCartCount.Location = New System.Drawing.Point(190, 8)
            Me.lblCartCount.Name = "lblCartCount"
            Me.lblCartCount.Size = New System.Drawing.Size(340, 20)
            Me.lblCartCount.TabIndex = 1
            Me.lblCartCount.Text = "0 line(s) · 0 unit(s)"
            Me.lblCartCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'lblScanCaption
            '
            Me.lblScanCaption.AutoSize = True
            Me.lblScanCaption.BackColor = System.Drawing.Color.White
            Me.lblScanCaption.Location = New System.Drawing.Point(8, 42)
            Me.lblScanCaption.Name = "lblScanCaption"
            Me.lblScanCaption.Size = New System.Drawing.Size(40, 15)
            Me.lblScanCaption.TabIndex = 2
            Me.lblScanCaption.Text = "Scan:"
            '
            'txtScan
            '
            Me.txtScan.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtScan.Location = New System.Drawing.Point(56, 38)
            Me.txtScan.Name = "txtScan"
            Me.txtScan.Size = New System.Drawing.Size(296, 23)
            Me.txtScan.TabIndex = 3
            '
            'btnScanAdd
            '
            Me.btnScanAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnScanAdd.Kind = Global.oopfinals.Controls.ThemeButtonKind.Secondary
            Me.btnScanAdd.Location = New System.Drawing.Point(360, 36)
            Me.btnScanAdd.Name = "btnScanAdd"
            Me.btnScanAdd.Size = New System.Drawing.Size(104, 27)
            Me.btnScanAdd.TabIndex = 4
            Me.btnScanAdd.Text = "Add item"
            Me.btnScanAdd.UseVisualStyleBackColor = False
            '
            'btnQtyMinus
            '
            Me.btnQtyMinus.Kind = Global.oopfinals.Controls.ThemeButtonKind.Secondary
            Me.btnQtyMinus.Location = New System.Drawing.Point(8, 74)
            Me.btnQtyMinus.Name = "btnQtyMinus"
            Me.btnQtyMinus.Size = New System.Drawing.Size(104, 32)
            Me.btnQtyMinus.TabIndex = 5
            Me.btnQtyMinus.Text = "− Qty"
            Me.btnQtyMinus.UseVisualStyleBackColor = False
            '
            'btnQtyPlus
            '
            Me.btnQtyPlus.Kind = Global.oopfinals.Controls.ThemeButtonKind.Secondary
            Me.btnQtyPlus.Location = New System.Drawing.Point(118, 74)
            Me.btnQtyPlus.Name = "btnQtyPlus"
            Me.btnQtyPlus.Size = New System.Drawing.Size(104, 32)
            Me.btnQtyPlus.TabIndex = 6
            Me.btnQtyPlus.Text = "+ Qty"
            Me.btnQtyPlus.UseVisualStyleBackColor = False
            '
            'btnRemoveItem
            '
            Me.btnRemoveItem.Kind = Global.oopfinals.Controls.ThemeButtonKind.Secondary
            Me.btnRemoveItem.Location = New System.Drawing.Point(228, 74)
            Me.btnRemoveItem.Name = "btnRemoveItem"
            Me.btnRemoveItem.Size = New System.Drawing.Size(112, 32)
            Me.btnRemoveItem.TabIndex = 7
            Me.btnRemoveItem.Text = "Remove line"
            Me.btnRemoveItem.UseVisualStyleBackColor = False
            '
            'btnClearCart
            '
            Me.btnClearCart.Kind = Global.oopfinals.Controls.ThemeButtonKind.Destructive
            Me.btnClearCart.Location = New System.Drawing.Point(346, 74)
            Me.btnClearCart.Name = "btnClearCart"
            Me.btnClearCart.Size = New System.Drawing.Size(110, 32)
            Me.btnClearCart.TabIndex = 8
            Me.btnClearCart.Text = "Clear cart"
            Me.btnClearCart.UseVisualStyleBackColor = False
            '
            'cartGrid
            '
            Me.cartGrid.AllowUserToAddRows = False
            Me.cartGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            Me.cartGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colCartItem, Me.colCartQty, Me.colCartUnit, Me.colCartLine})
            Me.cartGrid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.cartGrid.Location = New System.Drawing.Point(8, 132)
            Me.cartGrid.MultiSelect = False
            Me.cartGrid.Name = "cartGrid"
            Me.cartGrid.ReadOnly = True
            Me.cartGrid.RowHeadersVisible = False
            Me.cartGrid.Size = New System.Drawing.Size(44, 337)
            Me.cartGrid.TabIndex = 0
            '
            'colCartItem
            '
            Me.colCartItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            Me.colCartItem.HeaderText = "Item"
            Me.colCartItem.Name = "colCartItem"
            Me.colCartItem.ReadOnly = True
            '
            'colCartQty
            '
            Me.colCartQty.HeaderText = "Qty"
            Me.colCartQty.Name = "colCartQty"
            Me.colCartQty.ReadOnly = True
            Me.colCartQty.Width = 56
            '
            'colCartUnit
            '
            Me.colCartUnit.HeaderText = "Unit price"
            Me.colCartUnit.Name = "colCartUnit"
            Me.colCartUnit.ReadOnly = True
            Me.colCartUnit.Width = 96
            '
            'colCartLine
            '
            Me.colCartLine.HeaderText = "Line total"
            Me.colCartLine.Name = "colCartLine"
            Me.colCartLine.ReadOnly = True
            Me.colCartLine.Width = 104
            '
            'productsPanel (Dock.Left 380)
            '
            Me.productsPanel.BackColor = System.Drawing.Color.White
            Me.productsPanel.Controls.Add(Me.posGrid)
            Me.productsPanel.Controls.Add(Me.productsHeader)
            Me.productsPanel.Dock = System.Windows.Forms.DockStyle.Left
            Me.productsPanel.Location = New System.Drawing.Point(8, 8)
            Me.productsPanel.Name = "productsPanel"
            Me.productsPanel.Padding = New System.Windows.Forms.Padding(8)
            Me.productsPanel.Size = New System.Drawing.Size(380, 477)
            Me.productsPanel.TabIndex = 1
            '
            'productsHeader
            '
            Me.productsHeader.BackColor = System.Drawing.Color.White
            Me.productsHeader.Controls.Add(Me.lblProductsTitle)
            Me.productsHeader.Controls.Add(Me.lblGridHint)
            Me.productsHeader.Controls.Add(Me.lblSearchCaption)
            Me.productsHeader.Controls.Add(Me.txtSearch)
            Me.productsHeader.Controls.Add(Me.lblCategoryCaption)
            Me.productsHeader.Controls.Add(Me.cmbCategory)
            Me.productsHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.productsHeader.Location = New System.Drawing.Point(8, 8)
            Me.productsHeader.Name = "productsHeader"
            Me.productsHeader.Size = New System.Drawing.Size(364, 116)
            Me.productsHeader.TabIndex = 1
            '
            'lblProductsTitle
            '
            Me.lblProductsTitle.AutoSize = True
            Me.lblProductsTitle.BackColor = System.Drawing.Color.White
            Me.lblProductsTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 11.0!)
            Me.lblProductsTitle.ForeColor = System.Drawing.Color.FromArgb(26, 31, 28)
            Me.lblProductsTitle.Location = New System.Drawing.Point(8, 4)
            Me.lblProductsTitle.Name = "lblProductsTitle"
            Me.lblProductsTitle.Size = New System.Drawing.Size(96, 24)
            Me.lblProductsTitle.TabIndex = 0
            Me.lblProductsTitle.Text = "Products"
            '
            'lblGridHint
            '
            Me.lblGridHint.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblGridHint.BackColor = System.Drawing.Color.White
            Me.lblGridHint.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128)
            Me.lblGridHint.Location = New System.Drawing.Point(12, 30)
            Me.lblGridHint.Name = "lblGridHint"
            Me.lblGridHint.Size = New System.Drawing.Size(344, 16)
            Me.lblGridHint.TabIndex = 1
            Me.lblGridHint.Text = "Double-click a row (or scan) to add one unit to the cart."
            '
            'lblSearchCaption
            '
            Me.lblSearchCaption.AutoSize = True
            Me.lblSearchCaption.BackColor = System.Drawing.Color.White
            Me.lblSearchCaption.Location = New System.Drawing.Point(8, 54)
            Me.lblSearchCaption.Name = "lblSearchCaption"
            Me.lblSearchCaption.Size = New System.Drawing.Size(47, 15)
            Me.lblSearchCaption.TabIndex = 2
            Me.lblSearchCaption.Text = "Search:"
            '
            'txtSearch
            '
            Me.txtSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtSearch.Location = New System.Drawing.Point(66, 50)
            Me.txtSearch.Name = "txtSearch"
            Me.txtSearch.Size = New System.Drawing.Size(290, 23)
            Me.txtSearch.TabIndex = 3
            '
            'lblCategoryCaption
            '
            Me.lblCategoryCaption.AutoSize = True
            Me.lblCategoryCaption.BackColor = System.Drawing.Color.White
            Me.lblCategoryCaption.Location = New System.Drawing.Point(8, 86)
            Me.lblCategoryCaption.Name = "lblCategoryCaption"
            Me.lblCategoryCaption.Size = New System.Drawing.Size(59, 15)
            Me.lblCategoryCaption.TabIndex = 4
            Me.lblCategoryCaption.Text = "Category:"
            '
            'cmbCategory
            '
            Me.cmbCategory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbCategory.FormattingEnabled = True
            Me.cmbCategory.Location = New System.Drawing.Point(78, 82)
            Me.cmbCategory.Name = "cmbCategory"
            Me.cmbCategory.Size = New System.Drawing.Size(278, 23)
            Me.cmbCategory.TabIndex = 5
            '
            'posGrid
            '
            Me.posGrid.AllowUserToAddRows = False
            Me.posGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            Me.posGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colProduct, Me.colPrice, Me.colStock, Me.colStatus})
            Me.posGrid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.posGrid.Location = New System.Drawing.Point(8, 124)
            Me.posGrid.MultiSelect = False
            Me.posGrid.Name = "posGrid"
            Me.posGrid.ReadOnly = True
            Me.posGrid.RowHeadersVisible = False
            Me.posGrid.Size = New System.Drawing.Size(364, 345)
            Me.posGrid.TabIndex = 0
            '
            'colProduct
            '
            Me.colProduct.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            Me.colProduct.HeaderText = "Product"
            Me.colProduct.Name = "colProduct"
            Me.colProduct.ReadOnly = True
            '
            'colPrice
            '
            Me.colPrice.HeaderText = "Price"
            Me.colPrice.Name = "colPrice"
            Me.colPrice.ReadOnly = True
            Me.colPrice.Width = 92
            '
            'colStock
            '
            Me.colStock.HeaderText = "Stock"
            Me.colStock.Name = "colStock"
            Me.colStock.ReadOnly = True
            Me.colStock.Width = 66
            '
            'colStatus
            '
            Me.colStatus.HeaderText = "Status"
            Me.colStatus.Name = "colStatus"
            Me.colStatus.ReadOnly = True
            Me.colStatus.Width = 92
            '
            'paymentRail (Dock.Right 300)
            '
            Me.paymentRail.BackColor = System.Drawing.Color.White
            Me.paymentRail.Controls.Add(Me.lblPaymentTitle)
            Me.paymentRail.Controls.Add(Me.lblDiscountCaption)
            Me.paymentRail.Controls.Add(Me.cmbDiscount)
            Me.paymentRail.Controls.Add(Me.lblCashCaption)
            Me.paymentRail.Controls.Add(Me.txtCash)
            Me.paymentRail.Controls.Add(Me.lblTotalsTitle)
            Me.paymentRail.Controls.Add(Me.lblSubtotalCaption)
            Me.paymentRail.Controls.Add(Me.lblSubtotalValue)
            Me.paymentRail.Controls.Add(Me.lblDiscountTotalCaption)
            Me.paymentRail.Controls.Add(Me.lblDiscountValue)
            Me.paymentRail.Controls.Add(Me.separatorPanel)
            Me.paymentRail.Controls.Add(Me.lblTotalDueCaption)
            Me.paymentRail.Controls.Add(Me.lblTotalValue)
            Me.paymentRail.Controls.Add(Me.lblChangeCaption)
            Me.paymentRail.Controls.Add(Me.lblChangeValue)
            Me.paymentRail.Controls.Add(Me.btnCheckout)
            Me.paymentRail.Controls.Add(Me.lblCheckoutHint)
            Me.paymentRail.Dock = System.Windows.Forms.DockStyle.Right
            Me.paymentRail.Location = New System.Drawing.Point(408, 8)
            Me.paymentRail.Name = "paymentRail"
            Me.paymentRail.Size = New System.Drawing.Size(300, 477)
            Me.paymentRail.TabIndex = 0
            '
            'lblPaymentTitle
            '
            Me.lblPaymentTitle.AutoSize = True
            Me.lblPaymentTitle.BackColor = System.Drawing.Color.White
            Me.lblPaymentTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 11.0!)
            Me.lblPaymentTitle.ForeColor = System.Drawing.Color.FromArgb(26, 31, 28)
            Me.lblPaymentTitle.Location = New System.Drawing.Point(16, 14)
            Me.lblPaymentTitle.Name = "lblPaymentTitle"
            Me.lblPaymentTitle.Size = New System.Drawing.Size(94, 24)
            Me.lblPaymentTitle.TabIndex = 0
            Me.lblPaymentTitle.Text = "Payment"
            '
            'lblDiscountCaption
            '
            Me.lblDiscountCaption.BackColor = System.Drawing.Color.White
            Me.lblDiscountCaption.Location = New System.Drawing.Point(16, 52)
            Me.lblDiscountCaption.Name = "lblDiscountCaption"
            Me.lblDiscountCaption.Size = New System.Drawing.Size(110, 20)
            Me.lblDiscountCaption.TabIndex = 1
            Me.lblDiscountCaption.Text = "Discount:"
            Me.lblDiscountCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'cmbDiscount
            '
            Me.cmbDiscount.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbDiscount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbDiscount.FormattingEnabled = True
            Me.cmbDiscount.Location = New System.Drawing.Point(132, 49)
            Me.cmbDiscount.Name = "cmbDiscount"
            Me.cmbDiscount.Size = New System.Drawing.Size(152, 23)
            Me.cmbDiscount.TabIndex = 2
            '
            'lblCashCaption
            '
            Me.lblCashCaption.BackColor = System.Drawing.Color.White
            Me.lblCashCaption.Location = New System.Drawing.Point(16, 84)
            Me.lblCashCaption.Name = "lblCashCaption"
            Me.lblCashCaption.Size = New System.Drawing.Size(110, 20)
            Me.lblCashCaption.TabIndex = 3
            Me.lblCashCaption.Text = "Cash received:"
            Me.lblCashCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'txtCash
            '
            Me.txtCash.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtCash.Location = New System.Drawing.Point(132, 81)
            Me.txtCash.Name = "txtCash"
            Me.txtCash.Size = New System.Drawing.Size(152, 23)
            Me.txtCash.TabIndex = 4
            '
            'lblTotalsTitle
            '
            Me.lblTotalsTitle.AutoSize = True
            Me.lblTotalsTitle.BackColor = System.Drawing.Color.White
            Me.lblTotalsTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!)
            Me.lblTotalsTitle.ForeColor = System.Drawing.Color.FromArgb(26, 31, 28)
            Me.lblTotalsTitle.Location = New System.Drawing.Point(16, 116)
            Me.lblTotalsTitle.Name = "lblTotalsTitle"
            Me.lblTotalsTitle.Size = New System.Drawing.Size(56, 19)
            Me.lblTotalsTitle.TabIndex = 5
            Me.lblTotalsTitle.Text = "Totals"
            '
            'lblSubtotalCaption
            '
            Me.lblSubtotalCaption.BackColor = System.Drawing.Color.White
            Me.lblSubtotalCaption.Location = New System.Drawing.Point(16, 146)
            Me.lblSubtotalCaption.Name = "lblSubtotalCaption"
            Me.lblSubtotalCaption.Size = New System.Drawing.Size(120, 20)
            Me.lblSubtotalCaption.TabIndex = 6
            Me.lblSubtotalCaption.Text = "Subtotal"
            Me.lblSubtotalCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblSubtotalValue
            '
            Me.lblSubtotalValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblSubtotalValue.AutoSize = True
            Me.lblSubtotalValue.BackColor = System.Drawing.Color.White
            Me.lblSubtotalValue.ForeColor = System.Drawing.Color.FromArgb(26, 31, 28)
            Me.lblSubtotalValue.Location = New System.Drawing.Point(160, 146)
            Me.lblSubtotalValue.Name = "lblSubtotalValue"
            Me.lblSubtotalValue.Size = New System.Drawing.Size(110, 15)
            Me.lblSubtotalValue.TabIndex = 7
            Me.lblSubtotalValue.Text = "PHP 0.00"
            Me.lblSubtotalValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'lblDiscountTotalCaption
            '
            Me.lblDiscountTotalCaption.BackColor = System.Drawing.Color.White
            Me.lblDiscountTotalCaption.Location = New System.Drawing.Point(16, 172)
            Me.lblDiscountTotalCaption.Name = "lblDiscountTotalCaption"
            Me.lblDiscountTotalCaption.Size = New System.Drawing.Size(120, 20)
            Me.lblDiscountTotalCaption.TabIndex = 8
            Me.lblDiscountTotalCaption.Text = "Discount"
            Me.lblDiscountTotalCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblDiscountValue
            '
            Me.lblDiscountValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblDiscountValue.AutoSize = True
            Me.lblDiscountValue.BackColor = System.Drawing.Color.White
            Me.lblDiscountValue.ForeColor = System.Drawing.Color.FromArgb(46, 125, 79)
            Me.lblDiscountValue.Location = New System.Drawing.Point(160, 172)
            Me.lblDiscountValue.Name = "lblDiscountValue"
            Me.lblDiscountValue.Size = New System.Drawing.Size(118, 15)
            Me.lblDiscountValue.TabIndex = 9
            Me.lblDiscountValue.Text = "- PHP 0.00"
            Me.lblDiscountValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'separatorPanel
            '
            Me.separatorPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.separatorPanel.BackColor = System.Drawing.Color.FromArgb(227, 232, 229)
            Me.separatorPanel.Location = New System.Drawing.Point(16, 200)
            Me.separatorPanel.Name = "separatorPanel"
            Me.separatorPanel.Size = New System.Drawing.Size(268, 2)
            Me.separatorPanel.TabIndex = 10
            '
            'lblTotalDueCaption
            '
            Me.lblTotalDueCaption.BackColor = System.Drawing.Color.White
            Me.lblTotalDueCaption.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
            Me.lblTotalDueCaption.Location = New System.Drawing.Point(16, 212)
            Me.lblTotalDueCaption.Name = "lblTotalDueCaption"
            Me.lblTotalDueCaption.Size = New System.Drawing.Size(120, 26)
            Me.lblTotalDueCaption.TabIndex = 11
            Me.lblTotalDueCaption.Text = "Total due"
            Me.lblTotalDueCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblTotalValue
            '
            Me.lblTotalValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblTotalValue.AutoSize = True
            Me.lblTotalValue.BackColor = System.Drawing.Color.White
            Me.lblTotalValue.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
            Me.lblTotalValue.ForeColor = System.Drawing.Color.FromArgb(26, 31, 28)
            Me.lblTotalValue.Location = New System.Drawing.Point(150, 208)
            Me.lblTotalValue.Name = "lblTotalValue"
            Me.lblTotalValue.Size = New System.Drawing.Size(100, 24)
            Me.lblTotalValue.TabIndex = 12
            Me.lblTotalValue.Text = "PHP 0.00"
            Me.lblTotalValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'lblChangeCaption
            '
            Me.lblChangeCaption.BackColor = System.Drawing.Color.White
            Me.lblChangeCaption.Location = New System.Drawing.Point(16, 248)
            Me.lblChangeCaption.Name = "lblChangeCaption"
            Me.lblChangeCaption.Size = New System.Drawing.Size(150, 20)
            Me.lblChangeCaption.TabIndex = 13
            Me.lblChangeCaption.Text = "Change / status"
            Me.lblChangeCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblChangeValue
            '
            Me.lblChangeValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblChangeValue.AutoEllipsis = True
            Me.lblChangeValue.BackColor = System.Drawing.Color.White
            Me.lblChangeValue.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128)
            Me.lblChangeValue.Location = New System.Drawing.Point(16, 272)
            Me.lblChangeValue.Name = "lblChangeValue"
            Me.lblChangeValue.Size = New System.Drawing.Size(268, 34)
            Me.lblChangeValue.TabIndex = 14
            Me.lblChangeValue.Text = "—"
            Me.lblChangeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'btnCheckout
            '
            Me.btnCheckout.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCheckout.Enabled = False
            Me.btnCheckout.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
            Me.btnCheckout.Location = New System.Drawing.Point(16, 322)
            Me.btnCheckout.Name = "btnCheckout"
            Me.btnCheckout.Size = New System.Drawing.Size(268, 46)
            Me.btnCheckout.TabIndex = 15
            Me.btnCheckout.Text = "Checkout"
            Me.btnCheckout.UseVisualStyleBackColor = False
            '
            'lblCheckoutHint
            '
            Me.lblCheckoutHint.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblCheckoutHint.BackColor = System.Drawing.Color.White
            Me.lblCheckoutHint.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128)
            Me.lblCheckoutHint.Location = New System.Drawing.Point(16, 376)
            Me.lblCheckoutHint.Name = "lblCheckoutHint"
            Me.lblCheckoutHint.Size = New System.Drawing.Size(268, 60)
            Me.lblCheckoutHint.TabIndex = 16
            Me.lblCheckoutHint.Text = "Cash must cover the total due (≤ PHP 999,999.99). F12 = Checkout; Enter in the scan row adds items. Every sale writes one atomic receipt — stock, audit trail and Today all update on success."
            '
            'CashierDashboardForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(984, 661)
            Me.Controls.Add(Me.contentCard)
            Me.Controls.Add(Me.tabRail)
            Me.Controls.Add(Me.appHeader)
            Me.Name = "CashierDashboardForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Cashier Dashboard"
            Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
            CType(Me.permissionsGrid, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.cartGrid, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.posGrid, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.todayGrid, System.ComponentModel.ISupportInitialize).EndInit()
            Me.permissionsView.ResumeLayout(False)
            Me.todayView.ResumeLayout(False)
            Me.todayHeader.ResumeLayout(False)
            Me.todayHeader.PerformLayout()
            Me.posContainer.ResumeLayout(False)
            Me.cartPanel.ResumeLayout(False)
            Me.cartHeader.ResumeLayout(False)
            Me.cartHeader.PerformLayout()
            Me.productsPanel.ResumeLayout(False)
            Me.productsHeader.ResumeLayout(False)
            Me.productsHeader.PerformLayout()
            Me.paymentRail.ResumeLayout(False)
            Me.paymentRail.PerformLayout()
            Me.viewHost.ResumeLayout(False)
            Me.contentCard.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents appHeader As Global.oopfinals.Controls.AppHeader
        Friend WithEvents btnLogout As Global.oopfinals.Controls.StyledButton
        Friend WithEvents tabRail As Global.oopfinals.Controls.IconTabBar
        Friend WithEvents contentCard As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents pageHeading As Global.oopfinals.Controls.PageHeading
        Friend WithEvents viewHost As System.Windows.Forms.Panel
        Friend WithEvents permissionsView As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents permissionsGrid As System.Windows.Forms.DataGridView
        Friend WithEvents colPermission As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents todayView As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents todayGrid As System.Windows.Forms.DataGridView
        Friend WithEvents colTodayReceipt As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colTodayTime As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colTodayCashier As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colTodayTotal As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents todayHeader As System.Windows.Forms.Panel
        Friend WithEvents lblTodayTitle As System.Windows.Forms.Label
        Friend WithEvents lblTodayCount As System.Windows.Forms.Label
        Friend WithEvents btnTodayDetails As Global.oopfinals.Controls.StyledButton
        Friend WithEvents btnTodayRefresh As Global.oopfinals.Controls.StyledButton
        Friend WithEvents posContainer As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents cartPanel As System.Windows.Forms.Panel
        Friend WithEvents cartHeader As System.Windows.Forms.Panel
        Friend WithEvents lblCartTitle As System.Windows.Forms.Label
        Friend WithEvents lblCartCount As System.Windows.Forms.Label
        Friend WithEvents lblScanCaption As System.Windows.Forms.Label
        Friend WithEvents txtScan As System.Windows.Forms.TextBox
        Friend WithEvents btnScanAdd As Global.oopfinals.Controls.StyledButton
        Friend WithEvents btnQtyMinus As Global.oopfinals.Controls.StyledButton
        Friend WithEvents btnQtyPlus As Global.oopfinals.Controls.StyledButton
        Friend WithEvents btnRemoveItem As Global.oopfinals.Controls.StyledButton
        Friend WithEvents btnClearCart As Global.oopfinals.Controls.StyledButton
        Friend WithEvents cartGrid As System.Windows.Forms.DataGridView
        Friend WithEvents colCartItem As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colCartQty As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colCartUnit As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colCartLine As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents productsPanel As System.Windows.Forms.Panel
        Friend WithEvents productsHeader As System.Windows.Forms.Panel
        Friend WithEvents lblProductsTitle As System.Windows.Forms.Label
        Friend WithEvents lblGridHint As System.Windows.Forms.Label
        Friend WithEvents lblSearchCaption As System.Windows.Forms.Label
        Friend WithEvents txtSearch As System.Windows.Forms.TextBox
        Friend WithEvents lblCategoryCaption As System.Windows.Forms.Label
        Friend WithEvents cmbCategory As System.Windows.Forms.ComboBox
        Friend WithEvents posGrid As System.Windows.Forms.DataGridView
        Friend WithEvents colProduct As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colPrice As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colStock As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colStatus As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents paymentRail As System.Windows.Forms.Panel
        Friend WithEvents lblPaymentTitle As System.Windows.Forms.Label
        Friend WithEvents lblDiscountCaption As System.Windows.Forms.Label
        Friend WithEvents cmbDiscount As System.Windows.Forms.ComboBox
        Friend WithEvents lblCashCaption As System.Windows.Forms.Label
        Friend WithEvents txtCash As System.Windows.Forms.TextBox
        Friend WithEvents lblTotalsTitle As System.Windows.Forms.Label
        Friend WithEvents lblSubtotalCaption As System.Windows.Forms.Label
        Friend WithEvents lblSubtotalValue As System.Windows.Forms.Label
        Friend WithEvents lblDiscountTotalCaption As System.Windows.Forms.Label
        Friend WithEvents lblDiscountValue As System.Windows.Forms.Label
        Friend WithEvents separatorPanel As System.Windows.Forms.Panel
        Friend WithEvents lblTotalDueCaption As System.Windows.Forms.Label
        Friend WithEvents lblTotalValue As System.Windows.Forms.Label
        Friend WithEvents lblChangeCaption As System.Windows.Forms.Label
        Friend WithEvents lblChangeValue As System.Windows.Forms.Label
        Friend WithEvents btnCheckout As Global.oopfinals.Controls.StyledButton
        Friend WithEvents lblCheckoutHint As System.Windows.Forms.Label

    End Class

End Namespace
