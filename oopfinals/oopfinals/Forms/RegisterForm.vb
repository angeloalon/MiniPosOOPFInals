Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Forms/RegisterForm.vb — customer self-signup (M8, spec §10.2/§9.1):
' username / full name / password + confirm, validated by
' Services.ValidationRules (the single source of truth) and persisted
' through AuthService.RegisterCustomer.
'
' OOP concept: ENCAPSULATION — the form contains zero rule logic; it
' only collects input and displays the messages the shared
' validators and AuthService return. Inline error labels keep every
' failure user-friendly instead of a raw constraint error.
'
' LAYERING: Forms → Services (ValidationRules, AuthService) — Models
' are reached only through the Customer instance AuthService returns.
'
' UX filters (spec §10.2): letters-only full name via KeyPress;
' space-typing blocked in the credential fields; show-password
' checkbox; created username surfaces back on the login form through
' the CreatedUsername property.
' =====================================================================

Imports System
Imports System.Windows.Forms
Imports oopfinals.Controls
Imports oopfinals.Models
Imports oopfinals.Services

Namespace Forms

    Public Class RegisterForm

        ' Communication channel back to the login form: after a
        ' successful registration it can greet the new account by name.
        ' (Private mutable field, read-only property — outside code can
        ' see the result but never change it.)
        Private _createdUsername As String = String.Empty

        Public ReadOnly Property CreatedUsername As String
            Get
                Return _createdUsername
            End Get
        End Property

        Public Sub New()
            MyBase.New()
            InitializeComponent()
            ApplyRestyle()
        End Sub

#Region "Registration"

        Private Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
            ' Trim the identity fields but NEVER the password: spaces are
            ' already unreachable there via KeyPress, and silencing
            ' trailing spaces would mis-state what was asked of the user.
            Dim username As String = txtUsername.Text.Trim()
            Dim fullName As String = txtFullName.Text.Trim()
            Dim password As String = txtPassword.Text
            Dim confirm As String = txtConfirm.Text

            ' Field-first inline errors (spec: friendly, specific).
            If Not ValidationRules.IsValidUsername(username) Then
                ShowError("Username must be 3-20 characters and may only contain letters, numbers, dots (.) and underscores (_).")
                txtUsername.Focus()
                Return
            End If

            If Not ValidationRules.IsValidFullName(fullName) Then
                ShowError("Full name may only contain letters, spaces, hyphens, apostrophes, dots and commas.")
                txtFullName.Focus()
                Return
            End If

            If Not ValidationRules.IsValidPassword(password) Then
                ShowError("Password must be at least 6 characters and contain no whitespace.")
                txtPassword.Focus()
                Return
            End If

            If password <> confirm Then
                ShowError("Passwords do not match.")
                txtConfirm.Focus()
                Return
            End If

            ' AuthService raises ArgumentException (rule violation),
            ' InvalidOperationException (username taken) or propagates a
            ' database failure — each mapped to its own friendly text.
            Dim registered As Customer
            Try
                registered = AuthService.RegisterCustomer(username, password, fullName)
            Catch ex As InvalidOperationException
                ShowError(ex.Message)
                txtUsername.Focus()
                Return
            Catch ex As ArgumentException
                ShowError(ex.Message)
                Return
            Catch ex As Exception
                ShowError("Could not create the account: " & ex.Message)
                Return
            End Try

            ' Account created — hand the username back to the login form
            ' and close with OK so the caller reacts accordingly.
            _createdUsername = registered.Username

            MessageBox.Show("Welcome, " & registered.FullName & "! Your customer account is ready — please log in.",
                            "Account created", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

#End Region

#Region "Input filters (spec §10.2)"

        ' Letters-only full name: block everything ValidationRules would
        ' reject (the shared validator remains the final authority —
        ' paste and IME input can still bypass KeyPress).
        Private Sub txtFullName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtFullName.KeyPress
            If Char.IsControl(e.KeyChar) Then
                Return
            End If

            If Not Char.IsLetter(e.KeyChar) AndAlso
               e.KeyChar <> " "c AndAlso e.KeyChar <> "-"c AndAlso
               e.KeyChar <> "'"c AndAlso e.KeyChar <> "."c AndAlso
               e.KeyChar <> ","c Then
                e.Handled = True
            End If
        End Sub

        ' Credential fields never accept whitespace.
        Private Sub BlockCredentialSpaces(sender As Object, e As KeyPressEventArgs) Handles txtUsername.KeyPress, txtPassword.KeyPress, txtConfirm.KeyPress
            If Char.IsWhiteSpace(e.KeyChar) Then
                e.Handled = True
            End If
        End Sub

        Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPassword.CheckedChanged
            txtPassword.UseSystemPasswordChar = Not chkShowPassword.Checked
            txtConfirm.UseSystemPasswordChar = Not chkShowPassword.Checked
        End Sub

#End Region

#Region "Back to login"

        Private Sub lnkBackToLogin_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkBackToLogin.LinkClicked
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Sub

#End Region

#Region "Error label"

        Private Sub ShowError(message As String)
            lblError.Text = message
            lblError.ForeColor = Theme.ErrorRed
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
            Me.lblFullName.Font = Theme.FontFor(ThemeWeight.Semibold, 9.0F)
            Me.lblFullName.ForeColor = Theme.Ink
            Me.lblPassword.Font = Theme.FontFor(ThemeWeight.Semibold, 9.0F)
            Me.lblPassword.ForeColor = Theme.Ink
            Me.lblConfirm.Font = Theme.FontFor(ThemeWeight.Semibold, 9.0F)
            Me.lblConfirm.ForeColor = Theme.Ink

            StyledInputs.StyleTextBox(Me.txtUsername)
            StyledInputs.StyleTextBox(Me.txtFullName)
            StyledInputs.StyleTextBox(Me.txtPassword)
            StyledInputs.StyleTextBox(Me.txtConfirm)
            Me.txtPassword.UseSystemPasswordChar = True
            Me.txtConfirm.UseSystemPasswordChar = True

            Me.chkShowPassword.Font = Theme.FontFor(ThemeWeight.Regular, 9.0F)
            Me.chkShowPassword.ForeColor = Theme.Ink

            Me.btnRegister.Kind = ThemeButtonKind.Primary
            Me.btnRegister.Font = Theme.FontFor(ThemeWeight.Semibold, 11.0F)

            Me.lblError.Font = Theme.FontFor(ThemeWeight.Regular, 9.0F)
            Me.lblError.ForeColor = Theme.ErrorRed

            Me.lnkBackToLogin.Font = Theme.FontFor(ThemeWeight.Semibold, 9.0F)
            Me.lnkBackToLogin.LinkColor = Theme.Green
            Me.lnkBackToLogin.ActiveLinkColor = Theme.GreenPress
            Me.lnkBackToLogin.LinkBehavior = LinkBehavior.HoverUnderline
        End Sub

#End Region

    End Class

End Namespace
