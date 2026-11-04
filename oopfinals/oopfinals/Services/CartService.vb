Option Explicit On
Option Strict On

' =====================================================================
' oopfinals — Mini-Grocery Store POS & Daily Inventory System
' ITP 102 — Object-Oriented Programming · University of Caloocan City
'
' Services/CartService.vb — the ONE in-memory cart + its money math
' (spec §9.3). The intentionally stateful service of the Services
' layer: unlike AuthService/ValidationRules it holds per-session
' working state, so it is instantiable — but still single-responsibility:
' it knows the cart and NOTHING about SQL or screens.
'
' OOP concept: ENCAPSULATION (read-only cart exposure — spec §5)
'   The line list is private; callers see IReadOnlyList(Of SaleItem)
'   (_items.AsReadOnly()), so a form cannot mutate the cart directly.
'   Every change goes through AddItem / UpdateItemQuantity /
'   RemoveItem — the only paths that enforce the stock ceilings and
'   the 999-per-line context cap (spec §14).
'
' OOP concept: POLYMORPHISM #2 (Strategy swap — spec §5.2)
'   GetDiscountAmount() is literally CurrentDiscount.Apply(subtotal):
'   ONE base-type slot (Discount) with interchangeable strategy
'   subclasses (NoDiscount / RegularDiscount / SeniorPwdDiscount).
'   The POS combo installs a different subclass into the same slot and
'   the SAME call computes a different number — the strategy swap is
'   visible on the totals with zero If/Else on the discount kind.
'
' LAYERING: Services → Models only (Product / SaleItem / Discount).
' No Data, no Forms (spec §3 one-way dependency flow).
' =====================================================================

Imports System
Imports System.Collections.Generic
Imports oopfinals.Models

Namespace Services

    Public NotInheritable Class CartService

        ' Spec §14: cart items carry a 999-per-line context cap.
        Public Const MaxLineQuantity As Integer = 999

        Private ReadOnly _items As List(Of SaleItem)
        Private _currentDiscount As Discount

        ' A cart is born with the zero-discount strategy installed.
        Public Sub New()
            _items = New List(Of SaleItem)()
            _currentDiscount = New NoDiscount()
        End Sub

#Region "Read-only exposure (encapsulation)"

        ' Live read-only view of the cart lines — callers can enumerate,
        ' never mutate. Spec §5: "cart list is private; exposed only as
        ' IReadOnlyList(Of SaleItem)".
        Public ReadOnly Property Items As IReadOnlyList(Of SaleItem)
            Get
                Return _items.AsReadOnly()
            End Get
        End Property

        ' The strategy slot. The POS installs whatever Discount subclass
        ' its combo holds; ClearCart() always resets to NoDiscount.
        Public Property CurrentDiscount As Discount
            Get
                Return _currentDiscount
            End Get
            Set(value As Discount)
                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value), "Discount strategy cannot be Nothing.")
                End If
                _currentDiscount = value
            End Set
        End Property

        ' The installed strategy's spec display name ("None (0%)", …).
        Public Function GetDiscountName() As String
            Return _currentDiscount.DiscountName
        End Function

        ' Badge-friendly unit count (Σ Quantity across all lines).
        Public Function GetTotalItemCount() As Integer
            Dim total As Integer = 0
            For Each line As SaleItem In _items
                total += line.Quantity
            Next
            Return total
        End Function

#End Region

#Region "Cart mutations (the ONLY write paths)"

        ' Merge-or-append with a COMBINED stock check (spec §9.3): the
        ' ceiling applies to existing.Quantity + requestedQty together,
        ' so nudging an existing line can never sneak past StockQty.
        ' Throws with the available stock in the message.
        Public Sub AddItem(product As Product, requestedQty As Integer)
            If product Is Nothing Then
                Throw New ArgumentNullException(NameOf(product), "A valid Product is required to add to the cart.")
            End If
            If requestedQty <= 0 Then
                Throw New ArgumentException("Cart quantity must be greater than zero.")
            End If
            If requestedQty > MaxLineQuantity Then
                Throw New ArgumentException(
                    String.Format("Quantity cannot exceed {0} per cart line.", MaxLineQuantity))
            End If

            Dim existing As SaleItem = FindLine(product.ProductId)
            Dim combined As Integer = If(existing Is Nothing, 0, existing.Quantity) + requestedQty

            ' The 999-per-line ceiling applies to the MERGED total too —
            ' a merge can never be used to bypass the cap.
            If combined > MaxLineQuantity Then
                Throw New ArgumentException(
                    String.Format("Quantity cannot exceed {0} per cart line.", MaxLineQuantity))
            End If

            If combined > product.StockQty Then
                If existing Is Nothing Then
                    Throw New InvalidOperationException(String.Format(
                        "Cannot add {0} x {1} ({2}). Only {3} unit(s) available in stock.",
                        requestedQty, product.Name, product.ProductCode, product.StockQty))
                Else
                    Throw New InvalidOperationException(String.Format(
                        "The cart already holds {0} x {1} ({2}); adding {3} more exceeds the {4} unit(s) available in stock.",
                        existing.Quantity, product.Name, product.ProductCode, requestedQty, product.StockQty))
                End If
            End If

            If existing Is Nothing Then
                _items.Add(New SaleItem(product.ProductId, product.ProductCode, product.Name, requestedQty, product.Price))
            Else
                existing.Quantity = combined   ' validated setter keeps it ≥ 1
            End If
        End Sub

        ' Re-price an existing line. 0/negative ⇒ remove (spec §9.3).
        ' The caller supplies the product's CURRENT stock so the ceiling
        ' is evaluated against fresh data at call time.
        Public Sub UpdateItemQuantity(productId As Integer, newQty As Integer, availableStock As Integer)
            If newQty <= 0 Then
                RemoveItem(productId)
                Return
            End If
            If newQty > MaxLineQuantity Then
                Throw New ArgumentException(
                    String.Format("Quantity cannot exceed {0} per cart line.", MaxLineQuantity))
            End If
            If newQty > availableStock Then
                Throw New InvalidOperationException(String.Format(
                    "Only {0} unit(s) available in stock.", availableStock))
            End If

            Dim existing As SaleItem = FindLine(productId)
            If existing Is Nothing Then
                Return   ' nothing to update — quiet no-op
            End If
            existing.Quantity = newQty
        End Sub

        ' Remove a line; True when a line actually went away.
        Public Function RemoveItem(productId As Integer) As Boolean
            Dim existing As SaleItem = FindLine(productId)
            If existing Is Nothing Then
                Return False
            End If
            Return _items.Remove(existing)
        End Function

        ' Empty the cart AND reset the strategy — a fresh cart always
        ' starts with "None (0%)" (spec §9.3).
        Public Sub ClearCart()
            _items.Clear()
            _currentDiscount = New NoDiscount()
        End Sub

#End Region

#Region "Money math (Decimal + AwayFromZero, spec §13)"

        ' Σ LineTotal, rounded at the boundary.
        Public Function GetSubtotal() As Decimal
            Dim sum As Decimal = 0D
            For Each line As SaleItem In _items
                sum += line.LineTotal
            Next
            Return Math.Round(sum, 2, MidpointRounding.AwayFromZero)
        End Function

        ' THE polymorphic call (spec §5.2): whichever Discount subclass
        ' currently occupies the slot answers with its own Apply() —
        ' NoDiscount returns 0, the percentage strategies compute and
        ' round inside the model. The service never branches on kind.
        Public Function GetDiscountAmount() As Decimal
            Return Math.Round(_currentDiscount.Apply(GetSubtotal()), 2, MidpointRounding.AwayFromZero)
        End Function

        ' Net payable, floored at zero and rounded.
        Public Function GetNetTotal() As Decimal
            Dim net As Decimal = Math.Round(GetSubtotal() - GetDiscountAmount(), 2, MidpointRounding.AwayFromZero)
            If net < 0D Then
                net = 0D
            End If
            Return net
        End Function

        ' Change for the cash box. Guards first (spec §9.3): an empty
        ' cart cannot be paid for, and short cash reports the total due.
        Public Function ComputeChange(cashReceived As Decimal) As Decimal
            If GetTotalItemCount() = 0 Then
                Throw New InvalidOperationException("The cart is empty. Cannot compute change.")
            End If

            Dim totalDue As Decimal = GetNetTotal()
            If cashReceived < totalDue Then
                Throw New InvalidOperationException(
                    String.Format("Insufficient cash. Total due is PHP {0:N2}.", totalDue))
            End If

            Return Math.Round(cashReceived - totalDue, 2, MidpointRounding.AwayFromZero)
        End Function

#End Region

#Region "Private helpers"

        Private Function FindLine(productId As Integer) As SaleItem
            For Each line As SaleItem In _items
                If line.ProductId = productId Then
                    Return line
                End If
            Next
            Return Nothing
        End Function

#End Region

    End Class

End Namespace
