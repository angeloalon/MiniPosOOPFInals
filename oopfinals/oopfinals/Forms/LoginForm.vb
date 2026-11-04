Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Forms/LoginForm.vb — the app's real front door (M8): database
' bootstrap on load, credential pre-checks, brute-force lockout and
' the POLYMORPHIC dashboard-routing showcase (spec §5.4 / §12.2).
'
' OOP concept: POLYMORPHISM #1 (specs §5.4/§11) — after AuthService
' returns a concrete subclass typed AS the abstract base
' (oopfinals.Models.User), the only dispatch this form performs is
' user.GetDashboardForm(). No If/Else or Select Case on roles exists
' anywhere in the flow; the RUNTIME type decides which dashboard
' (StoreOwner / Cashier / InventoryClerk / Customer) opens.
'
' LAYERING: Forms → Services (AuthService) / Data (DatabaseHelper) →
' Models (User family) — the one-way dependency flow of spec §3.
'
' SECURITY NOTES (spec §13): the typed password lives only in the
' control and is cleared on every logout; five consecutive failures
' lock the credential inputs for 30 seconds; the deactivated-account
' message surfaces as-is from AuthService and is never logged.
' =====================================================================

Imports System
Imports System.Windows.Forms
Imports oopfinals.Controls
Imports oopfinals.Data
Imports oopfinals.Models
Imports oopfinals.Services

Namespace Forms

    Public Class LoginForm

        ' Brute-force guard (spec §10.2): five consecutive failures
        ' lock the credential inputs for 30 seconds. The countdown is
        ' driven by a lightweight form-level timer (created in code —
        ' no designer component needed).
        Private _failedAttempts As Integer
        Private _lockoutUntil As DateTime
        Private ReadOnly _lockoutTimer As Timer

        Private Const MaxFailedAttempts As Integer = 5
        Private Const LockoutSeconds As Integer = 30

        Public Sub New()
            MyBase.New()
            InitializeComponent()

            _lockoutTimer = New Timer()
            _lockoutTimer.Interval = 500
            AddHandler _lockoutTimer.Tick, AddressOf OnLockoutTimerTick

            ApplyRestyle()
        End Sub

#Region "Load — database bootstrap (spec §12.1)"

        ' InitializeDatabase() runs on EVERY app start (spec §8): the
        ' DDL is idempotent and first-run seeding is guarded, so the app
        ' self-heals a deleted grocery.db. Failures are captured into
        ' the status label per spec — the form still opens so the user
        ' sees exactly what went wrong instead of a silent crash.
        Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Try
                Dim freshlySeeded As Boolean = DatabaseHelper.InitializeDatabase()

                If freshlySeeded Then
                    SetStatus("Demo database created and seeded — sign in with any demo account.", ThemeTone.Info)
                Else
                    SetStatus("Ready. Enter your username and password.", ThemeTone.Neutral)
                End If
            Catch ex As Exception
                SetStatus("Could not initialize the database: " & ex.Message, ThemeTone.[Error])
            End Try

            Me.ActiveControl = txtUsername
        End Sub

#End Region

#Region "Login → polymorphic routing (spec §12.2)"

        Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
            If IsLockedOut() Then
                UpdateLockoutStatus()
                Return
            End If

            ' Local pre-checks mirror the ValidationRules minimums —
            ' cheap feedback BEFORE any crypto/DB work. They never count
            ' as failed attempts: no credential comparison happened.
            Dim username As String = txtUsername.Text.Trim()

            If username.Length < ValidationRules.MinUsernameLength Then
                SetStatus("Username must be at least 3 characters.", ThemeTone.Warn)
                txtUsername.Focus()
                Return
            End If

            If txtPassword.Text.Length < ValidationRules.MinPasswordLength Then
                SetStatus("Password must be at least 6 characters.", ThemeTone.Warn)
                txtPassword.Focus()
                Return
            End If

            ' THE showcase call (spec §5.4): loggedInUser is declared as
            ' the ABSTRACT base type, and GetDashboardForm() below is
            ' resolved at runtime — a StoreOwner row produces the Owner
            ' dashboard, a Cashier row the POS, etc., with this form
            ' knowing nothing about which one. No role branching.
            Dim loggedInUser As User
            Try
                loggedInUser = AuthService.Authenticate(username, txtPassword.Text)
            Catch ex As InvalidOperationException
                ' Deactivated account message surface — deliberately NOT
                ' counted as a password failure (spec §12.2 flow).
                SetStatus(ex.Message, ThemeTone.[Error])
                Return
            Catch ex As Exception
                SetStatus("Could not reach the login database: " & ex.Message, ThemeTone.[Error])
                Return
            End Try

            If loggedInUser Is Nothing Then
                RegisterFailedAttempt()
                Return
            End If

            ' Success: reset the brute-force counter, then route through
            ' the single role-blind path. Using-style dispose keeps every
            ' logout cycle clean; the loop resets the form for the next
            ' account (logout round-trip, spec §10.2).
            _failedAttempts = 0

            Dim dashboard As Form = loggedInUser.GetDashboardForm()

            Me.Hide()
            Try
                dashboard.ShowDialog()
            Finally
                dashboard.Dispose()
            End Try

            Me.Show()
            txtPassword.Clear()
            chkShowPassword.Checked = False
            Me.ActiveControl = txtUsername
            SetStatus("Logged out successfully.", ThemeTone.Success)
        End Sub

#End Region

#Region "Register link (M8 signup loop)"

        Private Sub lnkRegister_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkRegister.LinkClicked
            Using register As New RegisterForm()
                register.ShowDialog(Me)

                If register.CreatedUsername.Length > 0 Then
                    SetStatus("Account created for '" & register.CreatedUsername & "' — you can now log in.", ThemeTone.Success)
                End If
            End Using
        End Sub

#End Region

#Region "Field behavior"

        ' Credential fields never accept whitespace — a username with a
        ' space or a password with whitespace is wrong by construction.
        Private Sub BlockCredentialSpaces(sender As Object, e As KeyPressEventArgs) Handles txtUsername.KeyPress, txtPassword.KeyPress
            If Char.IsWhiteSpace(e.KeyChar) Then
                e.Handled = True
            End If
        End Sub

        Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPassword.CheckedChanged
            txtPassword.UseSystemPasswordChar = Not chkShowPassword.Checked
        End Sub

#End Region

#Region "Brute-force lockout (spec §10.2)"

        Private Function IsLockedOut() As Boolean
            Return DateTime.Now < _lockoutUntil
        End Function

        ' A failed authentication (verify returned Nothing) counts as
        ' one attempt; the fifth consecutive one locks the input.
        Private Sub RegisterFailedAttempt()
            ' Credential hygiene on every failure, matching the logout
            ' loop in btnLogin_Click: the password box must never carry
            ' the previous attempt's secret into a retry or a lockout —
            ' a shared terminal plus the show-password toggle would
            ' otherwise reveal it with a single click. Lockouts start
            ' here too (fifth failure), so both cases are covered.
            txtPassword.Clear()
            chkShowPassword.Checked = False

            _failedAttempts += 1

            If _failedAttempts >= MaxFailedAttempts Then
                _lockoutUntil = DateTime.Now.AddSeconds(LockoutSeconds)
                SetLockoutState(True)
                _lockoutTimer.Start()
                UpdateLockoutStatus()
            Else
                SetStatus("Incorrect username or password. Attempt " &
                          _failedAttempts.ToString() & " of " &
                          MaxFailedAttempts.ToString() & ".", ThemeTone.[Error])
            End If
        End Sub

        Private Sub SetLockoutState(locked As Boolean)
            txtUsername.Enabled = Not locked
            txtPassword.Enabled = Not locked
            btnLogin.Enabled = Not locked
        End Sub

        Private Sub OnLockoutTimerTick(sender As Object, e As EventArgs)
            If IsLockedOut() Then
                UpdateLockoutStatus()
            Else
                _lockoutTimer.Stop()
                _lockoutUntil = DateTime.MinValue
                _failedAttempts = 0
                SetLockoutState(False)
                SetStatus("Lockout lifted — you may sign in again.", ThemeTone.Info)
            End If
        End Sub

        Private Sub UpdateLockoutStatus()
            Dim secondsLeft As Integer = CInt(Math.Ceiling((_lockoutUntil - DateTime.Now).TotalSeconds))
            SetStatus("Too many failed attempts. Locked out — try again in " &
                      secondsLeft.ToString() & " s.", ThemeTone.[Error])
        End Sub

#End Region

#Region "Status label (token-tinted per tone)"

        Private Sub SetStatus(message As String, tone As ThemeTone)
            Dim fore As Color
            Dim back As Color
            Theme.GetToneColors(tone, fore, back)

            lblStatus.Text = message
            lblStatus.ForeColor = fore
        End Sub

#End Region

#Region "Figma UI restyle (M3 tokens — §10.1)"

        ' Purely visual, token consumption only — no ad-hoc colors.
        Private Sub ApplyRestyle()
            Me.BackColor = Theme.PageBg
            Me.tlpRoot.BackColor = Theme.PageBg

            Me.lblTitle.Font = Theme.FontFor(ThemeWeight.Semibold, 16.0F)
            Me.lblTitle.ForeColor = Theme.Ink
            Me.lblSubtitle.Font = Theme.FontFor(ThemeWeight.Regular, 9.0F)
            Me.lblSubtitle.ForeColor = Theme.Muted

            Me.lblUsername.Font = Theme.FontFor(ThemeWeight.Semibold, 9.0F)
            Me.lblUsername.ForeColor = Theme.Ink
            Me.lblPassword.Font = Theme.FontFor(ThemeWeight.Semibold, 9.0F)
            Me.lblPassword.ForeColor = Theme.Ink

            StyledInputs.StyleTextBox(Me.txtUsername)
            StyledInputs.StyleTextBox(Me.txtPassword)
            Me.txtPassword.UseSystemPasswordChar = True

            Me.chkShowPassword.Font = Theme.FontFor(ThemeWeight.Regular, 9.0F)
            Me.chkShowPassword.ForeColor = Theme.Ink

            Me.btnLogin.Kind = ThemeButtonKind.Primary
            Me.btnLogin.Font = Theme.FontFor(ThemeWeight.Semibold, 11.0F)

            Me.lblStatus.Font = Theme.FontFor(ThemeWeight.Regular, 9.0F)
            Me.lblStatus.ForeColor = Theme.Muted

            Me.lnkRegister.Font = Theme.FontFor(ThemeWeight.Semibold, 9.0F)
            Me.lnkRegister.LinkColor = Theme.Green
            Me.lnkRegister.ActiveLinkColor = Theme.GreenPress
            Me.lnkRegister.LinkBehavior = LinkBehavior.HoverUnderline
        End Sub

#End Region

    End Class

End Namespace
