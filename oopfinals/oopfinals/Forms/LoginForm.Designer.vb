Option Explicit On
Option Strict On

Namespace Forms

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class LoginForm
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
            Me.lblPassword = New System.Windows.Forms.Label()
            Me.txtPassword = New System.Windows.Forms.TextBox()
            Me.chkShowPassword = New System.Windows.Forms.CheckBox()
            Me.btnLogin = New Global.oopfinals.Controls.StyledButton()
            Me.lblStatus = New System.Windows.Forms.Label()
            Me.lnkRegister = New System.Windows.Forms.LinkLabel()
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
            Me.card.Controls.Add(Me.lblPassword)
            Me.card.Controls.Add(Me.txtPassword)
            Me.card.Controls.Add(Me.chkShowPassword)
            Me.card.Controls.Add(Me.btnLogin)
            Me.card.Controls.Add(Me.lblStatus)
            Me.card.Controls.Add(Me.lnkRegister)
            Me.card.Location = New System.Drawing.Point(232, 103)
            Me.card.Name = "card"
            Me.card.Size = New System.Drawing.Size(520, 456)
            Me.card.TabIndex = 0
            '
            'lblTitle
            '
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Location = New System.Drawing.Point(32, 28)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "Mini-Grocery Store POS"
            '
            'lblSubtitle
            '
            Me.lblSubtitle.AutoSize = True
            Me.lblSubtitle.Location = New System.Drawing.Point(32, 64)
            Me.lblSubtitle.Name = "lblSubtitle"
            Me.lblSubtitle.TabIndex = 1
            Me.lblSubtitle.Text = "Sign in to continue (demo accounts: owner, cashier, clerk, customer)"
            '
            'lblUsername
            '
            Me.lblUsername.AutoSize = True
            Me.lblUsername.Location = New System.Drawing.Point(32, 108)
            Me.lblUsername.Name = "lblUsername"
            Me.lblUsername.TabIndex = 2
            Me.lblUsername.Text = "Username"
            '
            'txtUsername
            '
            Me.txtUsername.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtUsername.Location = New System.Drawing.Point(32, 130)
            Me.txtUsername.MaxLength = 20
            Me.txtUsername.Name = "txtUsername"
            Me.txtUsername.Size = New System.Drawing.Size(456, 23)
            Me.txtUsername.TabIndex = 3
            '
            'lblPassword
            '
            Me.lblPassword.AutoSize = True
            Me.lblPassword.Location = New System.Drawing.Point(32, 174)
            Me.lblPassword.Name = "lblPassword"
            Me.lblPassword.TabIndex = 4
            Me.lblPassword.Text = "Password"
            '
            'txtPassword
            '
            Me.txtPassword.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtPassword.Location = New System.Drawing.Point(32, 196)
            Me.txtPassword.Name = "txtPassword"
            Me.txtPassword.Size = New System.Drawing.Size(456, 23)
            Me.txtPassword.TabIndex = 5
            Me.txtPassword.UseSystemPasswordChar = True
            '
            'chkShowPassword
            '
            Me.chkShowPassword.AutoSize = True
            Me.chkShowPassword.Location = New System.Drawing.Point(32, 236)
            Me.chkShowPassword.Name = "chkShowPassword"
            Me.chkShowPassword.TabIndex = 6
            Me.chkShowPassword.Text = "Show password"
            Me.chkShowPassword.UseVisualStyleBackColor = True
            '
            'btnLogin
            '
            Me.btnLogin.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnLogin.Kind = Global.oopfinals.Controls.ThemeButtonKind.Primary
            Me.btnLogin.Location = New System.Drawing.Point(32, 270)
            Me.btnLogin.Name = "btnLogin"
            Me.btnLogin.Size = New System.Drawing.Size(456, 46)
            Me.btnLogin.TabIndex = 7
            Me.btnLogin.Text = "Login"
            Me.btnLogin.UseVisualStyleBackColor = False
            '
            'lblStatus
            '
            Me.lblStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblStatus.Location = New System.Drawing.Point(32, 330)
            Me.lblStatus.Name = "lblStatus"
            Me.lblStatus.Size = New System.Drawing.Size(456, 48)
            Me.lblStatus.TabIndex = 8
            Me.lblStatus.Text = ""
            '
            'lnkRegister
            '
            Me.lnkRegister.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lnkRegister.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline
            Me.lnkRegister.Location = New System.Drawing.Point(32, 392)
            Me.lnkRegister.Name = "lnkRegister"
            Me.lnkRegister.Size = New System.Drawing.Size(456, 22)
            Me.lnkRegister.TabIndex = 9
            Me.lnkRegister.TabStop = True
            Me.lnkRegister.Text = "New customer? Create an account"
            Me.lnkRegister.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'LoginForm
            '
            Me.AcceptButton = Me.btnLogin
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(984, 661)
            Me.Controls.Add(Me.tlpRoot)
            Me.Name = "LoginForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Mini-Grocery Store POS — Login"
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
        Friend WithEvents lblPassword As System.Windows.Forms.Label
        Friend WithEvents txtPassword As System.Windows.Forms.TextBox
        Friend WithEvents chkShowPassword As System.Windows.Forms.CheckBox
        Friend WithEvents btnLogin As Global.oopfinals.Controls.StyledButton
        Friend WithEvents lblStatus As System.Windows.Forms.Label
        Friend WithEvents lnkRegister As System.Windows.Forms.LinkLabel

    End Class

End Namespace
