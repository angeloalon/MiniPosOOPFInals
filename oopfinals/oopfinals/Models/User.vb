Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/User.vb — Abstract base class of the user family.
'
' OOP concept: ABSTRACTION (the heart of the project):
'   - Every dashboard/permission consumer programs against THIS base
'     type. GetRoleName / GetDashboardForm / GetPermissions are the
'     "what" without the "how" — each concrete subclass supplies its
'     own implementation and the runtime resolves them late-bound.
'
' OOP concept: ENCAPSULATION:
'   - Private backing fields; setters validate and throw so an invalid
'     User state is impossible to construct. Role is stamped once at
'     construction and never mutated afterwards (getter-only).
'
' LAYERING NOTE (spec §3 vs §6.2): this class touches no UI types
' except the Form return type of GetDashboardForm(), which §6.1
' mandates. The concrete subclasses construct their dashboards —
' that is the ONE deliberate Models→UI edge (see each subclass file).
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

Namespace Models

    Public MustInherit Class User

#Region "Backing fields"

        Private _userId As Integer
        Private _username As String = String.Empty
        Private _passwordHash As String = String.Empty
        Private _salt As String = String.Empty
        Private _fullName As String = String.Empty
        Private _role As String = String.Empty
        Private _isActive As Boolean = True
        Private _createdAt As Date = Date.Now

#End Region

#Region "Constructors"

        ' Default: active account created right now (spec §6.1).
        Protected Sub New()
            _isActive = True
            _createdAt = Date.Now
        End Sub

        ' Full-constructor chains the default ctor and stamps Role.
        Protected Sub New(userId As Integer,
                          username As String,
                          fullName As String,
                          role As String,
                          isActive As Boolean)
            Me.New()

            _userId = userId
            _username = If(username, String.Empty).Trim().ToLowerInvariant()
            _fullName = If(fullName, String.Empty).Trim()
            _role = If(role, String.Empty)
            _isActive = isActive

            ' Constructor-time validation mirrors the property setters
            ' exactly, so an invalid User can never be constructed.
            If _username.Length = 0 Then
                Throw New ArgumentException("Username cannot be empty.")
            End If
            If _fullName.Length = 0 Then
                Throw New ArgumentException("Full name cannot be empty.")
            End If
            If _role.Length = 0 Then
                Throw New ArgumentException("Role cannot be empty.")
            End If
        End Sub

#End Region

#Region "Properties (validated)"

        Public Property UserId As Integer
            Get
                Return _userId
            End Get
            Set(value As Integer)
                _userId = value
            End Set
        End Property

        ' Normalized: trimmed + lowercase (login lookups rely on this).
        Public Property Username As String
            Get
                Return _username
            End Get
            Set(value As String)
                Dim cleaned As String = If(value, String.Empty).Trim().ToLowerInvariant()
                If cleaned.Length = 0 Then
                    Throw New ArgumentException("Username cannot be empty.")
                End If
                _username = cleaned
            End Set
        End Property

        ' PBKDF2 hash only — plaintext passwords never exist in Models
        ' (AuthService owns hashing, landing at M6).
        Public Property PasswordHash As String
            Get
                Return _passwordHash
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("Password hash cannot be empty.")
                End If
                _passwordHash = value
            End Set
        End Property

        ' Per-user salt kept SEPARATE from the hash (spec §13).
        Public Property Salt As String
            Get
                Return _salt
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("Salt cannot be empty.")
                End If
                _salt = value
            End Set
        End Property

        Public Property FullName As String
            Get
                Return _fullName
            End Get
            Set(value As String)
                Dim cleaned As String = If(value, String.Empty).Trim()
                If cleaned.Length = 0 Then
                    Throw New ArgumentException("Full name cannot be empty.")
                End If
                _fullName = cleaned
            End Set
        End Property

        ' Persistence key per spec §8 ("StoreOwner"/"Cashier"/"InventoryClerk"/
        ' "Customer"). Stamped once by each concrete constructor; display
        ' naming goes through GetRoleName() so the two can never be conflated.
        Public ReadOnly Property Role As String
            Get
                Return _role
            End Get
        End Property

        ' Third authorization gate (M14 deactivate/reactivate).
        Public Property IsActive As Boolean
            Get
                Return _isActive
            End Get
            Set(value As Boolean)
                _isActive = value
            End Set
        End Property

        Public ReadOnly Property CreatedAt As Date
            Get
                Return _createdAt
            End Get
        End Property

#End Region

#Region "Abstract contract (Polymorphism #1 showcase)"

        ' Display-facing role name, e.g. "Store Owner" (spec §6.2).
        Public MustOverride Function GetRoleName() As String

        ' Factory-method flavour: produces the right dashboard per RUNTIME
        ' type — LoginForm routes with `user.GetDashboardForm()` and zero
        ' role branching (spec §5.4, live at M8).
        Public MustOverride Function GetDashboardForm() As Form

        ' The keys this role may act on (exact strings per spec §6.2/§11).
        Public MustOverride Function GetPermissions() As List(Of String)

        ' Late-bound dispatch back into the concrete subclass: membership
        ' test against whatever GetPermissions() returns RIGHT NOW.
        Public Overridable Function HasPermission(permissionKey As String) As Boolean
            If String.IsNullOrWhiteSpace(permissionKey) Then
                Return False
            End If
            Return GetPermissions().Contains(permissionKey)
        End Function

        ' "Maria Santos (Store Owner) - @owner" — AppHeader caption format.
        Public Overrides Function ToString() As String
            Return _fullName & " (" & GetRoleName() & ") - @" & _username
        End Function

#End Region

    End Class

End Namespace
