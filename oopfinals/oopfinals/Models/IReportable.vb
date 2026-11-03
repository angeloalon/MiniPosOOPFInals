Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/IReportable.vb — INTERFACE contract decoupling report
' generation from UI presentation. Any report generator that satisfies
' this contract can be bound into the Owner's Reports tab, exported to
' CSV, and printed — without the UI knowing the concrete class.
' =====================================================================

Namespace Models

    Public Interface IReportable

        ReadOnly Property ReportTitle As String

        ReadOnly Property GeneratedDate As DateTime

        Function GenerateReportData() As DataTable

        Function ExportToCsv(filePath As String) As Boolean

    End Interface

End Namespace
