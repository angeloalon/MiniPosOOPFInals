Option Explicit On
Option Strict On

Namespace Forms

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class RegisterForm
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
            Me.tlpRoot = New System.Windows.Forms.TableLayoutPanel()
            Me.card = New Global.oopfinals.Controls.RoundedPanel()
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.lblSubtitle = New System.Windows.Forms.Label()
            Me.lblUsername = New System.Windows.Forms.Label()
            Me.txtUsername = New System.Windows.Forms.TextBox()
            Me.lblFullName = New System.Windows.Forms.Label()
            Me.txtFullName = New System.Windows.Forms.TextBox()
            Me.lblPassword = New System.Windows.Forms.Label()
            Me.txtPassword = New System.Windows.Forms.TextBox()
            Me.lblConfirm = New System.Windows.Forms.Label()
            Me.txtConfirm = New System.Windows.Forms.TextBox()
            Me.chkShowPassword = New System.Windows.Forms.CheckBox()
            Me.btnRegister = New Global.oopfinals.Controls.StyledButton()
            Me.lblError = New System.Windows.Forms.Label()
            Me.lnkBackToLogin = New System.Windows.Forms.LinkLabel()
            Me.tlpRoot.SuspendLayout()
            Me.card.SuspendLayout()
            Me.SuspendLayout()
            '
            'tlpRoot
            '
            Me.tlpRoot.ColumnCount = 1
            Me.tlpRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.tlpRoot.Controls.Add(Me.card, 0, 0)
            Me.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tlpRoot.Location = New System.Drawing.Point(0, 0)
            Me.tlpRoot.Name = "tlpRoot"
            Me.tlpRoot.RowCount = 1
            Me.tlpRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.tlpRoot.Size = New System.Drawing.Size(984, 661)
            Me.tlpRoot.TabIndex = 0
            '
            'card (Anchor=None → centered by tlpRoot at every window size)
            '
            Me.card.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.card.Controls.Add(Me.lblTitle)
            Me.card.Controls.Add(Me.lblSubtitle)
            Me.card.Controls.Add(Me.lblUsername)
            Me.card.Controls.Add(Me.txtUsername)
            Me.card.Controls.Add(Me.lblFullName)
            Me.card.Controls.Add(Me.txtFullName)
            Me.card.Controls.Add(Me.lblPassword)
            Me.card.Controls.Add(Me.txtPassword)
            Me.card.Controls.Add(Me.lblConfirm)
            Me.card.Controls.Add(Me.txtConfirm)
            Me.card.Controls.Add(Me.chkShowPassword)
            Me.card.Controls.Add(Me.btnRegister)
            Me.card.Controls.Add(Me.lblError)
            Me.card.Controls.Add(Me.lnkBackToLogin)
            Me.card.Location = New System.Drawing.Point(232, 80)
            Me.card.Name = "card"
            Me.card.Size = New System.Drawing.Size(520, 500)
            Me.card.TabIndex = 0
            '
            'lblTitle
            '
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Location = New System.Drawing.Point(32, 28)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "Create a customer account"
            '
            'lblSubtitle
            '
            Me.lblSubtitle.AutoSize = True
            Me.lblSubtitle.Location = New System.Drawing.Point(32, 62)
            Me.lblSubtitle.Name = "lblSubtitle"
            Me.lblSubtitle.TabIndex = 1
            Me.lblSubtitle.Text = "Username and password must contain no spaces"
            '
            'lblUsername
            '
            Me.lblUsername.AutoSize = True
            Me.lblUsername.Location = New System.Drawing.Point(32, 100)
            Me.lblUsername.Name = "lblUsername"
            Me.lblUsername.TabIndex = 2
            Me.lblUsername.Text = "Username"
            '
            'txtUsername
            '
            Me.txtUsername.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtUsername.Location = New System.Drawing.Point(32, 122)
            Me.txtUsername.MaxLength = 20
            Me.txtUsername.Name = "txtUsername"
            Me.txtUsername.Size = New System.Drawing.Size(456, 23)
            Me.txtUsername.TabIndex = 3
            '
            'lblFullName
            '
            Me.lblFullName.AutoSize = True
            Me.lblFullName.Location = New System.Drawing.Point(32, 166)
            Me.lblFullName.Name = "lblFullName"
            Me.lblFullName.TabIndex = 4
            Me.lblFullName.Text = "Full name"
            '
            'txtFullName
            '
            Me.txtFullName.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtFullName.Location = New System.Drawing.Point(32, 188)
            Me.txtFullName.MaxLength = 50
            Me.txtFullName.Name = "txtFullName"
            Me.txtFullName.Size = New System.Drawing.Size(456, 23)
            Me.txtFullName.TabIndex = 5
            '
            'lblPassword
            '
            Me.lblPassword.AutoSize = True
            Me.lblPassword.Location = New System.Drawing.Point(32, 232)
            Me.lblPassword.Name = "lblPassword"
            Me.lblPassword.TabIndex = 6
            Me.lblPassword.Text = "Password"
            '
            'txtPassword
            '
            Me.txtPassword.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtPassword.Location = New System.Drawing.Point(32, 254)
            Me.txtPassword.MaxLength = 50
            Me.txtPassword.Name = "txtPassword"
            Me.txtPassword.Size = New System.Drawing.Size(456, 23)
            Me.txtPassword.TabIndex = 7
            Me.txtPassword.UseSystemPasswordChar = True
            '
            'lblConfirm
            '
            Me.lblConfirm.AutoSize = True
            Me.lblConfirm.Location = New System.Drawing.Point(32, 298)
            Me.lblConfirm.Name = "lblConfirm"
            Me.lblConfirm.TabIndex = 8
            Me.lblConfirm.Text = "Confirm password"
            '
            'txtConfirm
            '
            Me.txtConfirm.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtConfirm.Location = New System.Drawing.Point(32, 320)
            Me.txtConfirm.MaxLength = 50
            Me.txtConfirm.Name = "txtConfirm"
            Me.txtConfirm.Size = New System.Drawing.Size(456, 23)
            Me.txtConfirm.TabIndex = 9
            Me.txtConfirm.UseSystemPasswordChar = True
            '
            'chkShowPassword
            '
            Me.chkShowPassword.AutoSize = True
            Me.chkShowPassword.Location = New System.Drawing.Point(32, 358)
            Me.chkShowPassword.Name = "chkShowPassword"
            Me.chkShowPassword.TabIndex = 10
            Me.chkShowPassword.Text = "Show passwords"
            Me.chkShowPassword.UseVisualStyleBackColor = True
            '
            'btnRegister
            '
            Me.btnRegister.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnRegister.Kind = Global.oopfinals.Controls.ThemeButtonKind.Primary
            Me.btnRegister.Location = New System.Drawing.Point(32, 388)
            Me.btnRegister.Name = "btnRegister"
            Me.btnRegister.Size = New System.Drawing.Size(456, 46)
            Me.btnRegister.TabIndex = 11
            Me.btnRegister.Text = "Create account"
            Me.btnRegister.UseVisualStyleBackColor = False
            '
            'lblError
            '
            Me.lblError.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblError.Location = New System.Drawing.Point(32, 448)
            Me.lblError.Name = "lblError"
            Me.lblError.Size = New System.Drawing.Size(456, 40)
            Me.lblError.TabIndex = 12
            Me.lblError.Text = ""
            '
            'lnkBackToLogin
            '
            Me.lnkBackToLogin.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lnkBackToLogin.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline
            Me.lnkBackToLogin.Location = New System.Drawing.Point(32, 500)
            Me.lnkBackToLogin.Name = "lnkBackToLogin"
            Me.lnkBackToLogin.Size = New System.Drawing.Size(456, 22)
            Me.lnkBackToLogin.TabIndex = 13
            Me.lnkBackToLogin.TabStop = True
            Me.lnkBackToLogin.Text = "Already registered? Back to login"
            Me.lnkBackToLogin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'RegisterForm
            '
            Me.AcceptButton = Me.btnRegister
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(984, 661)
            Me.Controls.Add(Me.tlpRoot)
            Me.Name = "RegisterForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Mini-Grocery Store POS — Create account"
            Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
            Me.card.ResumeLayout(False)
            Me.card.PerformLayout()
            Me.tlpRoot.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents tlpRoot As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents card As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents lblSubtitle As System.Windows.Forms.Label
        Friend WithEvents lblUsername As System.Windows.Forms.Label
        Friend WithEvents txtUsername As System.Windows.Forms.TextBox
        Friend WithEvents lblFullName As System.Windows.Forms.Label
        Friend WithEvents txtFullName As System.Windows.Forms.TextBox
        Friend WithEvents lblPassword As System.Windows.Forms.Label
        Friend WithEvents txtPassword As System.Windows.Forms.TextBox
        Friend WithEvents lblConfirm As System.Windows.Forms.Label
        Friend WithEvents txtConfirm As System.Windows.Forms.TextBox
        Friend WithEvents chkShowPassword As System.Windows.Forms.CheckBox
        Friend WithEvents btnRegister As Global.oopfinals.Controls.StyledButton
        Friend WithEvents lblError As System.Windows.Forms.Label
        Friend WithEvents lnkBackToLogin As System.Windows.Forms.LinkLabel

    End Class

End Namespace
