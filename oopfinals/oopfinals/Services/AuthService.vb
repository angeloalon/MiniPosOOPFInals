Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Services/AuthService.vb — authentication + registration with PBKDF2
' credential hashing (spec §9.1, ~124 lines). THE only place plaintext
' passwords are ever touched; they are converted to salted PBKDF2
' hashes and immediately discarded — never stored, logged, displayed.
'
' OOP concept: ENCAPSULATION (static gate):
'   NotInheritable + Private Sub New — the visible surface is Shared
'   members only; there are no AuthService instances to leak state.
'
' LAYERING: Services → Data → Models (one-way flow, spec §3). This
' file consumes UserRepository and the User family but is consumed by
' Forms only (Login M8, Register M8, Owner staff M14, Profile M12).
'
' SECURITY NOTES (spec §13):
'   - 16-byte cryptographically random salt PER USER (Base64), stored
'     separately from the 32-byte hash.
'   - PBKDF2-HMAC-SHA256, 10,000 iterations, via Rfc2898DeriveBytes.
'   - Verify recomputes and compares with SlowEquals — a constant-time,
'     single-decision compare that gives timing attacks nothing to read.
' =====================================================================

Imports System
Imports System.Security.Cryptography
Imports oopfinals.Data
Imports oopfinals.Models

Namespace Services

    Public NotInheritable Class AuthService

        ' Static utility: never instantiated (spec §4).
        Private Sub New()
        End Sub

#Region "Crypto constants (spec §9.1)"

        ' Salt: 16 bytes → Base64 string in the Users.Salt column.
        ' Hash: 32 bytes derived key → Base64 in the PasswordHash column.
        ' Iterations: PBKDF2 work factor for this demo scale.
        Public Const SaltSize As Integer = 16
        Public Const HashSize As Integer = 32
        Public Const Iterations As Integer = 10000

#End Region

#Region "Salt + hash + verify"

        ' Cryptographically random 16 bytes, exposed as Base64 (the same
        ' string shape the Users table stores).
        Public Shared Function GenerateSalt() As String
            Dim saltBytes(SaltSize - 1) As Byte

            Using generator As RandomNumberGenerator = RandomNumberGenerator.Create()
                generator.GetBytes(saltBytes)
            End Using

            Return Convert.ToBase64String(saltBytes)
        End Function

        ' PBKDF2 → 32-byte derived key → Base64. Throws on an empty
        ' password (there is no such credential, ever).
        Public Shared Function HashPassword(password As String, saltBase64 As String) As String
            If String.IsNullOrEmpty(password) Then
                Throw New ArgumentException("Password cannot be empty.")
            End If

            If String.IsNullOrWhiteSpace(saltBase64) Then
                Throw New ArgumentException("Salt cannot be empty.")
            End If

            Dim saltBytes As Byte() = Convert.FromBase64String(saltBase64)
            Dim hashBytes As Byte()

            Using pbkdf2 As New Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256)
                hashBytes = pbkdf2.GetBytes(HashSize)
            End Using

            Return Convert.ToBase64String(hashBytes)
        End Function

        ' Recompute the derived key with the SAME salt and compare in
        ' constant time. Any missing piece means "no match" — never a
        ' crash, never a leak of how wrong the answer was.
        Public Shared Function VerifyPassword(password As String, salt As String, expectedHash As String) As Boolean
            If String.IsNullOrEmpty(password) OrElse String.IsNullOrEmpty(salt) OrElse String.IsNullOrEmpty(expectedHash) Then
                Return False
            End If

            Try
                Dim computed As String = HashPassword(password, salt)
                Return SlowEquals(computed, expectedHash)
            Catch ex As FormatException
                ' Corruption in the Base64 credential text (salt or hash)
                ' can never represent a match — report a clean mismatch.
                Return False
            End Try
        End Function

        ' Constant-time compare (spec §9.1): XOR the lengths once, then
        ' accumulate every character XOR into one cumulative OR, and
        ' decide exactly ONCE at the end — equality, not position, is
        ' the only thing observable through timing.
        Private Shared Function SlowEquals(valueA As String, valueB As String) As Boolean
            Dim diff As Integer = valueA.Length Xor valueB.Length
            Dim shortest As Integer = If(valueA.Length < valueB.Length, valueA.Length, valueB.Length)

            For index As Integer = 0 To shortest - 1
                Dim charDiff As Integer = Convert.ToInt32(valueA(index)) Xor Convert.ToInt32(valueB(index))
                diff = diff Or charDiff
            Next

            Return diff = 0
        End Function

#End Region

#Region "Authenticate"

        ' Login heart (spec §9.1): fence → lookup → active gate →
        ' constant-time verify → return the POLYMORPHIC subclass.
        ' Any ordinary miss is Nothing; ONLY a deactivated account
        ' throws, and it throws the exact spec message.
        Public Shared Function Authenticate(username As String, plainPassword As String) As User
            If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(plainPassword) Then
                Return Nothing
            End If

            ' UserRepository lowercases on lookup and hands back a real
            ' subclass instance with hash + salt hydrated.
            Dim user As User = UserRepository.GetByUsername(username)
            If user Is Nothing Then
                Return Nothing
            End If

            ' Third gate (spec §8): rejected BEFORE password comparison
            ' can succeed — an inactive account never even gets there.
            If Not user.IsActive Then
                Throw New InvalidOperationException("This account has been deactivated. Please contact the Store Owner.")
            End If

            If Not VerifyPassword(plainPassword, user.Salt, user.PasswordHash) Then
                Return Nothing
            End If

            Return user
        End Function

#End Region

#Region "RegisterCustomer"

        ' Public self-signup (spec §9.1): validate through ValidationRules
        ' (the single source of truth), reject taken usernames friendly,
        ' salt+hash, build a Customer, insert via CreateUser.
        '   ArgumentException  — a friendly rule violation message.
        '   InvalidOperationException — username already taken.
        Public Shared Function RegisterCustomer(username As String, plainPassword As String, fullName As String) As Customer
            If Not ValidationRules.IsValidUsername(username) Then
                Throw New ArgumentException("Username must be 3-20 characters and may only contain letters, numbers, dots (.) and underscores (_).", NameOf(username))
            End If

            If Not ValidationRules.IsValidPassword(plainPassword) Then
                Throw New ArgumentException("Password must be at least 6 characters and contain no whitespace.", NameOf(plainPassword))
            End If

            If Not ValidationRules.IsValidFullName(fullName) Then
                Throw New ArgumentException("Full name may only contain letters, spaces, hyphens, apostrophes, dots and commas.", NameOf(fullName))
            End If

            ' Friendly pre-check before persistence — the UNIQUE
            ' constraint stays as the last physical line of defense.
            If UserRepository.UsernameExists(username) Then
                Throw New InvalidOperationException("Username '" & username & "' is already taken.")
            End If

            Dim salt As String = GenerateSalt()
            Dim passwordHash As String = HashPassword(plainPassword, salt)

            ' 0 = no id yet; CreateUser stamps the real Users.UserId via
            ' last_insert_rowid(), which is copied back so the caller
            ' receives a fully hydrated Customer. The validating setters
            ' keep an empty hash unreachable even at this boundary.
            Dim customer As New Customer(0, username, fullName, True)
            customer.PasswordHash = passwordHash
            customer.Salt = salt

            Dim newUserId As Integer = UserRepository.CreateUser(customer)
            customer.UserId = newUserId
            Return customer
        End Function

#End Region

    End Class

End Namespace
