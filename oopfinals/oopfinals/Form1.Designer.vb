Option Explicit On
Option Strict On

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Me.btnOwner = New Global.oopfinals.Controls.StyledButton()
        Me.btnCashier = New Global.oopfinals.Controls.StyledButton()
        Me.btnClerk = New Global.oopfinals.Controls.StyledButton()
        Me.btnCustomer = New Global.oopfinals.Controls.StyledButton()
        Me.btnCryptoTest = New Global.oopfinals.Controls.StyledButton()
        Me.btnInitDb = New Global.oopfinals.Controls.StyledButton()
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
        Me.tlpRoot.Size = New System.Drawing.Size(800, 450)
        Me.tlpRoot.TabIndex = 0
        '
        'card (Anchor=None → centered by tlpRoot at every window size)
        '
        Me.card.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.card.Controls.Add(Me.lblTitle)
        Me.card.Controls.Add(Me.lblSubtitle)
        Me.card.Controls.Add(Me.btnOwner)
        Me.card.Controls.Add(Me.btnCashier)
        Me.card.Controls.Add(Me.btnClerk)
        Me.card.Controls.Add(Me.btnCustomer)
        Me.card.Controls.Add(Me.btnCryptoTest)
        Me.card.Controls.Add(Me.btnInitDb)
        Me.card.Location = New System.Drawing.Point(170, 45)
        Me.card.Name = "card"
        Me.card.Size = New System.Drawing.Size(460, 482)
        Me.card.TabIndex = 0
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(28, 28)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(230, 26)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Mini-Grocery Store POS"
        '
        'lblSubtitle
        '
        Me.lblSubtitle.AutoSize = True
        Me.lblSubtitle.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSubtitle.Location = New System.Drawing.Point(28, 66)
        Me.lblSubtitle.Name = "lblSubtitle"
        Me.lblSubtitle.Size = New System.Drawing.Size(390, 15)
        Me.lblSubtitle.TabIndex = 1
        Me.lblSubtitle.Text = "M7 dev launcher — open a role, init the demo database, or run the crypto self-test"
        '
        'btnOwner
        '
        Me.btnOwner.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOwner.Location = New System.Drawing.Point(28, 118)
        Me.btnOwner.Name = "btnOwner"
        Me.btnOwner.Size = New System.Drawing.Size(404, 46)
        Me.btnOwner.TabIndex = 2
        Me.btnOwner.Text = "Maria Santos — Store Owner"
        Me.btnOwner.UseVisualStyleBackColor = False
        '
        'btnCashier
        '
        Me.btnCashier.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCashier.Location = New System.Drawing.Point(28, 174)
        Me.btnCashier.Name = "btnCashier"
        Me.btnCashier.Size = New System.Drawing.Size(404, 46)
        Me.btnCashier.TabIndex = 3
        Me.btnCashier.Text = "Juan Dela Cruz — Cashier"
        Me.btnCashier.UseVisualStyleBackColor = False
        '
        'btnClerk
        '
        Me.btnClerk.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClerk.Location = New System.Drawing.Point(28, 230)
        Me.btnClerk.Name = "btnClerk"
        Me.btnClerk.Size = New System.Drawing.Size(404, 46)
        Me.btnClerk.TabIndex = 4
        Me.btnClerk.Text = "Pedro Penduko — Inventory Clerk"
        Me.btnClerk.UseVisualStyleBackColor = False
        '
        'btnCustomer
        '
        Me.btnCustomer.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCustomer.Location = New System.Drawing.Point(28, 286)
        Me.btnCustomer.Name = "btnCustomer"
        Me.btnCustomer.Size = New System.Drawing.Size(404, 46)
        Me.btnCustomer.TabIndex = 5
        Me.btnCustomer.Text = "Ana Batumbakal — Customer"
        Me.btnCustomer.UseVisualStyleBackColor = False
        '
        'btnCryptoTest
        '
        Me.btnCryptoTest.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCryptoTest.Location = New System.Drawing.Point(28, 342)
        Me.btnCryptoTest.Name = "btnCryptoTest"
        Me.btnCryptoTest.Size = New System.Drawing.Size(404, 46)
        Me.btnCryptoTest.TabIndex = 6
        Me.btnCryptoTest.Text = "Crypto self-test — M6 PBKDF2 hash & verify"
        Me.btnCryptoTest.UseVisualStyleBackColor = False
        '
        'btnInitDb
        '
        Me.btnInitDb.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnInitDb.Location = New System.Drawing.Point(28, 398)
        Me.btnInitDb.Name = "btnInitDb"
        Me.btnInitDb.Size = New System.Drawing.Size(404, 46)
        Me.btnInitDb.TabIndex = 7
        Me.btnInitDb.Text = "Init DB & demo login — M7 schema + seed test"
        Me.btnInitDb.UseVisualStyleBackColor = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.tlpRoot)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Mini-Grocery Store POS — Dev Launcher (M4–M7)"
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
    Friend WithEvents btnOwner As Global.oopfinals.Controls.StyledButton
    Friend WithEvents btnCashier As Global.oopfinals.Controls.StyledButton
    Friend WithEvents btnClerk As Global.oopfinals.Controls.StyledButton
    Friend WithEvents btnCustomer As Global.oopfinals.Controls.StyledButton
    Friend WithEvents btnCryptoTest As Global.oopfinals.Controls.StyledButton
    Friend WithEvents btnInitDb As Global.oopfinals.Controls.StyledButton

End Class
