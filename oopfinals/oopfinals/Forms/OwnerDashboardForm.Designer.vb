Option Explicit On
Option Strict On

Namespace Forms

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class OwnerDashboardForm
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
            Me.appHeader = New Global.oopfinals.Controls.AppHeader()
            Me.btnLogout = New Global.oopfinals.Controls.StyledButton()
            Me.tabRail = New Global.oopfinals.Controls.IconTabBar()
            Me.contentCard = New Global.oopfinals.Controls.RoundedPanel()
            Me.pageHeading = New Global.oopfinals.Controls.PageHeading()
            Me.permissionsGrid = New System.Windows.Forms.DataGridView()
            Me.colPermission = New System.Windows.Forms.DataGridViewTextBoxColumn()
            CType(Me.permissionsGrid, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.contentCard.SuspendLayout()
            Me.SuspendLayout()
            '
            'appHeader
            '
            Me.appHeader.Controls.Add(Me.btnLogout)
            Me.appHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.appHeader.Location = New System.Drawing.Point(0, 0)
            Me.appHeader.Name = "appHeader"
            Me.appHeader.Size = New System.Drawing.Size(984, 64)
            Me.appHeader.TabIndex = 2
            '
            'btnLogout (child of appHeader → always paints above it)
            '
            Me.btnLogout.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnLogout.Kind = Global.oopfinals.Controls.ThemeButtonKind.Danger
            Me.btnLogout.Location = New System.Drawing.Point(848, 14)
            Me.btnLogout.Name = "btnLogout"
            Me.btnLogout.Size = New System.Drawing.Size(120, 36)
            Me.btnLogout.TabIndex = 0
            Me.btnLogout.Text = "Logout"
            Me.btnLogout.UseVisualStyleBackColor = False
            '
            'tabRail
            '
            Me.tabRail.Dock = System.Windows.Forms.DockStyle.Left
            Me.tabRail.Location = New System.Drawing.Point(0, 64)
            Me.tabRail.Name = "tabRail"
            Me.tabRail.Size = New System.Drawing.Size(220, 597)
            Me.tabRail.TabIndex = 1
            '
            'contentCard
            '
            Me.contentCard.Controls.Add(Me.permissionsGrid)
            Me.contentCard.Controls.Add(Me.pageHeading)
            Me.contentCard.Dock = System.Windows.Forms.DockStyle.Fill
            Me.contentCard.Location = New System.Drawing.Point(220, 64)
            Me.contentCard.Name = "contentCard"
            Me.contentCard.Padding = New System.Windows.Forms.Padding(24)
            Me.contentCard.Size = New System.Drawing.Size(764, 597)
            Me.contentCard.TabIndex = 0
            '
            'pageHeading
            '
            Me.pageHeading.Dock = System.Windows.Forms.DockStyle.Top
            Me.pageHeading.Location = New System.Drawing.Point(24, 24)
            Me.pageHeading.Name = "pageHeading"
            Me.pageHeading.Size = New System.Drawing.Size(716, 56)
            Me.pageHeading.Subtitle = Nothing
            Me.pageHeading.TabIndex = 1
            Me.pageHeading.TabStop = False
            '
            'permissionsGrid
            '
            Me.permissionsGrid.AllowUserToAddRows = False
            Me.permissionsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
            Me.permissionsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            Me.permissionsGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colPermission})
            Me.permissionsGrid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.permissionsGrid.Location = New System.Drawing.Point(24, 80)
            Me.permissionsGrid.MultiSelect = False
            Me.permissionsGrid.Name = "permissionsGrid"
            Me.permissionsGrid.ReadOnly = True
            Me.permissionsGrid.RowHeadersVisible = False
            Me.permissionsGrid.Size = New System.Drawing.Size(716, 493)
            Me.permissionsGrid.TabIndex = 0
            '
            'colPermission
            '
            Me.colPermission.HeaderText = "Permission key"
            Me.colPermission.Name = "colPermission"
            Me.colPermission.ReadOnly = True
            '
            'OwnerDashboardForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(984, 661)
            Me.Controls.Add(Me.contentCard)
            Me.Controls.Add(Me.tabRail)
            Me.Controls.Add(Me.appHeader)
            Me.Name = "OwnerDashboardForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Store Owner Dashboard"
            Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
            CType(Me.permissionsGrid, System.ComponentModel.ISupportInitialize).EndInit()
            Me.contentCard.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents appHeader As Global.oopfinals.Controls.AppHeader
        Friend WithEvents btnLogout As Global.oopfinals.Controls.StyledButton
        Friend WithEvents tabRail As Global.oopfinals.Controls.IconTabBar
        Friend WithEvents contentCard As Global.oopfinals.Controls.RoundedPanel
        Friend WithEvents pageHeading As Global.oopfinals.Controls.PageHeading
        Friend WithEvents permissionsGrid As System.Windows.Forms.DataGridView
        Friend WithEvents colPermission As System.Windows.Forms.DataGridViewTextBoxColumn

    End Class

End Namespace
