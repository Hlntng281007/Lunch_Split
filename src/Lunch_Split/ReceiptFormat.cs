using System.Linq;
using System.Text;
using Lunch_Split.Model;

namespace Lunch_Split.Output;

public class ReceiptFormat
{
    private const string StudentName = "Kim";
    
    private readonly Func<DateTime> _clock;

    public ReceiptFormat() : this (() => DateTime.Now)
    {
        
    } 

    public ReceiptFormat(Func<DateTime> clock)
    {
        _clock = clock;
    }
     
    public string Format(Bill bill, List<Attendee> attendees, List<Share>shares)
    {
        DateTime now = _clock();
        StringBuilder receipt = new StringBuilder();

        receipt.AppendLine($"Student: {StudentName}");
        receipt.AppendLine($"Date: {now: yyyy-MM-dd}");
        receipt.AppendLine($"Generated: {now: yyyy-MM-dd HH:mm:ss}");

        receipt.AppendLine("----------------------------------------");
        receipt.AppendLine($"Subtotal: {bill.Subtotal:C}");
        receipt.AppendLine($"Tax: {bill.Tax:C}");
        receipt.AppendLine("----------------------------------------");

        foreach (Share share in shares)
        {
            receipt.AppendLine($"{share.Name}: {share.Amount:C}");
        }

        decimal grandTotal = shares.Sum(share => share.Amount);
        receipt.AppendLine("----------------------------------------");
        receipt.AppendLine($"Total: {grandTotal:C}");

        return receipt.ToString();

    }
    
    
}
