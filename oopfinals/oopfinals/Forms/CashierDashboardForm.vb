Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Forms/CashierDashboardForm.vb — Cashier dashboard: POS stage 1+2
' (M9 catalog/cart/strategy work landed in M9; M10 completes the
' money path: checkout, receipt trio and the Today tab).
'
' What runs TODAY (since M10):
'   • Everything from M9 (Products card, Cart card, live strategy
'     swap, live tender math) — see the M9 notes below.
'   • CHECKOUT (the money path, spec §12.3): guards run BEFORE any
'     SQL (empty cart, non-positive total, cash parse/limits), the
'     cashier CONFIRMS, then SaleRepository.RecordSale writes sale +
'     items + guarded stock decrements + audit movements in ONE
'     transaction. ReceiptPreviewForm shows the receipt; when it
'     closes the cart clears, the payment UI resets and POS stock +
'     Today refresh. _isCheckingOut makes a double-click unable to
'     fire two sales. F12 is the checkout shortcut (scan-row Enter
'     adds items — the M9 Enter shortcut).
'   • TODAY tab — the day's receipts (SaleRepository.GetSalesByDate)
'     with View details opening PastReceiptForm (reprint included).
'
' What stayed OFF in M9 — now enabled: btnCheckout wiring (this file
' owns the Enabled gate flipped inside UpdateCheckoutTotals).
'
' OOP concept: ENCAPSULATION — the form binds UI events to Services/
' Data only (spec §3); money math lives in CartService, SQL in the
' repositories; this file holds layout glue and messages.
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports oopfinals.Controls
Imports oopfinals.Data
Imports oopfinals.Models
Imports oopfinals.Services

Namespace Forms

    Partial Public Class CashierDashboardForm

        Private ReadOnly _cashier As Cashier
        Private ReadOnly _cart As New CartService()

        ' Snapshot of every ACTIVE product. Scan lookups must see the
        ' FULL catalog even when a search/filter hides grid rows. M10
        ' re-refreshes after each sale (stock decrements) — noted there.
        Private ReadOnly _catalogCache As New Dictionary(Of Integer, Product)()
        Private ReadOnly _catalogByCode As New Dictionary(Of String, Product)()

        ' Re-entrancy guards: _isInitializing keeps combo autofills from
        ' firing handlers during construction; _isRefreshingCart keeps
        ' SelectionChanged from chasing its own rebuild; _isCheckingOut
        ' (M10) makes a double-click on Checkout fire exactly ONE sale
        ' even while the confirmation dialog / preview is still on
        ' screen (spec §12.3 re-entry guard).
        Private _isInitializing As Boolean = True
        Private _isRefreshingCart As Boolean = False
        Private _isCheckingOut As Boolean = False
        Private _selectedCartProductId As Integer = 0

        ' Optional constructor keeps the WinForms designer usable
        ' (runtime always passes the strongly-typed cashier — M4 shell).
        Public Sub New(Optional cashier As Cashier = Nothing)
            MyBase.New()
            InitializeComponent()

            StyledGrid.Apply(Me.permissionsGrid)
            StyledGrid.Apply(Me.posGrid)
            StyledGrid.Apply(Me.cartGrid)
            StyledGrid.Apply(Me.todayGrid)
            StyledInputs.StyleTextBox(Me.txtSearch)
            StyledInputs.StyleTextBox(Me.txtScan)
            StyledInputs.StyleTextBox(Me.txtCash)
            StyledInputs.StyleComboBox(Me.cmbCategory)
            StyledInputs.StyleComboBox(Me.cmbDiscount)

            _cashier = cashier

            _isInitializing = True
            LoadDiscountStrategies()
            LoadCategoryFilter()
            _isInitializing = False

            Me.tabRail.AddTab("pos", "POS", "pos")
            Me.tabRail.AddTab("today", "Today", "today")
            Me.tabRail.AddTab("permissions", "Permissions", "permissions")
            Me.tabRail.SelectedKey = "pos"

            If _cashier IsNot Nothing Then
                ApplyUser()
                LoadPosProducts()
                RefreshCartView()
                UpdateCheckoutTotals()
            End If
            ApplyRestyle()
        End Sub

#Region "User binding + tab shell (M4 contract kept)"

        Private Sub ApplyUser()
            If _cashier Is Nothing Then
                Return   ' designer-time shell — no live user bound
            End If
            Me.appHeader.Caption = _cashier.ToString()
            ApplyHeadingForTab(Me.tabRail.SelectedKey)
        End Sub

        Private Sub ApplyHeadingForTab(tabKey As String)
            If tabKey = "permissions" Then
                Me.pageHeading.Text = "Permissions"
                If _cashier IsNot Nothing Then
                    Me.pageHeading.Subtitle = _cashier.GetPermissions().Count.ToString() &
                        " permission keys granted — " & _cashier.GetRoleName() &
                        " role (spec §6.2/§11)"
                End If
            ElseIf tabKey = "today" Then
                Me.pageHeading.Text = "Today"
                Me.pageHeading.Subtitle = "Today's completed transactions — double-click a row (or use View details) to reopen the receipt"
            Else
                Me.pageHeading.Text = "POS"
                If _cashier IsNot Nothing Then
                    Me.pageHeading.Subtitle = "Search, scan and check out the cart — every sale writes one atomic receipt (F12 = checkout)"
                End If
            End If
        End Sub

        Private Sub FillPermissionsGrid()
            If _cashier Is Nothing Then
                Return
            End If
            Me.permissionsGrid.Rows.Clear()
            For Each key As String In _cashier.GetPermissions()
                Me.permissionsGrid.Rows.Add(key)
            Next
        End Sub

        Private Sub tabRail_SelectionChanged(key As String) Handles tabRail.SelectionChanged
            Dim onPermissions As Boolean = (key = "permissions")
            Dim onToday As Boolean = (Not onPermissions) AndAlso (key = "today")
            Me.posContainer.Visible = Not (onPermissions OrElse onToday)
            Me.todayView.Visible = onToday
            Me.permissionsView.Visible = onPermissions

            If onToday Then
                LoadTodayTransactions()   ' fresh list on every visit
            End If
            ApplyHeadingForTab(key)
        End Sub

        Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
            Me.Close()
        End Sub

#End Region

#Region "Combo population (strategies + category filter)"

        ' The Strategy family, ready to be INSTALLED: the combo stores
        ' base-type (Discount) references and renders each via its
        ' ToString() — the spec labels "None (0%)", "Regular / Promo
        ' (5%)", "Senior Citizen / PWD (20%)".
        Private Sub LoadDiscountStrategies()
            Me.cmbDiscount.Items.Clear()
            Me.cmbDiscount.Items.Add(New NoDiscount())
            Me.cmbDiscount.Items.Add(New RegularDiscount())
            Me.cmbDiscount.Items.Add(New SeniorPwdDiscount())
            Me.cmbDiscount.SelectedIndex = 0   ' NoDiscount is the cart default
        End Sub

        Private Sub LoadCategoryFilter()
            Me.cmbCategory.Items.Clear()
            Me.cmbCategory.Items.Add(New FilterChoice(0, "All Categories"))
            If _cashier Is Nothing Then
                Return   ' designer preview — no DB round-trip
            End If
            Try
                For Each category As Category In CategoryRepository.GetAll()
                    Me.cmbCategory.Items.Add(New FilterChoice(category.CategoryId, category.Name))
                Next
            Catch dbError As Exception
                ShowDataError("Could not load the product categories.", dbError)
            End Try
            Me.cmbCategory.SelectedIndex = 0
        End Sub

        ' 0 = All categories (the sentinel row).
        Private Function SelectedCategoryId() As Integer
            Dim choice As FilterChoice = TryCast(Me.cmbCategory.SelectedItem, FilterChoice)
            If choice Is Nothing Then
                Return 0
            End If
            Return choice.CategoryId
        End Function

#End Region

#Region "Products card (search / filter / scan source of truth)"

        Private Sub LoadPosProducts()
            If _cashier Is Nothing Then
                Return
            End If
            Try
                ' Scan lookups always see the FULL active catalog…
                RebuildCatalogCache(ProductRepository.GetAll(True))
                ' …while the grid shows the live search + category filter.
                PopulateProductsGrid(ProductRepository.Search(Me.txtSearch.Text, SelectedCategoryId(), True))
            Catch dbError As Exception
                ShowDataError("Could not load the product catalog.", dbError)
            End Try
        End Sub

        Private Sub RebuildCatalogCache(products As List(Of Product))
            _catalogCache.Clear()
            _catalogByCode.Clear()
            For Each product As Product In products
                _catalogCache(product.ProductId) = product
                _catalogByCode(product.ProductCode) = product
            Next
        End Sub

        Private Sub PopulateProductsGrid(products As List(Of Product))
            Me.posGrid.Rows.Clear()
            For Each product As Product In products
                Dim rowIndex As Integer = Me.posGrid.Rows.Add()
                Dim row As DataGridViewRow = Me.posGrid.Rows(rowIndex)
                row.Cells(0).Value = product.ToString()   ' "PRD001 - ... (PHP 280.00)"
                row.Cells(1).Value = String.Format("PHP {0:N2}", product.Price)
                row.Cells(2).Value = product.StockQty
                row.Cells(3).Value = product.GetStockStatus()
                row.Tag = product.ProductId
            Next
        End Sub

        Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
            If _isInitializing OrElse _cashier Is Nothing Then
                Return
            End If
            LoadPosProducts()
        End Sub

        Private Sub cmbCategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCategory.SelectedIndexChanged
            If _isInitializing OrElse _cashier Is Nothing Then
                Return
            End If
            LoadPosProducts()
        End Sub

        Private Sub posGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles posGrid.CellDoubleClick
            If e.RowIndex < 0 Then
                Return
            End If
            Dim productId As Integer = RowProductId(Me.posGrid.Rows(e.RowIndex))
            If productId = 0 Then
                Return
            End If
            AddProductToCart(productId, 1)
        End Sub

#End Region

#Region "Cart card (all mutations flow through CartService)"

        Private Sub RefreshCartView()
            _isRefreshingCart = True
            Me.cartGrid.Rows.Clear()
            For Each line As SaleItem In _cart.Items
                Dim rowIndex As Integer = Me.cartGrid.Rows.Add()
                Dim row As DataGridViewRow = Me.cartGrid.Rows(rowIndex)
                row.Cells(0).Value = String.Format("{0} · {1}", line.ProductCode, line.ProductName)
                row.Cells(1).Value = line.Quantity
                row.Cells(2).Value = String.Format("PHP {0:N2}", line.UnitPrice)
                row.Cells(3).Value = String.Format("PHP {0:N2}", line.LineTotal)
                row.Tag = line.ProductId
                If line.ProductId = _selectedCartProductId Then
                    row.Selected = True
                End If
            Next
            _isRefreshingCart = False
            Me.lblCartCount.Text = String.Format("{0} line(s) · {1} unit(s)",
                                                 _cart.Items.Count, _cart.GetTotalItemCount())
        End Sub

        Private Sub cartGrid_SelectionChanged(sender As Object, e As EventArgs) Handles cartGrid.SelectionChanged
            If _isRefreshingCart Then
                Return
            End If
            _selectedCartProductId = SelectedCartProductId()
        End Sub

        Private Sub AddProductToCart(productId As Integer, qty As Integer)
            Dim product As Product = Nothing
            If Not _catalogCache.TryGetValue(productId, product) Then
                MessageBox.Show(Me,
                                "This product is no longer in the active catalog. Refresh and try again.",
                                "Product unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Try
                _cart.AddItem(product, qty)          ' combined stock check inside
                _selectedCartProductId = productId
                RefreshCartView()
                UpdateCheckoutTotals()
            Catch stockError As InvalidOperationException
                ' Over-stock: the message carries the available figure.
                MessageBox.Show(Me, stockError.Message, "Product not added",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch ruleError As ArgumentException
                MessageBox.Show(Me, ruleError.Message, "Invalid quantity",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub

        Private Sub AddScannedItem()
            If _cashier Is Nothing Then
                Return
            End If
            Dim code As String = Me.txtScan.Text.Trim().ToUpperInvariant()
            If code.Length = 0 Then
                MessageBox.Show(Me, "Type or scan a product code first.",
                                "Nothing to add", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim product As Product = Nothing
            If Not _catalogByCode.TryGetValue(code, product) Then
                MessageBox.Show(Me,
                                String.Format("No active product with code '{0}' was found in the catalog.", code),
                                "Product not found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            AddProductToCart(product.ProductId, 1)
            Me.txtScan.Clear()
            Me.txtScan.Focus()
        End Sub

        Private Sub btnScanAdd_Click(sender As Object, e As EventArgs) Handles btnScanAdd.Click
            AddScannedItem()
        End Sub

        Private Sub txtScan_KeyDown(sender As Object, e As KeyEventArgs) Handles txtScan.KeyDown
            If e.KeyCode = Keys.Enter Then
                e.SuppressKeyPress = True
                AddScannedItem()
            End If
        End Sub

        Private Sub btnQtyPlus_Click(sender As Object, e As EventArgs) Handles btnQtyPlus.Click
            AdjustSelectedQuantity(1)
        End Sub

        Private Sub btnQtyMinus_Click(sender As Object, e As EventArgs) Handles btnQtyMinus.Click
            AdjustSelectedQuantity(-1)
        End Sub

        ' +1/−1 on the selected line; UpdateItemQuantity removes the
        ' line when the result reaches 0 (spec §9.3) and re-checks
        ' stock against the product's CURRENT snapshot.
        Private Sub AdjustSelectedQuantity(delta As Integer)
            Dim productId As Integer = SelectedCartProductId()
            If productId = 0 Then
                MessageBox.Show(Me, "Select a cart line first.",
                                "No line selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim currentQty As Integer = 0
            For Each line As SaleItem In _cart.Items
                If line.ProductId = productId Then
                    currentQty = line.Quantity
                    Exit For
                End If
            Next

            Try
                _cart.UpdateItemQuantity(productId, currentQty + delta, CachedStockFor(productId))
                RefreshCartView()
                UpdateCheckoutTotals()
            Catch stockError As InvalidOperationException
                MessageBox.Show(Me, stockError.Message, "Quantity not changed",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch ruleError As ArgumentException
                MessageBox.Show(Me, ruleError.Message, "Invalid quantity",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub

        Private Sub btnRemoveItem_Click(sender As Object, e As EventArgs) Handles btnRemoveItem.Click
            Dim productId As Integer = SelectedCartProductId()
            If productId = 0 Then
                MessageBox.Show(Me, "Select a cart line first.",
                                "No line selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            _cart.RemoveItem(productId)
            _selectedCartProductId = 0
            RefreshCartView()
            UpdateCheckoutTotals()
        End Sub

        Private Sub btnClearCart_Click(sender As Object, e As EventArgs) Handles btnClearCart.Click
            If _cart.Items.Count = 0 Then
                Return
            End If
            _cart.ClearCart()   ' resets items AND the discount strategy
            _selectedCartProductId = 0
            If Me.cmbDiscount.SelectedIndex <> 0 Then
                Me.cmbDiscount.SelectedIndex = 0   ' handler re-installs NoDiscount
            End If
            RefreshCartView()
            UpdateCheckoutTotals()
        End Sub

        Private Function SelectedCartProductId() As Integer
            If Me.cartGrid.CurrentRow Is Nothing Then
                Return 0
            End If
            Return RowProductId(Me.cartGrid.CurrentRow)
        End Function

        Private Function CachedStockFor(productId As Integer) As Integer
            Dim product As Product = Nothing
            If _catalogCache.TryGetValue(productId, product) Then
                Return product.StockQty
            End If
            Return 0
        End Function

#End Region

#Region "Payment rail (strategy swap + live tender math)"

        ' One totals routine: reads the cart ONCE and repaints every
        ' money label. Money labels are AutoSize (spec §13 UI rules).
        Private Sub UpdateCheckoutTotals()
            Me.lblSubtotalValue.Text = String.Format("PHP {0:N2}", _cart.GetSubtotal())
            Me.lblDiscountValue.Text = String.Format("- PHP {0:N2}", _cart.GetDiscountAmount())
            Me.lblTotalValue.Text = String.Format("PHP {0:N2}", _cart.GetNetTotal())

            ' Change line — live tender feedback. Short cash surfaces
            ' the CartService message VERBATIM (spec §9.3 wording).
            Me.lblChangeValue.Text = "—"
            Me.lblChangeValue.ForeColor = Theme.Muted
            Dim cashText As String = Me.txtCash.Text.Trim()
            Dim cash As Decimal
            If cashText.Length > 0 AndAlso Decimal.TryParse(cashText, cash) Then
                Try
                    Dim change As Decimal = _cart.ComputeChange(cash)
                    Me.lblChangeValue.Text = String.Format("PHP {0:N2}", change)
                    Me.lblChangeValue.ForeColor = Theme.SuccessGreen
                Catch tenderError As InvalidOperationException
                    Me.lblChangeValue.Text = tenderError.Message
                    Me.lblChangeValue.ForeColor = Theme.ErrorRed
                End Try
            End If

            ' M10 gate — the checkout button enables ONLY when every
            ' pre-SQL guard would pass: cart non-empty, positive total,
            ' a live user, no checkout already in flight, and a parsed
            ' cash value inside the §12.3 limits (0 ≤ cash ≤
            ' ₱999,999.99, ≥ total due). Everything else stays grey.
            Dim canCheckout As Boolean = False
            If _cart.Items.Count > 0 AndAlso
               _cart.GetNetTotal() > 0D AndAlso
               _cashier IsNot Nothing AndAlso
               Not _isCheckingOut Then

                Dim cashValue As Decimal
                If Decimal.TryParse(Me.txtCash.Text.Trim(), cashValue) Then
                    canCheckout = (cashValue >= _cart.GetNetTotal()) AndAlso
                                  (cashValue <= ValidationRules.MaxMoney)
                End If
            End If
            Me.btnCheckout.Enabled = canCheckout
        End Sub

        ' POLYMORPHISM #2 (spec §5.2): the selected combo item IS a
        ' Discount subclass; installing it into the single strategy
        ' slot makes the NEXT GetDiscountAmount() answer through a
        ' different overridden Apply() — no If/Else anywhere.
        Private Sub cmbDiscount_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbDiscount.SelectedIndexChanged
            If _isInitializing Then
                Return
            End If
            Dim selected As Discount = TryCast(Me.cmbDiscount.SelectedItem, Discount)
            If selected Is Nothing Then
                Return
            End If
            _cart.CurrentDiscount = selected
            UpdateCheckoutTotals()
        End Sub

        Private Sub txtCash_TextChanged(sender As Object, e As EventArgs) Handles txtCash.TextChanged
            If _isInitializing Then
                Return
            End If
            UpdateCheckoutTotals()
        End Sub

        ' Cash box: digits + one decimal point only (money input).
        Private Sub txtCash_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtCash.KeyPress
            If Char.IsControl(e.KeyChar) Then
                Return
            End If
            If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then
                e.Handled = True
            End If
        End Sub

#End Region

#Region "Checkout (the money path — spec §12.3)"

        ' F12 = checkout shortcut (POS tab only). Scan-row Enter (add
        ' item) is the M9 Enter shortcut and lives with txtScan above.
        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
            If keyData = Keys.F12 AndAlso
               Me.tabRail.SelectedKey = "pos" AndAlso
               Me.btnCheckout.Enabled AndAlso
               Not _isCheckingOut Then
                btnCheckout.PerformClick()
                Return True
            End If
            Return MyBase.ProcessCmdKey(msg, keyData)
        End Function

        Private Sub btnCheckout_Click(sender As Object, e As EventArgs) Handles btnCheckout.Click
            If _isCheckingOut OrElse _cashier Is Nothing Then
                Return
            End If

            ' ① Guards BEFORE any SQL runs (project rule): empty cart and
            '    a total of 0 or less can never reach the database.
            If _cart.Items.Count = 0 Then
                MessageBox.Show(Me, "The cart is empty. Add products before checking out.",
                                "Nothing to sell", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim netTotal As Decimal = _cart.GetNetTotal()
            If netTotal <= 0D Then
                MessageBox.Show(Me, "The cart total is PHP 0.00 — nothing to sell.",
                                "Nothing to sell", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            ' ② Tender validation against the §12.3 limits; the bare
            '    insufficient-cash wording comes verbatim from CartService.
            Dim cashText As String = Me.txtCash.Text.Trim()
            Dim cash As Decimal
            If Not Decimal.TryParse(cashText, cash) Then
                MessageBox.Show(Me, "Enter the cash amount received before checking out.",
                                "Cash required", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.txtCash.Focus()
                Return
            End If
            If cash > ValidationRules.MaxMoney Then
                MessageBox.Show(Me,
                                String.Format("Cash received cannot exceed PHP {0:N2}.",
                                              ValidationRules.MaxMoney),
                                "Cash too large", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim change As Decimal
            Try
                change = _cart.ComputeChange(cash)   ' throws exactly when cash is short
            Catch tenderError As InvalidOperationException
                MessageBox.Show(Me, tenderError.Message, "Checkout not possible",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Catch tenderRule As ArgumentException
                MessageBox.Show(Me, tenderRule.Message, "Invalid cash amount",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End Try

            ' ③ The cashier confirms BEFORE the transaction starts.
            Dim confirmation As DialogResult = MessageBox.Show(Me,
                String.Format("Proceed with this sale?" & Environment.NewLine & Environment.NewLine &
                              "Total due:       PHP {0:N2}" & Environment.NewLine &
                              "Cash received:   PHP {1:N2}" & Environment.NewLine &
                              "Change:          PHP {2:N2}",
                              netTotal, cash, change),
                "Confirm checkout", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If confirmation <> DialogResult.Yes Then
                Return
            End If

            ' ④ Execute — one atomic transaction in the repository; the
            '    receipt preview shows the recorded document; the cart
            '    clears only when the preview closes.
            _isCheckingOut = True
            Try
                Dim receiptNo As String = SaleRepository.GenerateReceiptNumber()

                Dim sale As New Sale(receiptNo, _cashier.UserId, _cart.GetSubtotal(), netTotal)
                sale.SaleDate = DateTime.Now
                sale.CashierName = _cashier.FullName
                sale.CustomerId = Nothing              ' POS sales are walk-in (M12 adds buy-as-member flows)
                sale.CustomerName = String.Empty
                sale.DiscountType = _cart.GetDiscountName()
                sale.DiscountAmount = _cart.GetDiscountAmount()
                sale.CashReceived = cash
                sale.ChangeAmount = change
                ' Frozen receipt copy for preview + printing — the cart
                ' keeps mutating afterwards; the Sale must not.
                sale.Items = New List(Of SaleItem)(_cart.Items)

                SaleRepository.RecordSale(sale, _cart.Items)

                Using preview As New ReceiptPreviewForm(sale)
                    preview.ShowDialog(Me)
                End Using

                ' ⑤ Post-sale (sequence §12.3): reset payment UI + POS
                '    stock numbers + Today list.
                ResetPaymentUi()
                LoadPosProducts()
                If Me.todayView.Visible Then
                    LoadTodayTransactions()
                End If
            Catch saleError As InvalidOperationException
                ' Over-stock race / canceled transaction message from the
                ' repository — verbatim, with "zero rows were written".
                MessageBox.Show(Me, saleError.Message, "Sale not completed",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch dbError As Exception
                ShowDataError("The sale could not be completed. No receipt was created — nothing was saved.", dbError)
            Finally
                _isCheckingOut = False
                RefreshCartView()
                UpdateCheckoutTotals()
            End Try
        End Sub

        ' Post-success state: empty cart + NoDiscount back in the
        ' strategy slot + cash box cleared + change line reset.
        Private Sub ResetPaymentUi()
            _cart.ClearCart()   ' items AND the discount strategy reset
            _selectedCartProductId = 0

            Me.txtCash.Clear()
            If Me.cmbDiscount.SelectedIndex <> 0 Then
                Me.cmbDiscount.SelectedIndex = 0   ' handler re-installs NoDiscount
            End If
            Me.lblChangeValue.Text = "—"
            Me.lblChangeValue.ForeColor = Theme.Muted

            RefreshCartView()
            UpdateCheckoutTotals()
        End Sub

#End Region

#Region "Today tab (completed transactions + details)"

        Private Sub LoadTodayTransactions()
            If _cashier Is Nothing Then
                Return
            End If

            Try
                Dim sales As List(Of Sale) = SaleRepository.GetSalesByDate(DateTime.Today)
                Me.todayGrid.Rows.Clear()
                For Each sale As Sale In sales
                    Dim rowIndex As Integer = Me.todayGrid.Rows.Add()
                    Dim row As DataGridViewRow = Me.todayGrid.Rows(rowIndex)
                    row.Cells(0).Value = sale.ReceiptNo
                    row.Cells(1).Value = sale.SaleDate.ToString("HH:mm:ss")
                    row.Cells(2).Value = If(String.IsNullOrWhiteSpace(sale.CashierName), "Cashier", sale.CashierName)
                    row.Cells(3).Value = String.Format("PHP {0:N2}", sale.Total)
                    row.Tag = sale.SaleId
                Next

                Me.lblTodayCount.Text = String.Format("{0} receipt(s) today", sales.Count)
            Catch dbError As Exception
                ShowDataError("Could not load today's transactions.", dbError)
            End Try

            todayGrid_SelectionChanged(Me, EventArgs.Empty)
        End Sub

        ' Selection governs whether "View details" can fire.
        Private Sub todayGrid_SelectionChanged(sender As Object, e As EventArgs) Handles todayGrid.SelectionChanged
            Me.btnTodayDetails.Enabled = SelectedTodaySaleId() <> 0
        End Sub

        Private Sub todayGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles todayGrid.CellDoubleClick
            If e.RowIndex < 0 Then
                Return
            End If
            OpenPastReceiptForSelection()
        End Sub

        Private Sub btnTodayDetails_Click(sender As Object, e As EventArgs) Handles btnTodayDetails.Click
            OpenPastReceiptForSelection()
        End Sub

        Private Sub btnTodayRefresh_Click(sender As Object, e As EventArgs) Handles btnTodayRefresh.Click
            LoadTodayTransactions()
        End Sub

        ' Row Tag carries the SaleId; GetById re-hydrates the frozen
        ' document and PastReceiptForm (shared by Owner/M15 and
        ' Customer/M12) shows + reprints it.
        Private Sub OpenPastReceiptForSelection()
            Dim saleId As Integer = SelectedTodaySaleId()
            If saleId = 0 Then
                MessageBox.Show(Me, "Select a receipt in the list first.",
                                "No receipt selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Try
                Dim sale As Sale = SaleRepository.GetById(saleId)
                If sale Is Nothing Then
                    MessageBox.Show(Me, "That receipt no longer exists in the database.",
                                    "Not found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    LoadTodayTransactions()
                    Return
                End If

                Using details As New PastReceiptForm(sale)
                    details.ShowDialog(Me)
                End Using
            Catch dbError As Exception
                ShowDataError("Could not open the receipt details.", dbError)
            End Try
        End Sub

        Private Function SelectedTodaySaleId() As Integer
            If Me.todayGrid.CurrentRow Is Nothing Then
                Return 0
            End If
            Return RowProductId(Me.todayGrid.CurrentRow)
        End Function

#End Region

#Region "Shared helpers"

        Private Shared Function RowProductId(row As DataGridViewRow) As Integer
            If row Is Nothing OrElse row.Tag Is Nothing Then
                Return 0
            End If
            Return Convert.ToInt32(row.Tag)
        End Function

        ' Data-load failures surface with context, never swallowed.
        Private Sub ShowDataError(context As String, dbError As Exception)
            MessageBox.Show(Me,
                            context & Environment.NewLine & dbError.Message,
                            "Data error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        ' Purely visual, token consumption only — no ad-hoc colors.
        Private Sub ApplyRestyle()
            Me.BackColor = Theme.PageBg

            Me.lblCartTitle.Font = Theme.FontFor(ThemeWeight.Semibold, 11.0F)
            Me.lblCartTitle.ForeColor = Theme.Ink
            Me.lblCartCount.ForeColor = Theme.Muted
            Me.lblGridHint.ForeColor = Theme.Muted
            Me.lblGridHint.Font = Theme.FontFor(ThemeWeight.Regular, 8.0F)

            Me.lblPaymentTitle.Font = Theme.FontFor(ThemeWeight.Semibold, 11.0F)
            Me.lblPaymentTitle.ForeColor = Theme.Ink
            Me.lblCheckoutHint.ForeColor = Theme.Muted
            Me.lblCheckoutHint.Font = Theme.FontFor(ThemeWeight.Regular, 8.0F)

            For Each caption As Label In New Label() {
                Me.lblDiscountCaption, Me.lblCashCaption, Me.lblSubtotalCaption,
                Me.lblDiscountTotalCaption, Me.lblChangeCaption}
                caption.ForeColor = Theme.Muted
            Next

            Me.lblTotalDueCaption.ForeColor = Theme.Ink
            Me.lblSubtotalValue.ForeColor = Theme.Ink
            Me.lblDiscountValue.ForeColor = Theme.Green
            Me.lblTotalDueCaption.Font = Theme.FontFor(ThemeWeight.Semibold, 9.0F)
            Me.lblTotalValue.ForeColor = Theme.Ink
            Me.lblTotalValue.Font = Theme.FontFor(ThemeWeight.Bold, 11.0F)
            Me.lblChangeValue.ForeColor = Theme.Muted

            ' Today tab captions (M10).
            Me.lblTodayTitle.Font = Theme.FontFor(ThemeWeight.Semibold, 11.0F)
            Me.lblTodayTitle.ForeColor = Theme.Ink
            Me.lblTodayCount.ForeColor = Theme.Muted
        End Sub

#End Region

#Region "Combo item wrapper"

        ' Private display wrapper for the category filter combo: carries
        ' the CategoryId (0 = All Categories sentinel) plus a friendly
        ' label rendered through ToString().
        Private NotInheritable Class FilterChoice

            Public ReadOnly CategoryId As Integer
            Private ReadOnly _label As String

            Public Sub New(categoryId As Integer, label As String)
                Me.CategoryId = categoryId
                _label = If(label, String.Empty)
            End Sub

            Public Overrides Function ToString() As String
                Return _label
            End Function

        End Class

#End Region

    End Class

End Namespace
