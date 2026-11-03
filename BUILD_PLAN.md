# BUILD_PLAN.md — Ordered Build Plan: Mini-Grocery Store POS & Daily Inventory System

> Produced by an architect review against `structureflow.md` (the authoritative spec) and `AGENTS.md`.
> Contract: every milestone ends with the solution compiling (0 errors) via MSBuild **and** `oopfinals/oopfinals/bin/Debug/oopfinals.exe` launching with the listed behavior verified.
> Check a milestone's box only after its entire "Run & verify" checklist is green (and `obj/`/`bin/` build artifacts are cleaned from the working tree).
> Do not reorder milestones without updating the dependency graph at the bottom.

---

## Ground rules (from `AGENTS.md`)

- Build (from the `oopfinals/` directory):
  ```
  & "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" oopfinals.sln -v:m
  ```
  Never `dotnet build` (fails: GenerateResource task-host error). Never add NuGet packages.
- Every new `.vb` file **must** be added to `oopfinals/oopfinals/oopfinals.vbproj` `<Compile Include>` — unlisted files silently never compile. Designer-backed forms ship both `X.vb` and `X.Designer.vb` entries (`SubType Form` + `DependentUpon`).
- Every `.vb` starts with `Option Explicit On` / `Option Strict On`. Namespaces: `oopfinals.Models` / `.Data` / `.Services` / `.Controls` / `.Forms`.
- Layering is strictly one-way: `Forms → Services/Data → Models`. Entities go in `Models/`, SQL in `Data/` repositories, workflows in `Services/`, shared UI in `Controls/`.
- Money is `Decimal`, rounded `Math.Round(x, 2, MidpointRounding.AwayFromZero)`. All SQL parameterized.
- Role routing is polymorphic (`GetDashboardForm()`), never an If/Else chain.
- Every form opens maximized; all controls anchored/docked; combo dropdowns are `DropDownList`; money labels `AutoSize`; test at 100% **and** 125% display scaling; constrain styling to `Controls/Theme` tokens — no ad-hoc form styling.
- No plaintext passwords ever: salted PBKDF2 only; never log or display them.
- VC shell note: the temporary `Form1` used as the dev **Launcher** (M4–M7) is a small in-scope edit to `Form1.vb`/`Form1.Designer.vb` and is deleted together with `Form1` at M8.

---

## OOP pillar landing map (spec §5 → milestone)

| Pillar (spec §5) | Lands | Demonstrated in the running exe |
|---|---|---|
| Abstraction — `MustInherit User` contract | M4 | M4 (launcher routes), M8 (real routing) |
| Abstraction — `MustInherit Discount` contract | M2 | M9 (strategy swap on totals) |
| Abstraction — `IReportable` interface | M2 | M16 (reports bound to `ReportTitle`) |
| Encapsulation — validating setters | M1–M2 | M7 (seed enforces), M9–M16 (messages surface in UI) |
| Encapsulation — private ctors / `Shared` only | M3, M6, M17 | code review + UI behavior |
| Encapsulation — read-only cart exposure | M9 | M9–M10 (cart mutated only via `AddItem`/`UpdateItemQuantity`/`RemoveItem`) |
| Inheritance — User family, `MyBase.New` chains | M4 | M4, M8 (routed roles) |
| Inheritance — Discount family | M2 | M9 |
| Polymorphism #1 — `GetDashboardForm()` routing, no if/else | M4 (shells) | **M8** |
| Polymorphism #2 — `CurrentDiscount.Apply(subtotal)` swap | M2 | **M9** |

---

## Milestones

### [x] M0 — Baseline sanity

**Goal:** prove the untouched template builds and launches — the green baseline.

- Files: none. No vbproj changes.
- Run & verify: MSBuild → 0 errors; launch `bin\Debug\oopfinals.exe`; blank `Form1` appears and closes cleanly; confirm no package restore is needed (packages are committed).

---

### [x] M1 — Models wave 1: inventory entities

**Goal:** land the four framework-only, validating entities everything else consumes.

- Files (create under `oopfinals/oopfinals/Models/`):
  - `Product.vb` — strictest validation (negatives rejected; `ProductCode` trimmed/uppercased; `CategoryId`/`SupplierId` > 0) + `IsLowStock/IsExpired/IsExpiringSoon/GetStockStatus` helpers reused by Clerk alerts, Owner reports, Customer catalog.
  - `Category.vb` — `CategoryId`, `Name` entity.
  - `Supplier.vb` — `SupplierId`, `Name`, `ContactNumber`, `Address`.
  - `StockMovement.vb` — immutable audit record (`StockIn`/`Adjustment`/`Sale`), defaults per spec §6.5.
- vbproj adds:
  ```xml
  <Compile Include="Models\Product.vb" />
  <Compile Include="Models\Category.vb" />
  <Compile Include="Models\Supplier.vb" />
  <Compile Include="Models\StockMovement.vb" />
  ```
- Run & verify: 0 errors; exe launches (models are UI-invisible yet). Review: money fields are `Decimal`, setters throw the spec's exact messages.

---

### [x] M2 — Models wave 2: receipts, discount strategies, report contract

**Goal:** finish the non-`User` domain so POS/reports have everything they need.

- Files (`Models/`):
  - `SaleItem.vb` — `Quantity` > 0; get-only `LineTotal = Round(Qty×UnitPrice, 2, AwayFromZero)`.
  - `Sale.vb` — receipt aggregate w/ `Items As List(Of SaleItem)` (aggregation), money setters reject negatives, `DiscountType` default `"None"`, nullable `CustomerId` (walk-in).
  - `Discount.vb` — one file, four classes: `Discount` (MustInherit; protected ctor validates name + % ∈ [0,100]), `NoDiscount` (returns 0), `RegularDiscount` (default 5%, configurable via optional ctor arg), `SeniorPwdDiscount` (hard 20%); `ToString()` = spec display names.
  - `IReportable.vb` — interface per §6.7 (consumed at M16).
- vbproj adds:
  ```xml
  <Compile Include="Models\SaleItem.vb" />
  <Compile Include="Models\Sale.vb" />
  <Compile Include="Models\Discount.vb" />
  <Compile Include="Models\IReportable.vb" />
  ```
- Run & verify: 0 errors; exe launches. Review: `Sale` references nothing beyond `SaleItem`; strategies contain zero UI/DB code.

---

### [ ] M3 — Controls wave 1: themed control library

**Goal:** land the Figma-token system so every later form is styled at birth.

- Files (`oopfinals/oopfinals/Controls/`):
  - `Theme.vb` — tokens (`PageBg #F3F5F4`, `HeaderGreen #1F5C3F`, `LogoutRed #C62828`, grays/destructive/info), `ThemeWeight`/`ThemeButtonKind`/`ThemeTone`, paint helpers, font fallback. `NotInheritable` + `Private Sub New`.
  - `IconArt.vb` — GDI-drawn glyph library.
  - `StyledControls.vb` — `StyledButton`, rounded panels/cards, styled textboxes/combos, focus rings.
  - `StyledGrid.vb` — uniform `DataGridView` skin.
  - `PageHeading.vb`, `AppHeader.vb`, `IconTabBar.vb`.
- vbproj adds:
  ```xml
  <Compile Include="Controls\Theme.vb" />
  <Compile Include="Controls\IconArt.vb" />
  <Compile Include="Controls\StyledControls.vb" />
  <Compile Include="Controls\StyledGrid.vb" />
  <Compile Include="Controls\PageHeading.vb" />
  <Compile Include="Controls\AppHeader.vb" />
  <Compile Include="Controls\IconTabBar.vb" />
  ```
- Run & verify: 0 errors; exe launches (no visual change yet — tokens light up in M4).

---

### [ ] M4 — User family + dashboard shells + temporary Launcher *(Abstraction + Inheritance land)*

**Goal:** make polymorphic routing compile today by shipping the real `User` hierarchy and real (minimal) dashboards.

- Files:
  - `Models/User.vb` — `MustInherit`; private fields; validating setters (`Password hash cannot be empty.`); protected default + parameterized ctors; `MustOverride GetRoleName()/GetDashboardForm()/GetPermissions()`; overridable `HasPermission`; `ToString()` override.
  - `Models/StoreOwner.vb` (14 permission keys), `Models/Cashier.vb` (5), `Models/InventoryClerk.vb` (5), `Models/Customer.vb` (3) — exact key strings from spec §6.2; each ctor chains `MyBase.New` and stamps `Role`.
  - `Forms/OwnerDashboardForm.vb` **+ `Forms/OwnerDashboardForm.Designer.vb`** — shell: AppHeader, user caption (`ToString()`), permission list, Logout (closes). Register-and-never-touch pattern; fleshed out M13–M15.
  - Same shell pattern now for `Forms/CashierDashboardForm.vb/.Designer.vb` (fleshed out M9–M10, M17), `Forms/ClerkDashboardForm.vb/.Designer.vb` (M11), `Forms/CustomerDashboardForm.vb/.Designer.vb` (M12). Each dashboards ctor takes its strongly-typed subclass (spec §6.2) stored in a `ReadOnly` field.
  - Edit `Form1.vb` + `Form1.Designer.vb` — the Launcher: four buttons; each constructs a subclass inline and calls `GetDashboardForm().ShowDialog()` (dynamic dispatch, no `Select Case`).
- vbproj adds (once — dashboards are never re-registered):
  ```xml
  <Compile Include="Models\User.vb" />
  <Compile Include="Models\StoreOwner.vb" />
  <Compile Include="Models\Cashier.vb" />
  <Compile Include="Models\InventoryClerk.vb" />
  <Compile Include="Models\Customer.vb" />
  <Compile Include="Forms\OwnerDashboardForm.vb">
    <SubType>Form</SubType>
  </Compile>
  <Compile Include="Forms\OwnerDashboardForm.Designer.vb">
    <DependentUpon>OwnerDashboardForm.vb</DependentUpon>
    <SubType>Form</SubType>
  </Compile>
  <!-- same three-way pattern for CashierDashboardForm, ClerkDashboardForm, CustomerDashboardForm (.vb + .Designer.vb) -->
  ```
- Run & verify: launcher opens each shell; captions read `Maria Santos (Store Owner) - @owner` (and counterparts); permission lists count 14/5/5/3 per spec §11; Logout closes; dashboards maximized + anchored; no role branching code anywhere.

---

### [ ] M5 — `Data/UserRepository` (inert by design)

**Goal:** user persistence + role→subclass factory that `AuthService` will consume next.

- Files: `Data/UserRepository.vb` — `GetByUsername/GetById/GetAllStaff/GetAllCustomers/UsernameExists/CreateUser` (polymorphic insert keyed on `user.Role`) `/UpdateUser/UpdatePassword/SetActiveStatus`; role string → concrete subclass dispatch inside `GetByUsername`.
- vbproj: `<Compile Include="Data\UserRepository.vb" />`
- Run & verify: 0 errors; exe smoke — nothing calls it until M6/M7 and the DB doesn't exist yet (expected).

---

### [ ] M6 — `Services/ValidationRules` + `Services/AuthService` *(crypto lands)*

**Goal:** one validation source of truth; PBKDF2 so plaintext never persists.

- Files:
  - `Services/ValidationRules.vb` — constants + five validators per §9.2 (username `^[A-Za-z0-9._]+$` 3–20; password ≥6, no whitespace; full-name letter rule; money ≥0, ≤ ₱999,999.99, ≤2 dp; contact `^[0-9+\-]{7,15}$`).
  - `Services/AuthService.vb` — `NotInheritable` + `Private Sub New`; 16-byte salt / 32-byte hash / 10,000 iterations via `Rfc2898DeriveBytes`; `VerifyPassword` + constant-time `SlowEquals`; `Authenticate` with deactivated message `"This account has been deactivated. Please contact the Store Owner."`; `RegisterCustomer`.
- vbproj adds:
  ```xml
  <Compile Include="Services\ValidationRules.vb" />
  <Compile Include="Services\AuthService.vb" />
  ```
- Run & verify (Launcher extension): hash a password → verify True; tamper → False; short password rejected; no plaintext logged. DB-dependent paths (`Authenticate`, `RegisterCustomer`) are exercised at M7/M8 — note that honestly in the checklist.

---

### [ ] M7 — `Data/DatabaseHelper`: schema + atomic seed

**Goal:** seven tables, FK-enforced connection, full first-run seed inside one guarded transaction.

- Files: `Data/DatabaseHelper.vb` — DB path `AppDomain.CurrentDomain.BaseDirectory + "grocery.db"`; connection `Data Source=...;Version=3;Foreign Keys=True;`; idempotent `InitializeDatabase()` (7 × `CREATE TABLE IF NOT EXISTS`); `SeedInitialDataIfEmpty` guarded by `Users COUNT(*) = 0`, single `SQLiteTransaction` w/ rollback; seeds 4 users (salt+hash via `AuthService` — no crypto duplication) + 6 categories + 3 suppliers + 30 products + 30 `StockIn` movements (reason `"Initial Store Inventory Stock-In"`, supplier, user 1), including the §15 edge cases (PRD002/029/030 low; PRD010 expired −20 d; PRD025 expired −5 d; PRD004 expiring +15 d); keep product seed in one editable method.
- vbproj: `<Compile Include="Data\DatabaseHelper.vb" />`
- Run & verify (Launcher gains a temporary "Init DB & demo login" button — dies with Form1 at M8): delete `bin\Debug\grocery.db` → all four demo pairs authenticate and route to their shells (proves DB → hash → verify → subclass factory end-to-end); relaunch without deleting → no duplicate seeding; edge-case rows exist in DB; no passwords logged.

---

### [ ] M8 — Real `LoginForm` + `RegisterForm`; startup swap; `Form1` retirement *(Polymorphism #1 live)*

**Goal:** the app becomes the spec's real front door; the placeholder and launcher scaffolding disappear.

- Files:
  - Create `Forms/LoginForm.vb` **+ `Forms/LoginForm.Designer.vb`** — token restyle; `InitializeDatabase()` on load with error capture into the status label; local pre-checks (username ≥3, password ≥6); lockout after 5 consecutive failures for 30 s (`_failedAttempts`/`_lockoutUntil`); space-typing blocked; show/hide password; register link; **routing = `user.GetDashboardForm()` → `Me.Hide()` → `ShowDialog()`** — no if/else; logout loop clears password + "Logged out successfully."
  - Create `Forms/RegisterForm.vb` **+ `Forms/RegisterForm.Designer.vb`** — `ValidationRules`-driven signup; letters-only full-name KeyPress; no-space credentials; friendly inline errors; calls `AuthService.RegisterCustomer`.
  - **Delete** `Form1.vb` + `Form1.Designer.vb` (launcher scaffolding dies with them; no other file may reference `Form1`).
- vbproj: remove the two `Form1` blocks; add:
  ```xml
  <Compile Include="Forms\LoginForm.vb">
    <SubType>Form</SubType>
  </Compile>
  <Compile Include="Forms\LoginForm.Designer.vb">
    <DependentUpon>LoginForm.vb</DependentUpon>
    <SubType>Form</SubType>
  </Compile>
  <Compile Include="Forms\RegisterForm.vb">
    <SubType>Form</SubType>
  </Compile>
  <Compile Include="Forms\RegisterForm.Designer.vb">
    <DependentUpon>RegisterForm.vb</DependentUpon>
    <SubType>Form</SubType>
  </Compile>
  ```
- `My Project` changes (only milestone in the plan that touches these): `Application.myapp` `<MainForm>Form1</MainForm>` → `<MainForm>LoginForm</MainForm>`; `Application.Designer.vb` `Me.MainForm = Global.oopfinals.Form1` → `Me.MainForm = Global.oopfinals.Forms.LoginForm` (RootNamespace `oopfinals` — both files must agree or startup breaks). `ShutdownMode` and `StartupObject` stay unchanged.
- Run & verify: delete DB → fresh seed via real `LoginForm_Load`; all four demo users route to their shells; register a new meeting-the-rules customer → login routes to the Customer shell; 5 wrong passwords → button locks 30 s; show/hide toggle works; logout round-trip; maximized window at 100% and 125% scaling.

---

### [ ] M9 — Catalog repos + `CartService` + Cashier POS stage 1 *(Polymorphism #2 live)*

**Goal:** the POS reads seeded data and the cart math runs; the sale itself is still pending.

- Files:
  - `Data/ProductRepository.vb` — `GetAll(activeOnly)/Search(keyword, categoryId, activeOnly)/GetById/GetByCode/Add/Update/UpdatePrice/DeleteOrDeactivate` + alert queries `GetLowStockProducts/GetExpiringProducts(30)/GetExpiredProducts`; JOINs fill `CategoryName`/`SupplierName`.
  - `Data/CategoryRepository.vb` — `GetAll/GetById/Add/Update/Delete`.
  - `Services/CartService.vb` — private list exposed as `IReadOnlyList(Of SaleItem)`; `AddItem` merges lines with a **combined** stock check (throws with available stock in message); `UpdateItemQuantity` (0/negative ⇒ remove), `RemoveItem`, `ClearCart` (resets discount to `NoDiscount`); `GetSubtotal`; `GetDiscountAmount = CurrentDiscount.Apply(subtotal)` (the polymorphic call); `GetNetTotal`; `ComputeChange` (insufficient-cash message w/ total due); `GetTotalItemCount`.
  - Edit `Forms/CashierDashboardForm.vb`/`.Designer.vb` (stage 1 of its ~1,179/905 targets): POS tab — Products card (search box, category combo `DropDownList`, grid), Cart card (grid + qty +/− + remove + clear + scan row), payment rail (discount combo, cash box, totals panel, checkout button **present but disabled**).
- vbproj adds:
  ```xml
  <Compile Include="Data\ProductRepository.vb" />
  <Compile Include="Data\CategoryRepository.vb" />
  <Compile Include="Services\CartService.vb" />
  ```
- Run & verify: cashier login → routed POS; grid lists seeded goods with readable `Product.ToString()` rows; category filter works; over-stock add throws with the available-stock message; discount combo shows `None (0%)`, `Regular / Promo (5%)`, `Senior Citizen / PWD (20%)`; **switching the combo recomputes the discount through a different overridden `Apply` — the strategy swap is visible**; cash < total shows the insufficient-cash message; checkout disabled; 100%/125% scaling.

---

### [ ] M10 — `SaleRepository` + checkout + receipt trio + Today *(the money path)*

**Goal:** turn the cart into one atomic receipt — the safety-critical core.

- Files:
  - `Data/SaleRepository.vb` — `GenerateReceiptNumber()` = `RCP-{yyyyMMdd}-{count+1:D4}`; **`RecordSale(sale, items)`**: entry validation (sale ≠ Nothing; ≥1 item; Total > 0; `Subtotal` equals the rounded Σ `LineTotal`) then BEGIN → INSERT Sales → `last_insert_rowid()` → per item INSERT SaleItems + guarded stock update `UPDATE Products SET StockQty = StockQty - @qty WHERE ProductId = @pid AND StockQty >= @qty` (0 rows affected ⇒ throw `"Insufficient stock … Transaction cancelled."`) + INSERT StockMovements ('Sale', −qty, `"POS Sale: RCP-…"`); COMMIT; on any failure ROLLBACK and **rethrow the original error**. Read side: `GetById/GetByReceiptNo/GetSaleItems/GetSalesByDate/GetSalesByDateRange/GetSalesByCustomer/GetSalesByCashierAndDate`.
  - `Services/ReceiptPrinter.vb` — 48-column fixed-width receipt text (store header, item table `NAME(−20) QTY(5) PRICE(10) TOTAL(10)`, summary block, "SALES INVOICE / OFFICIAL RECEIPT" footer); `ShowPrintPreview` (`PrintDocument` + `PrintPreviewDialog`); `PrintDirectly`.
  - `Forms/ReceiptPreviewForm.vb` **+ Designer** — modal post-checkout preview; Print → `ShowPrintPreview`; Close returns to POS which clears the cart.
  - `Forms/PastReceiptForm.vb` **+ Designer** — historical receipt details + reprint (reused by Cashier Today, Owner Transactions, Customer History).
  - Edit `CashierDashboardForm` (stage 2): checkout wiring (`_isCheckingOut` re-entry guard + confirm dialog), Today tab (grid + View details + refresh), Enter/F12 shortcuts, post-sale refresh of POS grid + Today, payment UI reset.
- vbproj adds:
  ```xml
  <Compile Include="Data\SaleRepository.vb" />
  <Compile Include="Services\ReceiptPrinter.vb" />
  <Compile Include="Forms\ReceiptPreviewForm.vb">
    <SubType>Form</SubType>
  </Compile>
  <Compile Include="Forms\ReceiptPreviewForm.Designer.vb">
    <DependentUpon>ReceiptPreviewForm.vb</DependentUpon>
    <SubType>Form</SubType>
  </Compile>
  <Compile Include="Forms\PastReceiptForm.vb">
    <SubType>Form</SubType>
  </Compile>
  <Compile Include="Forms\PastReceiptForm.Designer.vb">
    <DependentUpon>PastReceiptForm.vb</DependentUpon>
    <SubType>Form</SubType>
  </Compile>
  ```
- Run & verify: sell PRD001 ×2 → confirm dialog → preview shows correct totals/change → Print opens the print preview; close → cart cleared, discount reset, POS stock numbers reflect the decrement, Today lists the sale; two sequential sales get `-0001`/`-0002` receipt numbers; **guards**: empty cart and Total ≤ 0 blocked before any SQL; cash < total message; cash > ₱999,999.99 rejected; over-stock rejected with the transaction-cancelled message and zero rows written (POS grid unchanged proves rollback); double-click ⇒ exactly one sale; re-print from Today renders the identical receipt; logout round-trip.

---

### [ ] M11 — `StockRepository` + `SupplierRepository` + Clerk dashboard full

**Goal:** backroom inventory — seed edge cases drive Alerts; both write paths transactional.

- Files:
  - `Data/StockRepository.vb` — `RecordStockIn` (qty > 0 and ≤ 10,000; **past-dated expiry rejected**: `"Expired stock must be rejected, not received. Record a disposal adjustment instead."`; transaction: `StockQty +=` (+ optional `ExpiryDate` update) + movement 'StockIn'); `RecordAdjustment` (change ≠ 0; reason required; pre-check `currentQty + change >= 0` → negative-stock message; transaction + movement 'Adjustment'); `GetMovements` with type/date filters.
  - `Data/SupplierRepository.vb` — `GetAll/GetById/Add/Update/Delete`.
  - Edit `Forms/ClerkDashboardForm.vb`/`.Designer.vb` (to its 773/1,145 targets): Alerts (radio filters All/Low Stock/Expiring/Expired + typed badges + refresh), Stock-In (product, supplier `DropDownList`, qty, checkbox-gated optional new expiry, reason), Adjustment (current stock auto-shown, signed qty, mandatory reason combo), Movements (audit grid + filters), Catalog (read-only search).
- vbproj adds:
  ```xml
  <Compile Include="Data\StockRepository.vb" />
  <Compile Include="Data\SupplierRepository.vb" />
  ```
- Run & verify: clerk login → routed; Alerts reveals PRD002/029/030 low, PRD010/025 expired, PRD004 expiring ≤30 d; Stock-In PRD030 +5 succeeds (stock + movement visible); past-dated expiry rejected with the quoted message; qty 0/negative/10,001 rejected; Adjustment −999 blocked (negative-stock message); zero change rejected; missing reason rejected; Movements lists M10 'Sale' rows with `POS Sale: RCP-…` plus the new StockIn/Adjustment rows; Catalog is read-only; 100/125% scaling.

---

### [ ] M12 — Customer dashboard full

**Goal:** customer portal against live data; honest empty History. (Independent of M11 — may run any time after M10.)

- Files: edit `Forms/CustomerDashboardForm.vb`/`.Designer.vb` (to its 769/659 targets): Browse (product cards, live search, category filter, status/expiry badges from `Product` helpers); History (`GetSalesByCustomer` + "View receipt details" → `PastReceiptForm`); Profile (name edit with letters-only KeyPress; change password = verify current → `ValidationRules.IsValidPassword` → new salt+hash → `UserRepository.UpdatePassword`).
- vbproj: none (registered at M4).
- Run & verify: customer login → routed; Browse shows active products (live search/filter; expired items badged); History shows a clean empty state — expected, since demo POS sales are walk-in (`CustomerId` NULL); query must not error; name change blocks digits; password change → logout → old password fails, new one works; scaling.

---

### [ ] M13 — Owner stage A: Overview KPIs + Master Data

**Goal:** owner dashboard starts living (KPIs computed); master-data CRUD.

- Files: edit `Forms/OwnerDashboardForm.vb`/`.Designer.vb` (stage 1 of its ~1,962/2,055 targets): Overview (KPI cards — today's sales total, transaction count, low-stock count, expiring count — re-flowed on resize); Master Data (categories CRUD + suppliers CRUD, contact regex enforced, duplicate-name error).
- vbproj: none.
- Run & verify: owner login routes with the 14-key list; KPI numbers consistent with seed + M10 sales (cross-check counts vs. Clerk Alerts lists); resizing re-flows cards; adding/renaming a category shows up in product grids on next load; bad contact number rejected; deleting an empty category succeeds; combos are `DropDownList`; anchored at 100/125%.

---

### [ ] M14 — Owner stage B: Staff admin

**Goal:** full account lifecycle — closes the loop with login.

- Files: edit `OwnerDashboardForm` (Staff tab: `GetAllStaff` grid; New/Save via `ValidationRules` + unique-username pre-check; Reset password (new salt+hash → `UpdatePassword`); Activate/Deactivate via `SetActiveStatus`; role combo `DropDownList`; letters-only name filter).
- vbproj: none.
- Run & verify: grid shows the 3 seeded staff; create `cashier2`/`cashier123` → logout → login routes to the Cashier dashboard; deactivate `cashier2` → login shows the exact deactivation message; reactivate → works; reset password → new password logs in; duplicate username rejected; scaling.

---

### [ ] M15 — Owner stage C: Products CRUD + Transactions browser

**Goal:** catalog administration + receipts browser.

- Files: edit `OwnerDashboardForm` (Products tab: `Search` + category filter grid; New/Save with code/name/category/supplier/prices/stock/reorder/expiry; duplicate `ProductCode` rejected; money rules enforced at save; delete → `DeleteOrDeactivate`; Transactions tab: `GetSalesByDateRange` grid + View details → `PastReceiptForm` + reprint).
- vbproj: none.
- Run & verify: create `PRD031`; duplicate code rejected; price −1 / over cap / 3 decimals rejected at save; deactivate a product → it disappears from the POS grid and customer Browse (`IsActive`) → restore works; Transactions shows M10 sales; details dialog matches the sale; reprint equals the preview; scaling.

---

### [ ] M16 — `ReportService` + Owner Reports tab *(IReportable demonstrated)*

**Goal:** three reports with summary statistics, CSV export, print.

- Files: `Services/ReportService.vb` (DailySales with ByRef summary `TransactionCount/TotalGross/TotalDiscount/TotalNet`; InventoryValuation with `TotalCostValue/TotalRetailValue/PotentialProfit`; LowStockAndExpiry with `EXPIRED`/`LOW STOCK`/`EXPIRING SOON` tags; escaped CSV exporter; tabular print with page header from `ReportTitle` + `GeneratedDate`); edit `OwnerDashboardForm` (Reports tab: report combo → grid + summary chips + Export CSV via `SaveFileDialog` + Print).
- vbproj: `<Compile Include="Services\ReportService.vb" />`
- Run & verify: DailySales row count matches Today's sales; InventoryValuation potential profit spot-checkable against two products; LowStockAndExpiry lists the seed edge items with correct tags; exported CSV opens cleanly; print preview uses the `IReportable` title; zero UI dependencies inside the service; scaling.

---

### [ ] M17 — Nutrition module + Owner & Cashier integration

**Goal:** async demo import with honest "not found until imported" semantics.

- Files:
  - `Services/NutritionStore.vb` — `NotInheritable`; `SyncLock`'ed dictionary keyed by seed code; 25 of 30 records (5 personal-care SKUs intentionally absent); `OffImportProgress`/`OffImportState`; `RunImportAsync(IProgress, CancellationToken)`; `HasPer100Nutrition` ⇒ Verified, `NeedsReview` ⇒ "Needs review".
  - `Controls/NutritionTableControl.vb` — reusable nutrition facts table (energy/protein/fat/carbs/sodium).
  - `Forms/NutritionDetailView.vb` — single code-built dialog file, no Designer (spec row 48 lists none); Verified/"Needs review" badges; per-serving vs per-100g presentation; raw-data toggle.
  - Edit `OwnerDashboardForm` (Nutrition tab: import card, progress rail + counters, catalog table, detail dialog) and `CashierDashboardForm` (nutrition card on the POS payment rail — no DB writes).
- vbproj adds:
  ```xml
  <Compile Include="Services\NutritionStore.vb" />
  <Compile Include="Controls\NutritionTableControl.vb" />
  <Compile Include="Forms\NutritionDetailView.vb" />
  ```
- Run & verify: before import: Owner Nutrition tab shows "No nutrition records yet" and the POS card shows its honest empty state; run import → counters sweep to 25 processed with a visible valid/needs-review split; UI stays responsive (Task + `IProgress`); the 5 personal-care lookups return Nothing error-free; the detail dialog opens from the POS card link and the Owner row click; raw toggle works; scaling.

---

### [ ] M18 — Acceptance & hardening (no new files)

**Goal:** prove the whole app against the spec, then leave the tree clean.

- Checklist:
  - Full §11 permission matrix walk (Owner owns all 14 keys incl. ProcessSale/StockIn; Clerk never sees POS; Customer exactly 3 keys).
  - Every dashboard re-checked maximized/anchored at **100% and 125%** scaling.
  - Delete `grocery.db` → complete fresh reseed on first login.
  - Money math audit (`Decimal` everywhere, `MidpointRounding.AwayFromZero`).
  - Password audit (PBKDF2 path only; no plaintext in code, logs, or the `Users` table).
  - Every `.vb` carries `Option Strict On`.
  - `Form1` physically absent from both the vbproj and disk.
  - `packages.config` untouched (no new NuGet); no `dotnet build` in any log.
  - Final artifact hygiene per `AGENTS.md`: `git restore` dirtied `obj/` caches, `git clean -f oopfinals/oopfinals/bin oopfinals/oopfinals/obj`; commit only source.

---

## Dependency graph

```
M0 baseline
 └─ M1 inventory entities → M2 sale/Discount/IReportable → M3 Controls
     └─ M4 User hierarchy + 4 shells (+Launcher) → M5 UserRepository
         └─ M6 ValidationRules + AuthService → M7 DatabaseHelper (schema+seed)
             └─ M8 LoginForm/RegisterForm real; startup→LoginForm; Form1 deleted  ★routing live
                 └─ M9 Product/Category repos + CartService; POS stage 1  ★strategy swap
                     └─ M10 SaleRepository + receipt trio; checkout + Today  ★atomic sale
                         ├─ M11 Stock/Supplier repos; Clerk dashboard full
                         ├─ M12 Customer dashboard full        (any time ≥ M10)
                         └─ M13 Owner A Overview + MasterData
                             └─ M14 Owner B Staff → M15 Owner C Products + Transactions
                                 → M16 ReportService + Reports tab  ★IReportable
                                     → M17 Nutrition trio + integrations
                                         → M18 acceptance & hardening
```

- M11 and M12 are mutually independent once M10 lands; M12 may be reordered anywhere after M10.
- Every strict arrow preserves the milestone contract: compile 0 errors + feature runs in `bin\Debug\oopfinals.exe` at milestone end.
- Owner (M13–M15), Clerk (M11), and Customer (M12) dashboards are each staged as a single compiling slice — the spec's large line counts (~1,962/2,055 for Owner etc.) are targets, not exact sizes.
