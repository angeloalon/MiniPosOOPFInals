Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Models/Discount.vb — The polymorphic discount strategy hierarchy
' (all four classes in one file, per the spec's file inventory).
'
' OOP concept: ABSTRACTION — MustInherit Discount is a pure strategy
' contract: Apply(subtotal) minus how.  Fingerprint projection: the UI
' programs against the base type and swaps strategies at runtime.
'
' OOP concept: INHERITANCE — subclasses share the protected
' _discountName / _percentage state and a validating protected ctor
' (percentage clamped 0-100, ArgumentOutOfRangeException otherwise).
'
' The statutorily fixed rate of SeniorPwdDiscount mirrors the Philippine
' senior/PWD grocery discount (RA 9994 / RA 10754 style).
' =====================================================================

Namespace Models

    Public MustInherit Class Discount

        Protected ReadOnly _discountName As String
        Protected ReadOnly _percentage As Decimal

        Protected Sub New(discountName As String, percentage As Decimal)
            If String.IsNullOrWhiteSpace(discountName) Then
                Throw New ArgumentException("Discount name cannot be empty.")
            End If
            If percentage < 0D OrElse percentage > 100D Then
                Throw New ArgumentOutOfRangeException(
                    "percentage", percentage, "Discount percentage must be between 0 and 100.")
            End If
            _discountName = discountName.Trim()
            _percentage = percentage
        End Sub

        Public ReadOnly Property DiscountName As String
            Get
                Return _discountName
            End Get
        End Property

        Public ReadOnly Property Percentage As Decimal
            Get
                Return _percentage
            End Get
        End Property

        Public MustOverride Function Apply(subtotal As Decimal) As Decimal

        ' Shared money math so every strategy rounds identically.
        Protected Function PercentageOf(subtotal As Decimal) As Decimal
            If subtotal <= 0D Then
                Return 0D
            End If
            Return Math.Round(subtotal * (_percentage / 100D), 2, MidpointRounding.AwayFromZero)
        End Function

        Public Overrides Function ToString() As String
            Return _discountName
        End Function

    End Class

    ' Zero-discount strategy — the cart's default.
    Public NotInheritable Class NoDiscount
        Inherits Discount

        Public Sub New()
            MyBase.New("None (0%)", 0D)
        End Sub

        Public Overrides Function Apply(subtotal As Decimal) As Decimal
            Return 0D
        End Function

    End Class

    ' Configurable promo rate; the default builds the "Regular / Promo (5%)" label.
    Public NotInheritable Class RegularDiscount
        Inherits Discount

        Public Sub New(Optional rate As Decimal = 5D)
            MyBase.New(String.Format("Regular / Promo ({0:N0}%)", rate), rate)
        End Sub

        Public Overrides Function Apply(subtotal As Decimal) As Decimal
            Return PercentageOf(subtotal)
        End Function

    End Class

    ' Statutory senior citizen / PWD discount — hard 20%.
    Public NotInheritable Class SeniorPwdDiscount
        Inherits Discount

        Public Sub New()
            MyBase.New("Senior Citizen / PWD (20%)", 20D)
        End Sub

        Public Overrides Function Apply(subtotal As Decimal) As Decimal
            Return PercentageOf(subtotal)
        End Function

    End Class

End Namespace
