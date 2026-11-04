Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Data/DatabaseHelper.vb — connection owner, schema creation, and the
' atomic first-run seed (spec §7.1/§8/§15).
'
' Responsibilities (spec §7.1):
'   - Owns the ONE canonical DB location + connection string
'     ("bin\Debug\grocery.db", Foreign Keys ON).
'   - InitializeDatabase(): idempotent — runs the seven CREATE TABLE
'     IF NOT EXISTS statements, then seeds ONLY when Users is empty.
'   - SeedInitialDataIfEmpty(): 4 demo users + 6 categories + 3
'     suppliers + 30 products + 30 audit movements inside ONE
'     SQLiteTransaction — any failure rolls the WHOLE seed back.
'
' OOP concept: ENCAPSULATION (static gate):
'   NotInheritable + Private Sub New — no instances, Shared surface
'   only, exactly like AuthService/ValidationRules/UserRepository.
'
' OOP concept: ENCAPSULATION (the seed goes through the models):
'   Products are built via the REAL Product validating setters before
'   they are inserted, so invalid seed data is impossible to construct
'   — the seed enforces the same rules the UI does.
'
' SECURITY: demo passwords are hashed through Services/AuthService
' (PBKDF2 — no crypto duplication); only salt + hash ever reach the
' Users table, and no password is ever logged or displayed.
'
' LAYERING: Data → Services (AuthService hashing) + Data → Models
' (entities). User-facing flows keep 100% parameterized SQL — the seed
' is the trusted code path allowed to hydrate entities the ORM-style
' way per spec §7.1.
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Data.SQLite
Imports System.IO
Imports oopfinals.Models
Imports oopfinals.Services

Namespace Data

    Public NotInheritable Class DatabaseHelper

        ' Static gate: no DatabaseHelper instances, ever.
        Private Sub New()
        End Sub

#Region "Location + connection (spec §7.1/§8)"

        ' Single canonical file name + ISO format for every TEXT date
        ' column in the schema (spec §8: yyyy-MM-dd HH:mm:ss).
        Public Const DatabaseFileName As String = "grocery.db"
        Public Const IsoDateTimeFormat As String = "yyyy-MM-dd HH:mm:ss"

        ' bin\Debug\grocery.db at Debug run time (spec §7.1).
        Public Shared ReadOnly Property DatabasePath As String
            Get
                Return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseFileName)
            End Get
        End Property

        ' Always a FRESH, short-lived connection; Foreign Keys=True keeps
        ' the §8 referential rules enforced on every statement.
        Public Shared Function GetConnection() As SQLiteConnection
            Dim connection As New SQLiteConnection("Data Source=" & DatabasePath & ";Version=3;Foreign Keys=True;")
            connection.Open()
            Return connection
        End Function

        Private Shared Function IsoStamp(moment As DateTime) As String
            Return moment.ToString(IsoDateTimeFormat)
        End Function

#End Region

#Region "Schema — seven CREATE TABLE IF NOT EXISTS (spec §8)"

        ' Column order + names EXACTLY as the §8 table lists them. The
        ' statements are idempotent, so start-up calls are always safe.
        ' (Elements stay contiguous — VB implicit continuation does not
        ' tolerate blank lines inside a collection initializer.)
        Private Shared ReadOnly SchemaStatements() As String = {
            "CREATE TABLE IF NOT EXISTS Users (" &
            "UserId INTEGER PRIMARY KEY AUTOINCREMENT, " &
            "Username TEXT NOT NULL UNIQUE, " &
            "PasswordHash TEXT NOT NULL, " &
            "Salt TEXT NOT NULL, " &
            "FullName TEXT NOT NULL, " &
            "Role TEXT NOT NULL, " &
            "IsActive INTEGER NOT NULL DEFAULT 1, " &
            "CreatedAt TEXT NOT NULL);",
            "CREATE TABLE IF NOT EXISTS Categories (" &
            "CategoryId INTEGER PRIMARY KEY AUTOINCREMENT, " &
            "Name TEXT NOT NULL UNIQUE);",
            "CREATE TABLE IF NOT EXISTS Suppliers (" &
            "SupplierId INTEGER PRIMARY KEY AUTOINCREMENT, " &
            "Name TEXT NOT NULL, " &
            "ContactNumber TEXT, " &
            "Address TEXT);",
            "CREATE TABLE IF NOT EXISTS Products (" &
            "ProductId INTEGER PRIMARY KEY AUTOINCREMENT, " &
            "ProductCode TEXT NOT NULL UNIQUE, " &
            "Name TEXT NOT NULL, " &
            "CategoryId INTEGER NOT NULL, " &
            "SupplierId INTEGER NOT NULL, " &
            "Price REAL NOT NULL, " &
            "CostPrice REAL NOT NULL, " &
            "StockQty INTEGER NOT NULL DEFAULT 0, " &
            "ReorderLevel INTEGER NOT NULL DEFAULT 0, " &
            "ExpiryDate TEXT, " &
            "IsActive INTEGER NOT NULL DEFAULT 1, " &
            "FOREIGN KEY (CategoryId) REFERENCES Categories (CategoryId), " &
            "FOREIGN KEY (SupplierId) REFERENCES Suppliers (SupplierId));",
            "CREATE TABLE IF NOT EXISTS Sales (" &
            "SaleId INTEGER PRIMARY KEY AUTOINCREMENT, " &
            "ReceiptNo TEXT NOT NULL UNIQUE, " &
            "SaleDate TEXT NOT NULL, " &
            "CashierId INTEGER NOT NULL, " &
            "CustomerId INTEGER, " &
            "Subtotal REAL NOT NULL, " &
            "DiscountType TEXT NOT NULL, " &
            "DiscountAmount REAL NOT NULL, " &
            "Total REAL NOT NULL, " &
            "CashReceived REAL NOT NULL, " &
            "ChangeAmount REAL NOT NULL, " &
            "FOREIGN KEY (CashierId) REFERENCES Users (UserId), " &
            "FOREIGN KEY (CustomerId) REFERENCES Users (UserId));",
            "CREATE TABLE IF NOT EXISTS SaleItems (" &
            "SaleItemId INTEGER PRIMARY KEY AUTOINCREMENT, " &
            "SaleId INTEGER NOT NULL, " &
            "ProductId INTEGER NOT NULL, " &
            "Quantity INTEGER NOT NULL, " &
            "UnitPrice REAL NOT NULL, " &
            "LineTotal REAL NOT NULL, " &
            "FOREIGN KEY (SaleId) REFERENCES Sales (SaleId), " &
            "FOREIGN KEY (ProductId) REFERENCES Products (ProductId));",
            "CREATE TABLE IF NOT EXISTS StockMovements (" &
            "MovementId INTEGER PRIMARY KEY AUTOINCREMENT, " &
            "ProductId INTEGER NOT NULL, " &
            "MovementType TEXT NOT NULL, " &
            "QuantityChange INTEGER NOT NULL, " &
            "Reason TEXT NOT NULL DEFAULT '', " &
            "SupplierId INTEGER, " &
            "UserId INTEGER NOT NULL, " &
            "MovementDate TEXT NOT NULL, " &
            "FOREIGN KEY (ProductId) REFERENCES Products (ProductId), " &
            "FOREIGN KEY (SupplierId) REFERENCES Suppliers (SupplierId), " &
            "FOREIGN KEY (UserId) REFERENCES Users (UserId));"
        }

#End Region

#Region "InitializeDatabase — idempotent entry point (spec §7.1)"

        ' Called on every app start (LoginForm_Load at M8; the launcher
        ' demo button at M7). Returns True when THIS call created a
        ' brand-new database file or executed the first-run seed.
        '
        ' ① isNewDb = file missing right now (stored for callers),
        ' ② run the seven DDL statements,
        ' ③ seed only if Users is empty.
        Public Shared Function InitializeDatabase() As Boolean
            Dim isNewDatabase As Boolean = Not File.Exists(DatabasePath)

            Using connection As SQLiteConnection = GetConnection()
                For Each ddl As String In SchemaStatements
                    Using command As New SQLiteCommand(ddl, connection)
                        command.ExecuteNonQuery()
                    End Using
                Next

                ' One deliberate secondary index beyond the §8 DDL: SQLite
                ' auto-indexes PK/UNIQUE columns only and nothing on the
                ' child side — SaleItems.SaleId is the fan-out column of
                ' every receipt-details read (GetSaleItems, M10+). IF NOT
                ' EXISTS keeps it idempotent, like the CREATE TABLE loop.
                Using indexCommand As New SQLiteCommand("CREATE INDEX IF NOT EXISTS idx_saleitems_saleid ON SaleItems(SaleId);", connection)
                    indexCommand.ExecuteNonQuery()
                End Using

                Dim seedRan As Boolean = SeedInitialDataIfEmpty(connection)
                Return isNewDatabase OrElse seedRan
            End Using
        End Function

#End Region

#Region "Seed guard + transaction"

        ' Guard: SELECT COUNT(*) FROM Users must be ZERO — everything else
        ' is inside ONE transaction with rollback on ANY failure, so a
        ' half-seeded database can never exist on disk (spec §7.1).
        Private Shared Function SeedInitialDataIfEmpty(connection As SQLiteConnection) As Boolean
            Dim userCount As Long = 0

            Using countCommand As New SQLiteCommand("SELECT COUNT(*) FROM Users;", connection)
                userCount = Convert.ToInt64(countCommand.ExecuteScalar())
            End Using

            ' Seed ran once before — idempotence lives here.
            If userCount <> 0 Then
                Return False
            End If

            Using transaction As SQLiteTransaction = connection.BeginTransaction()
                Try
                    SeedUsers(connection, transaction)
                    SeedCategories(connection, transaction)
                    SeedSuppliers(connection, transaction)
                    SeedProductsAndMovements(connection, transaction)
                    transaction.Commit()
                    Return True
                Catch seedError As Exception
                    ' Best-effort rollback, guarded in its own Try/Catch:
                    ' if the rollback itself faults, that error must NOT
                    ' become the rethrown one. The ORIGINAL seeding error
                    ' is rethrown either way, unchanged.
                    Try
                        transaction.Rollback()
                    Catch
                        ' Rollback failure observed and deliberately set
                        ' aside — the rethrow below carries seedError.
                    End Try
                    Throw seedError   ' rethrow the ORIGINAL error (never swallowed)
                End Try
            End Using
        End Function

        ' Transaction-scoped command factory: every statement below is
        ' created through this and stays fully parameterized.
        Private Shared Function CreateCommand(connection As SQLiteConnection,
                                              transaction As SQLiteTransaction,
                                              sql As String) As SQLiteCommand
            Dim command As SQLiteCommand = connection.CreateCommand()
            command.Transaction = transaction
            command.CommandText = sql
            Return command
        End Function

        ' NULL-safe parameter helper for the nullable ExpiryDate column.
        Private Shared Sub AddNullableExpiry(command As SQLiteCommand,
                                             parameterName As String,
                                             value As Nullable(Of DateTime))
            If value.HasValue Then
                command.Parameters.AddWithValue(parameterName, IsoStamp(value.Value))
            Else
                command.Parameters.AddWithValue(parameterName, DBNull.Value)
            End If
        End Sub

#End Region

#Region "Seed records used by the iterations"

        ' Tiny seed structures (spec §7.1) — intent is obvious, values are
        ' the §15 catalog. The Password row is the §15 demo credential,
        ' hashed IMMEDIATELY at seed time; it is never stored, logged or
        ' displayed as plain text.
        Private NotInheritable Class SeedUserSpec
            Public ReadOnly RoleKey As String
            Public ReadOnly Username As String
            Public ReadOnly DemoPassword As String
            Public ReadOnly FullName As String

            Public Sub New(roleKey As String, username As String, demoPassword As String, fullName As String)
                Me.RoleKey = roleKey
                Me.Username = username
                Me.DemoPassword = demoPassword
                Me.FullName = fullName
            End Sub
        End Class

        Private NotInheritable Class SeedSupplierSpec
            Public ReadOnly Name As String
            Public ReadOnly ContactNumber As String
            Public ReadOnly Address As String

            Public Sub New(name As String, contactNumber As String, address As String)
                Me.Name = name
                Me.ContactNumber = contactNumber
                Me.Address = address
            End Sub
        End Class

        Private NotInheritable Class SeedProductSpec
            Public ReadOnly ProductCode As String
            Public ReadOnly ProductName As String
            Public ReadOnly CategoryId As Integer
            Public ReadOnly SupplierId As Integer
            Public ReadOnly Price As Decimal
            Public ReadOnly CostPrice As Decimal
            Public ReadOnly StockQty As Integer
            Public ReadOnly ReorderLevel As Integer
            ' Nothing = no expiration date at all (matches the nullable
            ' schema column); a value is DAYS FROM TODAY so the §15 edge
            ' cases stay relative to whenever the database is reseeded.
            Public ReadOnly ExpiryOffsetDays As Nullable(Of Integer)

            Public Sub New(productCode As String,
                           productName As String,
                           categoryId As Integer,
                           supplierId As Integer,
                           price As Decimal,
                           costPrice As Decimal,
                           stockQty As Integer,
                           reorderLevel As Integer,
                           Optional expiryOffsetDays As Nullable(Of Integer) = Nothing)
                Me.ProductCode = productCode
                Me.ProductName = productName
                Me.CategoryId = categoryId
                Me.SupplierId = supplierId
                Me.Price = price
                Me.CostPrice = costPrice
                Me.StockQty = stockQty
                Me.ReorderLevel = reorderLevel
                Me.ExpiryOffsetDays = expiryOffsetDays
            End Sub
        End Class

        ' One product's pending audit row, filled in during the product
        ' insert pass and written in the movement pass that follows.
        Private Structure PendingStockIn
            Public ProductId As Integer
            Public SupplierId As Integer
            Public StockQty As Integer
        End Structure

#End Region

#Region "Seed iterations"

        ' §15 demo identities + roles EXACTLY as UserRepository's factory
        ' dispatch expects them ("StoreOwner"/"Cashier"/"InventoryClerk"/
        ' "Customer"); usernames lowercase so login lookups match.
        Private Shared Sub SeedUsers(connection As SQLiteConnection, transaction As SQLiteTransaction)
            Dim seedUsers As New List(Of SeedUserSpec)()
            seedUsers.Add(New SeedUserSpec("StoreOwner", "owner", "owner123", "Maria Santos"))
            seedUsers.Add(New SeedUserSpec("Cashier", "cashier", "cashier123", "Juan Dela Cruz"))
            seedUsers.Add(New SeedUserSpec("InventoryClerk", "clerk", "clerk123", "Pedro Penduko"))
            seedUsers.Add(New SeedUserSpec("Customer", "customer", "customer123", "Ana Batumbakal"))

            For Each spec As SeedUserSpec In seedUsers
                ' AuthService owns crypto — no hash/salt duplication here.
                Dim salt As String = AuthService.GenerateSalt()
                Dim passwordHash As String = AuthService.HashPassword(spec.DemoPassword, salt)

                Using insert As SQLiteCommand = CreateCommand(connection, transaction,
                                                              "INSERT INTO Users (Username, PasswordHash, Salt, FullName, Role, IsActive, CreatedAt) " &
                                                              "VALUES (@username, @passwordHash, @salt, @fullName, @role, @isActive, @createdAt);")
                    insert.Parameters.AddWithValue("@username", spec.Username.ToLowerInvariant())
                    insert.Parameters.AddWithValue("@passwordHash", passwordHash)
                    insert.Parameters.AddWithValue("@salt", salt)
                    insert.Parameters.AddWithValue("@fullName", spec.FullName)
                    insert.Parameters.AddWithValue("@role", spec.RoleKey)
                    insert.Parameters.AddWithValue("@isActive", 1)
                    insert.Parameters.AddWithValue("@createdAt", IsoStamp(DateTime.Now))
                    insert.ExecuteNonQuery()
                End Using
            Next
        End Sub

        ' The 6 §15 categories; CategoryId 1..6 follows this exact order.
        Private Shared Sub SeedCategories(connection As SQLiteConnection, transaction As SQLiteTransaction)
            Dim names As New List(Of String)()
            names.Add("Rice & Grains")
            names.Add("Canned Goods")
            names.Add("Noodles & Pasta")
            names.Add("Beverages & Drinks")
            names.Add("Snacks & Biscuits")
            names.Add("Personal Care & Toiletries")

            For Each name As String In names
                Using insert As SQLiteCommand = CreateCommand(connection, transaction,
                                                              "INSERT INTO Categories (Name) VALUES (@name);")
                    insert.Parameters.AddWithValue("@name", name)
                    insert.ExecuteNonQuery()
                End Using
            Next
        End Sub

        ' The 3 §15 suppliers; SupplierId 1..3 follows this exact order.
        Private Shared Sub SeedSuppliers(connection As SQLiteConnection, transaction As SQLiteTransaction)
            Dim suppliers As New List(Of SeedSupplierSpec)()
            suppliers.Add(New SeedSupplierSpec("San Miguel Distribution", "0917-800-1234", "San Miguel Ave., Mandaluyong City"))
            suppliers.Add(New SeedSupplierSpec("Universal Robina Logistics", "0918-800-5678", "Julia Vargas Ave., Pasig City"))
            suppliers.Add(New SeedSupplierSpec("Mega Global Supply Co.", "0922-800-9012", "Grand Terminal, Valenzuela City"))

            For Each spec As SeedSupplierSpec In suppliers
                Using insert As SQLiteCommand = CreateCommand(connection, transaction,
                                                              "INSERT INTO Suppliers (Name, ContactNumber, Address) VALUES (@name, @contactNumber, @address);")
                    insert.Parameters.AddWithValue("@name", spec.Name)
                    insert.Parameters.AddWithValue("@contactNumber", spec.ContactNumber)
                    insert.Parameters.AddWithValue("@address", spec.Address)
                    insert.ExecuteNonQuery()
                End Using
            Next
        End Sub

        ' THE single editable catalog list (spec §15): every product goes
        ' through the real Product validating setters, then both passes —
        ' the Products insert and its paired StockIn movement — run here.
        Private Shared Sub SeedProductsAndMovements(connection As SQLiteConnection, transaction As SQLiteTransaction)
            Dim pendingMovements As New List(Of PendingStockIn)()
            Dim insertedAt As String = IsoStamp(DateTime.Now)

            For Each spec As SeedProductSpec In GetSeedProducts()
                ' Trust the model: invalid seed values would throw right here.
                Dim product As New Product(spec.ProductCode,
                                           spec.ProductName,
                                           spec.CategoryId,
                                           spec.SupplierId,
                                           spec.Price,
                                           spec.CostPrice,
                                           spec.StockQty,
                                           spec.ReorderLevel)
                If spec.ExpiryOffsetDays.HasValue Then
                    product.ExpiryDate = DateTime.Today.AddDays(spec.ExpiryOffsetDays.Value)
                End If
                product.IsActive = True

                Using insert As SQLiteCommand = CreateCommand(connection, transaction,
                                                              "INSERT INTO Products (ProductCode, Name, CategoryId, SupplierId, Price, CostPrice, StockQty, ReorderLevel, ExpiryDate, IsActive) " &
                                                              "VALUES (@productCode, @name, @categoryId, @supplierId, @price, @costPrice, @stockQty, @reorderLevel, @expiryDate, @isActive);")
                    insert.Parameters.AddWithValue("@productCode", product.ProductCode)
                    insert.Parameters.AddWithValue("@name", product.Name)
                    insert.Parameters.AddWithValue("@categoryId", product.CategoryId)
                    insert.Parameters.AddWithValue("@supplierId", product.SupplierId)
                    insert.Parameters.AddWithValue("@price", Math.Round(product.Price, 2, MidpointRounding.AwayFromZero))
                    insert.Parameters.AddWithValue("@costPrice", Math.Round(product.CostPrice, 2, MidpointRounding.AwayFromZero))
                    insert.Parameters.AddWithValue("@stockQty", product.StockQty)
                    insert.Parameters.AddWithValue("@reorderLevel", product.ReorderLevel)
                    AddNullableExpiry(insert, "@expiryDate", product.ExpiryDate)
                    insert.Parameters.AddWithValue("@isActive", 1)
                    insert.ExecuteNonQuery()
                End Using

                ' Capture the real id (SELECT last_insert_rowid) per spec.
                Dim newProductId As Integer
                Using idCommand As SQLiteCommand = CreateCommand(connection, transaction,
                                                                 "SELECT last_insert_rowid();")
                    newProductId = Convert.ToInt32(idCommand.ExecuteScalar())
                End Using

                pendingMovements.Add(New PendingStockIn With {
                    .ProductId = newProductId,
                    .SupplierId = spec.SupplierId,
                    .StockQty = spec.StockQty
                })
            Next

            ' §15: every seeded product gets ONE matching audit row with
            ' reason "Initial Store Inventory Stock-In", its supplier, user 1.
            For Each pending As PendingStockIn In pendingMovements
                Using insert As SQLiteCommand = CreateCommand(connection, transaction,
                                                              "INSERT INTO StockMovements (ProductId, MovementType, QuantityChange, Reason, SupplierId, UserId, MovementDate) " &
                                                              "VALUES (@productId, @movementType, @quantityChange, @reason, @supplierId, @userId, @movementDate);")
                    insert.Parameters.AddWithValue("@productId", pending.ProductId)
                    insert.Parameters.AddWithValue("@movementType", StockMovement.MovementTypeStockIn)
                    insert.Parameters.AddWithValue("@quantityChange", pending.StockQty)
                    insert.Parameters.AddWithValue("@reason", "Initial Store Inventory Stock-In")
                    insert.Parameters.AddWithValue("@supplierId", pending.SupplierId)
                    insert.Parameters.AddWithValue("@userId", 1)   ' owner seeded first
                    insert.Parameters.AddWithValue("@movementDate", insertedAt)
                    insert.ExecuteNonQuery()
                End Using
            Next
        End Sub

#End Region

#Region "GetSeedProducts — the one editable catalog method (spec §15)"

        ' 30 realistic Filipino grocery products, all 6 categories and
        ' all 3 suppliers, CostPrice < Price everywhere. Deliberate edge
        ' cases light up the first run:
        '   - LOW STOCK    : PRD002 (8 ≤ 10), PRD023 (6 ≤ 12),
        '                    PRD029 (5 ≤ 15), PRD030 (2 ≤ 15)
        '   - EXPIRED      : PRD010 (−20 d), PRD025 (−5 d)
        '   - EXPIRING 30d : PRD004 (+15 d)
        '   - NO EXPIRY    : PRD020, PRD028 (nullable column path)
        Private Shared Function GetSeedProducts() As List(Of SeedProductSpec)
            Dim products As New List(Of SeedProductSpec)()
            Dim noExpiry As Nullable(Of Integer) = Nothing

            ' --- Rice & Grains (categoryId 1) ---
            products.Add(New SeedProductSpec("PRD001", "Dinorado Special Rice 5kg", 1, 1, 280D, 245D, 45, 10, 240))
            products.Add(New SeedProductSpec("PRD002", "Sinandomeng Rice 5kg", 1, 1, 250D, 218D, 8, 10, 180))    ' LOW STOCK
            products.Add(New SeedProductSpec("PRD003", "Jasmine Rice 10kg", 1, 2, 520D, 462D, 30, 8, 300))
            products.Add(New SeedProductSpec("PRD004", "Brown Rice 2kg", 1, 1, 130D, 112D, 25, 10, 15))          ' EXPIRING SOON

            ' --- Canned Goods (categoryId 2) ---
            products.Add(New SeedProductSpec("PRD005", "Century Tuna Flakes in Oil 180g", 2, 3, 42D, 35D, 60, 15, 330))
            products.Add(New SeedProductSpec("PRD006", "Mega Sardines Green 155g", 2, 3, 25D, 20D, 80, 20, 240))
            products.Add(New SeedProductSpec("PRD007", "555 Sardines Green 155g", 2, 2, 24D, 19D, 55, 15, 210))
            products.Add(New SeedProductSpec("PRD008", "Ligo Sardines Green 155g", 2, 3, 22D, 17D, 48, 12, 180))
            products.Add(New SeedProductSpec("PRD009", "Argentina Beef Corned Beef 150g", 2, 2, 38D, 31D, 40, 12, 270))
            products.Add(New SeedProductSpec("PRD010", "Century Tuna Hot & Spicy 180g", 2, 3, 44D, 36D, 26, 12, -20)) ' EXPIRED

            ' --- Noodles & Pasta (categoryId 3) ---
            products.Add(New SeedProductSpec("PRD011", "Lucky Me! Pancit Canton Chili-Mansi 60g", 3, 2, 16D, 13D, 90, 24, 200))
            products.Add(New SeedProductSpec("PRD012", "Lucky Me! Beef Noodles 60g", 3, 2, 15D, 12D, 75, 24, 200))
            products.Add(New SeedProductSpec("PRD013", "Payless Xtra Big Zesty Cheese 85g", 3, 1, 22D, 18D, 50, 15, 160))
            products.Add(New SeedProductSpec("PRD014", "El Real Elbow Macaroni 1kg", 3, 3, 92D, 78D, 35, 10, 365))
            products.Add(New SeedProductSpec("PRD015", "Barilla Fettuccine 500g", 3, 3, 128D, 108D, 18, 8, 300))

            ' --- Beverages & Drinks (categoryId 4) ---
            products.Add(New SeedProductSpec("PRD016", "Coca-Cola Mabuhay 1.5L", 4, 1, 75D, 62D, 60, 15, 150))
            products.Add(New SeedProductSpec("PRD017", "Sprite 1.5L", 4, 1, 75D, 62D, 45, 12, 150))
            products.Add(New SeedProductSpec("PRD018", "Royal Tru-Orange 1L", 4, 1, 62D, 50D, 38, 10, 120))
            products.Add(New SeedProductSpec("PRD019", "C2 Apple Green 230ml", 4, 2, 14D, 11D, 120, 30, 180))
            products.Add(New SeedProductSpec("PRD020", "Wilkins Distilled Water 1L", 4, 1, 28D, 22D, 140, 36, noExpiry))

            ' --- Snacks & Biscuits (categoryId 5) ---
            products.Add(New SeedProductSpec("PRD021", "Piattos Cheese 85g", 5, 2, 40D, 32D, 70, 18, 105))
            products.Add(New SeedProductSpec("PRD022", "Nova Multigrain Cheese 88g", 5, 2, 40D, 33D, 52, 15, 95))
            products.Add(New SeedProductSpec("PRD023", "Chippy BBQ 98g", 5, 2, 21D, 17D, 6, 12, 120))            ' LOW STOCK
            products.Add(New SeedProductSpec("PRD024", "Sky Flakes Crackers 10s", 5, 1, 34D, 27D, 65, 20, 140))
            products.Add(New SeedProductSpec("PRD025", "Rebisco Sandwich Choco 10s", 5, 3, 46D, 38D, 30, 10, -5)) ' EXPIRED

            ' --- Personal Care & Toiletries (categoryId 6) ---
            products.Add(New SeedProductSpec("PRD026", "Safeguard Pure White Soap 125g", 6, 3, 29D, 23D, 60, 15, 365))
            products.Add(New SeedProductSpec("PRD027", "Colgate Double Action Toothpaste 75ml", 6, 3, 89D, 74D, 55, 12, 540))
            products.Add(New SeedProductSpec("PRD028", "Palmolive Naturals Shampoo 180ml", 6, 1, 96D, 80D, 42, 12, noExpiry))
            products.Add(New SeedProductSpec("PRD029", "Sunsilk Sachet Display Box 12s", 6, 1, 48D, 40D, 5, 15, 300))  ' LOW STOCK
            products.Add(New SeedProductSpec("PRD030", "Head & Shoulders Sachet Box 12s", 6, 3, 52D, 44D, 2, 15, 280)) ' LOW STOCK

            Return products
        End Function

#End Region

    End Class

End Namespace
