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

    
    
}
