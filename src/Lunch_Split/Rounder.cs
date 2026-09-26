using System.Linq;
using Lunch_Split.Model;

namespace Lunch_Split.Engine;

public class Rounder
{
    public List<Share> RoundShares(List<Share> rawShares, RoundingMode mode)
    {
        List<Share> rounded = rawShares
            .Select(share => new Share(share.Name, RoundAmount(share.Amount, mode)))
            .ToList();
        if (mode == RoundingMode.None || rawShares.Count == 0)
        {
            return rounded;
        }

        decimal rawTotal = rawShares.Sum(share => share.Amount);
        decimal targetTotal = Math.Round(rawTotal, 2 , MidpointRounding.AwayFromZero);

        return ReconcilePennies(rounded, rawShares, targetTotal);
    }

    private static decimal RoundAmount(decimal amount, RoundingMode mode) => mode switch
    {
        RoundingMode.None => amount,
        RoundingMode.Bankers => Math.Round(amount,MidpointRounding.ToEven),
        RoundingMode.Up => Math.Ceiling(amount * 100m) /100m,
        RoundingMode.Down => Math.Floor (amount * 100m) /100m,
        _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unrecognized rounding mode."),
    };


    private static List<Share> ReconcilePennies(List<Share> roundedShares, List<Share> rawShares, decimal targetTotal)
    {
        decimal roundedTotal = roundedShares.Sum(share => share.Amount);
        int pennyDifference = (int)Math.Round((targetTotal - roundedTotal) * 100m, MidpointRounding.AwayFromZero);
 
        if (pennyDifference == 0)
        {
            return roundedShares;
        }
 
        List<int> priorityOrder = Enumerable.Range(0, rawShares.Count)
            .OrderByDescending(i => rawShares[i].Amount - Math.Floor(rawShares[i].Amount * 100m) / 100m)
            .ToList();
 
        List<Share> adjusted = new List<Share>(roundedShares);
        int step = pennyDifference > 0 ? 1 : -1;
        int remainingPennies = Math.Abs(pennyDifference);
 
        for (int i = 0; i < remainingPennies; i++)
        {
            int shareIndex = priorityOrder[i % priorityOrder.Count];
            Share current = adjusted[shareIndex];
            adjusted[shareIndex] = new Share(current.Name, current.Amount + (step * 0.01m));
        }
 
        return adjusted;
    }
}