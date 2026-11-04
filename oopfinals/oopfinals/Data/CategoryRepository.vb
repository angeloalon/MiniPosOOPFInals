Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Data/CategoryRepository.vb — master-data CRUD for the Categories
' table (spec §7.3). Every statement is parameterized; deletes are
' SAFE forms — a category still referenced by any product is refused
' with a friendly message instead of a raw FK constraint error.
'
' OOP concept: ENCAPSULATION (utility class): NotInheritable +
' Private Sub New, Shared surface only, exactly like the other
' repositories. Name validation flows through the Category model's
' own setters so the same rule fires at every layer.
'
' LAYERING: Data → Models only (Category). No Services, no Forms.
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports System.Data.SQLite
Imports oopfinals.Models

Namespace Data

    Public NotInheritable Class CategoryRepository

        ' Static gate: repositories are stateless, never instantiated.
        Private Sub New()
        End Sub

#Region "Connection + row mapping"

        Private Shared Function CreateConnection() As SQLiteConnection
            Return DatabaseHelper.GetConnection()
        End Function

        Private Shared Function MapRowToCategory(reader As SQLiteDataReader) As Category
            Return New Category(Convert.ToInt32(reader("CategoryId")), reader("Name").ToString())
        End Function

#End Region

#Region "Reads"

        Public Shared Function GetAll() As List(Of Category)
            Dim categories As New List(Of Category)()

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("SELECT CategoryId, Name FROM Categories ORDER BY Name;", connection)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        Do While reader.Read()
                            categories.Add(MapRowToCategory(reader))
                        Loop
                    End Using
                End Using
            End Using

            Return categories
        End Function

        Public Shared Function GetById(categoryId As Integer) As Category
            If categoryId <= 0 Then
                Return Nothing
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("SELECT CategoryId, Name FROM Categories WHERE CategoryId = @categoryId;", connection)
                    command.Parameters.AddWithValue("@categoryId", categoryId)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapRowToCategory(reader)
                        End If
                    End Using
                End Using
            End Using

            Return Nothing
        End Function

#End Region

#Region "Writes"

        ' Duplicate names would trip the UNIQUE(Name) constraint — the
        ' pre-check surfaces the friendly message instead (case differs
        ' don't count as duplicates, matching UNIQUE + NOCASE intent).
        Private Shared Function NameExists(name As String, excludeCategoryId As Integer) As Boolean
            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(
                        "SELECT COUNT(1) FROM Categories WHERE Name = @name COLLATE NOCASE AND CategoryId <> @categoryId;",
                        connection)
                    command.Parameters.AddWithValue("@name", name)
                    command.Parameters.AddWithValue("@categoryId", excludeCategoryId)
                    Return Convert.ToInt64(command.ExecuteScalar()) > 0
                End Using
            End Using
        End Function

        ' Returns the new Categories.CategoryId. The Category ctor's
        ' validating setter throws "Category name cannot be empty." for
        ' blank input — one rule, every layer.
        Public Shared Function Add(name As String) As Integer
            Dim category As New Category(0, name)

            If NameExists(category.Name, 0) Then
                Throw New InvalidOperationException(
                    "A category named '" & category.Name & "' already exists.")
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand("INSERT INTO Categories (Name) VALUES (@name);", connection)
                    command.Parameters.AddWithValue("@name", category.Name)
                    command.ExecuteNonQuery()
                End Using

                Using idCommand As New SQLiteCommand("SELECT last_insert_rowid();", connection)
                    Return Convert.ToInt32(idCommand.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Shared Function Update(categoryId As Integer, name As String) As Boolean
            If categoryId <= 0 Then
                Throw New ArgumentException("A valid Category must be selected.")
            End If

            Dim category As New Category(categoryId, name)

            If NameExists(category.Name, categoryId) Then
                Throw New InvalidOperationException(
                    "A category named '" & category.Name & "' already exists.")
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using command As New SQLiteCommand(
                        "UPDATE Categories SET Name = @name WHERE CategoryId = @categoryId;", connection)
                    command.Parameters.AddWithValue("@name", category.Name)
                    command.Parameters.AddWithValue("@categoryId", categoryId)
                    Return command.ExecuteNonQuery() > 0
                End Using
            End Using
        End Function

        ' Safe delete (spec §7.3): refuse while any product still points
        ' at this category — deleting would orphan the catalog rows.
        Public Shared Function Delete(categoryId As Integer) As Boolean
            If categoryId <= 0 Then
                Throw New ArgumentException("A valid Category must be selected.")
            End If

            Using connection As SQLiteConnection = CreateConnection()
                Using countCommand As New SQLiteCommand(
                        "SELECT COUNT(1) FROM Products WHERE CategoryId = @categoryId;", connection)
                    countCommand.Parameters.AddWithValue("@categoryId", categoryId)
                    Dim productCount As Long = Convert.ToInt64(countCommand.ExecuteScalar())
                    If productCount > 0 Then
                        Throw New InvalidOperationException(String.Format(
                            "This category is still used by {0} product(s) and cannot be deleted.", productCount))
                    End If
                End Using

                Using command As New SQLiteCommand("DELETE FROM Categories WHERE CategoryId = @categoryId;", connection)
                    command.Parameters.AddWithValue("@categoryId", categoryId)
                    Return command.ExecuteNonQuery() > 0
                End Using
            End Using
        End Function

#End Region

    End Class

End Namespace
