namespace Lunch_Split.Model;

public enum TipMode
{
    none,
    percent,
    fixed_tip
}

public record Bill(decimal Subtotal, decimal Tax, TipMode TipMode, decimal tipInput);