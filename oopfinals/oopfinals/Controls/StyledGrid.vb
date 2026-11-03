Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Controls/StyledGrid.vb — The ONE uniform DataGridView skin shared by
' every dashboard: styled headers, alternating rows, borderless cells,
' green selection — no ugly default blue anywhere.
'
' OOP concept: ENCAPSULATION as a utility class (NotInheritable +
' Private Sub New, Shared only) — skinning is applied, never re-implemented.
' =====================================================================

Imports System.Drawing
Imports System.Windows.Forms

Namespace Controls

    Public NotInheritable Class StyledGrid

        Private Sub New()
        End Sub

        Public Shared Sub Apply(grid As DataGridView)
            If grid Is Nothing Then
                Throw New ArgumentNullException("grid", "StyledGrid.Apply requires a grid.")
            End If

            grid.BackgroundColor = Theme.CardBg
            grid.BorderStyle = BorderStyle.None
            grid.GridColor = Theme.BorderLine
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            grid.EnableHeadersVisualStyles = False

            ' Column headers
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 248)
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Theme.Ink
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(247, 249, 248)
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Theme.Ink
            grid.ColumnHeadersDefaultCellStyle.Font = Theme.FontFor(ThemeWeight.Semibold, 9.0F)
            grid.ColumnHeadersDefaultCellStyle.Padding = New Padding(6, 6, 6, 6)
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            grid.ColumnHeadersHeight = 38
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single

            ' Row template
            grid.RowTemplate.Height = 32
            grid.DefaultCellStyle.BackColor = Color.White
            grid.DefaultCellStyle.ForeColor = Theme.Ink
            grid.DefaultCellStyle.SelectionBackColor = Theme.GreenTint
            grid.DefaultCellStyle.SelectionForeColor = Theme.Ink
            grid.DefaultCellStyle.Font = Theme.BodyFont()
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 246)
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Theme.GreenTint
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Theme.Ink

            ' Interaction defaults for read-only views
            grid.RowHeadersVisible = False
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            grid.MultiSelect = False
            grid.AllowUserToAddRows = False
            grid.AllowUserToDeleteRows = False
            grid.AllowUserToResizeRows = False
        End Sub

    End Class

End Namespace
