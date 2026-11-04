Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Services/ValidationRules.vb — the SINGLE source of truth for input
' rules (spec §9.2, ~63 lines). Every consumer — Login (M8), Register
' (M8), Owner staff tooling (M14), Customer profile (M12), AuthService
' (M6) — asks THESE methods, so the rules can never drift apart.
'
' OOP concept: ENCAPSULATION (static utility):
'   NotInheritable + Private Sub New — instantiating this class is a
'   compile error; the visible surface is Shared members only.
'
' LAYERING: Services may be consumed by Forms and other Services; it
' references NOTHING outside System.* (no Models, no Data, no UI).
'
' MILESTONE NOTE (M6): the Boolean validators land now; the friendly
' user-facing messages that map them to screen text arrive with the
' forms in M8/M12/M14 — this file stays UI-free by design.
' =====================================================================

Imports System
Imports System.Text.RegularExpressions

Namespace Services

    Public NotInheritable Class ValidationRules

        ' Static utility: never instantiated (spec §4).
        Private Sub New()
        End Sub

#Region "Constants (spec §9.2)"

        Public Const MinUsernameLength As Integer = 3
        Public Const MaxUsernameLength As Integer = 20
        Public Const MinPasswordLength As Integer = 6
        Public Const MaxMoney As Decimal = 999999.99D
        Public Const MaxQuantity As Integer = 999

        ' Central regexes — defined here, referenced nowhere else.
        ' Username: letters/digits/dot/underscore only (spec §9.2).
        Private Const UsernamePattern As String = "^[A-Za-z0-9._]+$"

        ' Contact: digits with optional leading + or embedded - (spec §9.2).
        Private Const ContactPattern As String = "^[0-9+\-]{7,15}$"

#End Region

#Region "Validators"

        ' Username — ^[A-Za-z0-9._]+$ and 3–20 characters.
        Public Shared Function IsValidUsername(value As String) As Boolean
            If value Is Nothing Then
                Return False
            End If

            If value.Length < MinUsernameLength OrElse value.Length > MaxUsernameLength Then
                Return False
            End If

            Return Regex.IsMatch(value, UsernamePattern)
        End Function

        ' Full name — at least one letter; the only non-letters allowed
        ' are spaces, hyphen, apostrophe, dot and comma (spec §9.2).
        ' Trimmed emptiness is invalid, everything non-letter is rejected.
        Public Shared Function IsValidFullName(value As String) As Boolean
            If value Is Nothing OrElse value.Trim().Length = 0 Then
                Return False
            End If

            Dim hasLetter As Boolean = False
            For Each ch As Char In value
                If Char.IsLetter(ch) Then
                    hasLetter = True
                ElseIf Not (ch = " "c OrElse ch = "-"c OrElse ch = "'"c OrElse ch = "."c OrElse ch = ","c) Then
                    Return False
                End If
            Next

            ' At least one letter — "-'-." is punctuation, not a name.
            Return hasLetter
        End Function

        ' Password — at least MinPasswordLength characters and NO
        ' whitespace anywhere (leading, internal or trailing; spec §9.2).
        Public Shared Function IsValidPassword(value As String) As Boolean
            If value Is Nothing OrElse value.Length < MinPasswordLength Then
                Return False
            End If

            For Each ch As Char In value
                If Char.IsWhiteSpace(ch) Then
                    Return False
                End If
            Next

            Return True
        End Function

        ' Money — >= 0, <= MaxMoney, at most 2 decimal places (spec §9.2).
        ' Rounding the value must not change it — that is the 2-dp test —
        ' and it uses the project's canonical AwayFromZero mode (§13).
        Public Shared Function IsValidMoney(value As Decimal) As Boolean
            If value < 0D OrElse value > MaxMoney Then
                Return False
            End If

            Dim rounded As Decimal = Math.Round(value, 2, MidpointRounding.AwayFromZero)
            Return value = rounded
        End Function

        ' Contact number — ^[0-9+\-]{7,15}$ (digits, one optional +
        ' prefix, optional internal hyphens; spec §9.2).
        Public Shared Function IsValidContactNumber(value As String) As Boolean
            If value Is Nothing Then
                Return False
            End If

            Return Regex.IsMatch(value, ContactPattern)
        End Function

#End Region

    End Class

End Namespace
