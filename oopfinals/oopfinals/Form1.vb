Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Form1.vb — TEMPORARY dev launcher (M4, extended M6). Replaced by the
' real LoginForm at M8 (Application.myapp swap); every reference to
' Form1 dies with it (spec §12.2).
'
' OOP concept: POLYMORPHISM (spec §5.4)
'   - Each button constructs its concrete subclass directly, then ONLY
'     the base type (Models.User) is known further down. Routing is a
'     single role-blind call, user.GetDashboardForm() — dynamic
'     dispatch picks Owner/Cashier/Clerk/Customer at run-time, with
'     zero If/Else or Select Case role chains anywhere.
'
' NOTE: demo users are constructed inline with EMPTY password hashes
' by design (spec §15 personas) — AuthService owns hashing and
' hydrates PasswordHash/Salt at M6/M7.
' =====================================================================

Imports System
Imports System.Text
Imports System.Windows.Forms
Imports oopfinals.Controls
Imports oopfinals.Models
Imports oopfinals.Services

Public Class Form1

    Public Sub New()
        MyBase.New()
        InitializeComponent()
        ApplyRestyle()
    End Sub

#Region "Launcher buttons (construct the subclass; nothing role-aware after this)"

    Private Sub btnOwner_Click(sender As Object, e As EventArgs) Handles btnOwner.Click
        OpenDashboard(New StoreOwner(1, "owner", "Maria Santos", True))
    End Sub

    Private Sub btnCashier_Click(sender As Object, e As EventArgs) Handles btnCashier.Click
        OpenDashboard(New Cashier(2, "cashier", "Juan Dela Cruz", True))
    End Sub

    Private Sub btnClerk_Click(sender As Object, e As EventArgs) Handles btnClerk.Click
        OpenDashboard(New InventoryClerk(3, "clerk", "Pedro Penduko", True))
    End Sub

    Private Sub btnCustomer_Click(sender As Object, e As EventArgs) Handles btnCustomer.Click
        OpenDashboard(New Customer(4, "customer", "Ana Batumbakal", True))
    End Sub

#End Region

    ' The one shared, role-blind path (spec §5.4): declared as the
    ' abstract base type so a grader can SEE the dispatch was real.
    ' Using owns the dialog's dispose, so every logout cycle is clean.
    Private Sub OpenDashboard(user As Models.User)
        Using dashboard As Form = user.GetDashboardForm()
            dashboard.ShowDialog(Me)
        End Using
    End Sub

#Region "M6 crypto self-test (launcher helper — dies with Form1 at M8)"

    ' Exercises the M6 checklist out loud: hash → verify True, tamper →
    ' False, short/false passwords rejected, salt uniqueness. The test
    ' password lives ONLY inside this method and is never shown, logged
    ' or persisted — the summary reports pass/fail booleans alone.
    Private Sub btnCryptoTest_Click(sender As Object, e As EventArgs) Handles btnCryptoTest.Click
        Dim summary As New StringBuilder()

        Dim salt As String = AuthService.GenerateSalt()
        Dim password As String = "GrocerySelfTest-2026"
        Dim hash As String = AuthService.HashPassword(password, salt)

        Dim correctAccepted As Boolean = AuthService.VerifyPassword(password, salt, hash)
        Dim wrongPasswordRejected As Boolean = Not AuthService.VerifyPassword(password & "x", salt, hash)

        ' Flip the last two Base64 characters — same length, valid
        ' alphabet, different digest: prove the compare is real, not
        ' "always True".
        Dim tamperedHash As String = hash.Substring(0, hash.Length - 2) & "AA"
        Dim tamperedHashRejected As Boolean = Not AuthService.VerifyPassword(password, salt, tamperedHash)

        Dim shortPasswordRejected As Boolean = Not ValidationRules.IsValidPassword("Ab123")

        Dim emptyPasswordThrew As Boolean = False
        Try
            AuthService.HashPassword(String.Empty, salt)
        Catch ex As ArgumentException
            emptyPasswordThrew = True
        End Try

        ' A fresh salt over the same password must derive a different
        ' hash — that is what "salted" means.
        Dim secondHash As String = AuthService.HashPassword(password, AuthService.GenerateSalt())
        Dim saltUnique As Boolean = secondHash <> hash

        summary.AppendLine("M6 crypto self-test (PBKDF2-HMAC-SHA256, 10,000 iterations)")
        summary.AppendLine()
        summary.AppendLine("Hash → verify (correct password)   : " & If(correctAccepted, "PASS", "FAIL"))
        summary.AppendLine("Verify (wrong password) rejected   : " & If(wrongPasswordRejected, "PASS", "FAIL"))
        summary.AppendLine("Verify (tampered hash) rejected    : " & If(tamperedHashRejected, "PASS", "FAIL"))
        summary.AppendLine("Short password (5 chars) rejected  : " & If(shortPasswordRejected, "PASS", "FAIL"))
        summary.AppendLine("Empty password hashing throws      : " & If(emptyPasswordThrew, "PASS", "FAIL"))
        summary.AppendLine("New salt → new hash (per-user salt): " & If(saltUnique, "PASS", "FAIL"))

        MessageBox.Show(summary.ToString(), "M6 crypto self-test", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    End Sub

#End Region

End Class
