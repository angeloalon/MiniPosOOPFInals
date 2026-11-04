Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Data/UserRepository.vb — Identity persistence for the User family
' (spec §7.2). The ONLY file in the solution allowed to touch the
' Users table; every statement is parameterized (@param +
' Parameters.AddWithValue) — zero string-built SQL.
'
' OOP concept: POLYMORPHISM (role string → subclass factory):
'   A stored row is just a role string {"StoreOwner" | "Cashier" |
'   "InventoryClerk" | "Customer"}. The factory dispatches that string
'   to the RIGHT concrete constructor, so every read hands back a real
'   subclass instance — never the MustInherit base — and consumers
'   (AuthService at M6, the Owner staff grid at M14) program against
'   `User`/`Customer` while LoginForm routes via GetDashboardForm()
'   with zero role branching.
'
' OOP concept: ENCAPSULATION (static gate + guarded writes):
'   NotInheritable + Private Sub New — no repository instances, the
'   visible surface is Shared members only. CreateUser refuses empty
'   credential strings and taken usernames; UpdateUser never touches
'   Role or the credential columns (passwords change ONLY through
'   UpdatePassword, so a profile save can never wipe a hash); a
'   rename that grabs someone else's unique username is rejected with
'   a friendly message instead of a raw SQLite constraint error.
'
' LAYERING: Data → Models only. This file references the User family
' and nothing else (no Forms, no Services — those reach it the other
' way through the one-way dependency flow of spec §3).
'
' MILESTONE NOTE (M5): DatabaseHelper.vb (M7) becomes the connection
' owner. Until it lands, CreateConnection() uses the same canonical DB
' path + connection string as spec §7.1/§8 — so the eventual swap to
' DatabaseHelper.GetConnection() is a one-line change. Nothing calls
' this repository yet: it is inert by design until M6/M7, and the DB
' file itself does not exist either (expected).
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Data.SQLite
Imports oopfinals.Models

Namespace Data

    Public NotInheritable Class UserRepository

        ' Static gate: repositories are stateless, never instantiated.
        Private Sub New()
        End Sub

#Region "Role persistence keys (spec §8)"

        ' Exact strings stored in Users.Role. Constants keep the INSERT,
        ' the SELECT filters, and the factory dispatch literal-identical.
        Private Const RoleStoreOwner As String = "StoreOwner"
        Private Const RoleCashier As String = "Cashier"
        Private Const RoleInventoryClerk As String = "InventoryClerk"
        Private Const RoleCustomer As String = "Customer"

#End Region

#Region "Connection (canonical path + string per spec §7.1/§8)"

        ' Fresh, short-lived connection per operation — no shared state.
        ' Foreign Keys=True keeps the §8 referential rules enforced.
        Private Shared Function CreateConnection() As SQLiteConnection
            Dim dbPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "grocery.db")
            Dim connection As New SQLiteConnection("Data Source=" & dbPath & ";Version=3;Foreign Keys=True;")
            connection.Open()
            Return connection
        End Function

#End Region

#Region "Row mapping + role→subclass factory"

        ' Login lookups normalize to trimmed lowercase — exactly the same
        ' shape the model's Username setter guarantees on write.
        Private Shared Function NormalizeUsername(raw As String) As String
            Return If(raw, String.Empty).Trim().ToLowerInvariant()
        End Function

        ' THE factory (spec §7.2): stored role string → concrete subclass.
        ' A role that reached the table but maps to no class is data
        ' corruption — reported loudly, never silently skipped.
        Private Shared Function CreateUserFromRole(roleKey As String,
                                                   userId As Integer,
                                                   username As String,
                                                   fullName As String,
                                                   isActive As Boolean) As User
            Select Case roleKey
                Case RoleStoreOwner
                    Return New StoreOwner(userId, username, fullName, isActive)
                Case RoleCashier
                    Return New Cashier(userId, username, fullName, isActive)
                Case RoleInventoryClerk
                    Return New InventoryClerk(userId, username, fullName, isActive)
                Case RoleCustomer
                    Return New Customer(userId, username, fullName, isActive)
                Case Else
                    Throw New InvalidOperationException("Unknown user role '" & roleKey & "' found in the Users table.")
            End Select
        End Function

        ' Materializes the current reader row into a fully hydrated
        ' subclass instance. CreatedAt is deliberately NOT mapped back:
        ' the model exposes it as a read-only constructor-stamped value
        ' and no consumer needs the stored timestamp. Hash + salt ride
        ' the validated setters (the subclass constructors stamp identity
        ' only), which keeps "Password hash cannot be empty." enforced
        ' even if the DB somehow returned blanks.
        Private Shared Function MapRowToUser(reader As SQLiteDataReader) As User
            Dim userId As Integer = Convert.ToInt32(reader("UserId"))
            Dim username As String = reader("Username").ToString()
            Dim passwordHash As String = reader("PasswordHash").ToString()
            Dim salt As String = reader("Salt").ToString()
            Dim fullName As String = reader("FullName").ToString()
            Dim roleKey As String = reader("Role").ToString()
            Dim isActive As Boolean = Convert.ToBoolean(reader("IsActive"))

            Dim user As User = CreateUserFromRole(roleKey, userId, username, fullName, isActive)
            user.PasswordHash = passwordHash
            user.Salt = salt
            Return user
        End Function

        Private Shared Sub FillUserList(command As SQLiteCommand, target As List(Of User))
            Using reader As SQLiteDataReader = command.ExecuteReader()
                Do While reader.Read()
                    target.Add(MapRowToUser(reader))
                Loop
            End Using
        End Sub

#End Region

#Region "Reads"

        ' Login path (consumed by AuthService.Authenticate at M6). The
        ' role→subclass dispatch fires inside the mapping helper shared
        ' by every read below, so callers never see a raw row.
        Public Shared Function GetByUsername(username As String) As User
            Dim lookup As String = NormalizeUsername(username)
            If lookup.Length = 0 Then
                Return Nothing
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("SELECT UserId, Username, PasswordHash, Salt, FullName, Role, IsActive FROM Users WHERE Username = @username;", connection)
                    command.Parameters.AddWithValue("@username", lookup)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapRowToUser(reader)
                        End If
                    End Using
                End Using
            End Using

            Return Nothing
        End Function

        Public Shared Function GetById(userId As Integer) As User
            If userId <= 0 Then
                Return Nothing
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("SELECT UserId, Username, PasswordHash, Salt, FullName, Role, IsActive FROM Users WHERE UserId = @userId;", connection)
                    command.Parameters.AddWithValue("@userId", userId)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapRowToUser(reader)
                        End If
                    End Using
                End Using
            End Using

            Return Nothing
        End Function

        ' Staff = StoreOwner + Cashier + InventoryClerk (Owner Staff tab,
        ' M14). Three named parameters even for an IN clause — the list
        ' itself is never interpolated.
        Public Shared Function GetAllStaff() As List(Of User)
            Dim staff As New List(Of User)()

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("SELECT UserId, Username, PasswordHash, Salt, FullName, Role, IsActive FROM Users WHERE Role IN (@roleStoreOwner, @roleCashier, @roleInventoryClerk) ORDER BY FullName;", connection)
                    command.Parameters.AddWithValue("@roleStoreOwner", RoleStoreOwner)
                    command.Parameters.AddWithValue("@roleCashier", RoleCashier)
                    command.Parameters.AddWithValue("@roleInventoryClerk", RoleInventoryClerk)
                    FillUserList(command, staff)
                End Using
            End Using

            Return staff
        End Function

        ' Returned STRONGLY typed: M12 history/profile and M14 flows rely
        ' on Customer, not the abstract base. The filter guarantees every
        ' row dispatches to Customer — a miss means corrupted data.
        Public Shared Function GetAllCustomers() As List(Of Customer)
            Dim customers As New List(Of Customer)()

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("SELECT UserId, Username, PasswordHash, Salt, FullName, Role, IsActive FROM Users WHERE Role = @roleCustomer ORDER BY FullName;", connection)
                    command.Parameters.AddWithValue("@roleCustomer", RoleCustomer)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        Do While reader.Read()
                            Dim customer As Customer = TryCast(MapRowToUser(reader), Customer)
                            If customer Is Nothing Then
                                Throw New InvalidOperationException("Users table contains a 'Customer' row that did not map to a Customer instance.")
                            End If
                            customers.Add(customer)
                        Loop
                    End Using
                End Using
            End Using

            Return customers
        End Function

        ' Signup + Owner "New staff" pre-check (M8/M14). Same unicity as
        ' the UNIQUE constraint — but surfaces as a friendly message
        ' before the insert, not as a raw constraint error after it.
        Public Shared Function UsernameExists(username As String) As Boolean
            Dim lookup As String = NormalizeUsername(username)
            If lookup.Length = 0 Then
                Return False
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("SELECT COUNT(1) FROM Users WHERE Username = @username;", connection)
                    command.Parameters.AddWithValue("@username", lookup)
                    Return Convert.ToInt64(command.ExecuteScalar()) > 0
                End Using
            End Using
        End Function

#End Region

#Region "Writes"

        ' Polymorphic insert (spec §7.2): the Role column comes from the
        ' RUNTIME type (stamped in each subclass constructor), so the
        ' same call persists any member of the User family. Returns the
        ' new Users.UserId.
        Public Shared Function CreateUser(user As User) As Integer
            If user Is Nothing Then
                Throw New ArgumentNullException(NameOf(user))
            End If

            ' Fence: a user assembled without touching the validating
            ' setters would carry empty credential strings past the
            ' getters — refuse to persist that (model's own messages).
            If String.IsNullOrWhiteSpace(user.PasswordHash) Then
                Throw New ArgumentException("Password hash cannot be empty.")
            End If
            If String.IsNullOrWhiteSpace(user.Salt) Then
                Throw New ArgumentException("Salt cannot be empty.")
            End If

            If UsernameExists(user.Username) Then
                Throw New InvalidOperationException("Username '" & user.Username & "' is already taken.")
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("INSERT INTO Users (Username, PasswordHash, Salt, FullName, Role, IsActive, CreatedAt) VALUES (@username, @passwordHash, @salt, @fullName, @role, @isActive, @createdAt);", connection)
                    command.Parameters.AddWithValue("@username", user.Username)
                    command.Parameters.AddWithValue("@passwordHash", user.PasswordHash)
                    command.Parameters.AddWithValue("@salt", user.Salt)
                    command.Parameters.AddWithValue("@fullName", user.FullName)
                    command.Parameters.AddWithValue("@role", user.Role)
                    command.Parameters.AddWithValue("@isActive", user.IsActive)
                    command.Parameters.AddWithValue("@createdAt", user.CreatedAt)
                    command.ExecuteNonQuery()
                End Using

                Using idCommand As New SQLiteCommand("SELECT last_insert_rowid();", connection)
                    Return Convert.ToInt32(idCommand.ExecuteScalar())
                End Using
            End Using
        End Function

        ' Profile/staff edits: username, display name and active flag
        ' only. Credentials are NEVER touched here — that path is
        ' UpdatePassword() below — and Role is immutable by design
        ' (stamped once in the subclass constructor).
        Public Shared Function UpdateUser(user As User) As Boolean
            If user Is Nothing Then
                Throw New ArgumentNullException(NameOf(user))
            End If

            If UsernameExistsForOtherUser(user.Username, user.UserId) Then
                Throw New InvalidOperationException("Username '" & user.Username & "' is already taken by another account.")
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("UPDATE Users SET Username = @username, FullName = @fullName, IsActive = @isActive WHERE UserId = @userId;", connection)
                    command.Parameters.AddWithValue("@username", user.Username)
                    command.Parameters.AddWithValue("@fullName", user.FullName)
                    command.Parameters.AddWithValue("@isActive", user.IsActive)
                    command.Parameters.AddWithValue("@userId", user.UserId)
                    Return command.ExecuteNonQuery() > 0
                End Using
            End Using
        End Function

        ' Change-password (M12 Profile) + Owner reset-password (M14).
        ' Hash and salt travel together — half a credential pair is
        ' invalid — so both are required, like the model enforces.
        Public Shared Function UpdatePassword(userId As Integer, newHash As String, newSalt As String) As Boolean
            If String.IsNullOrWhiteSpace(newHash) Then
                Throw New ArgumentException("Password hash cannot be empty.")
            End If
            If String.IsNullOrWhiteSpace(newSalt) Then
                Throw New ArgumentException("Salt cannot be empty.")
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("UPDATE Users SET PasswordHash = @passwordHash, Salt = @salt WHERE UserId = @userId;", connection)
                    command.Parameters.AddWithValue("@passwordHash", newHash)
                    command.Parameters.AddWithValue("@salt", newSalt)
                    command.Parameters.AddWithValue("@userId", userId)
                    Return command.ExecuteNonQuery() > 0
                End Using
            End Using
        End Function

        ' Owner deactivate/reactivate (M14). The deactivated message
        ' lives in AuthService.Authenticate (M6), not here.
        Public Shared Function SetActiveStatus(userId As Integer, isActive As Boolean) As Boolean
            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("UPDATE Users SET IsActive = @isActive WHERE UserId = @userId;", connection)
                    command.Parameters.AddWithValue("@isActive", isActive)
                    command.Parameters.AddWithValue("@userId", userId)
                    Return command.ExecuteNonQuery() > 0
                End Using
            End Using
        End Function

        ' Same statement shape as UsernameExists, scoped so a row's OWN
        ' username never counts against its own rename.
        Private Shared Function UsernameExistsForOtherUser(username As String, excludeUserId As Integer) As Boolean
            Dim lookup As String = NormalizeUsername(username)
            If lookup.Length = 0 Then
                Return False
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("SELECT COUNT(1) FROM Users WHERE Username = @username AND UserId <> @userId;", connection)
                    command.Parameters.AddWithValue("@username", lookup)
                    command.Parameters.AddWithValue("@userId", excludeUserId)
                    Return Convert.ToInt64(command.ExecuteScalar()) > 0
                End Using
            End Using
        End Function

#End Region

    End Class

End Namespace
