using CommunityToolkit.Mvvm.ComponentModel;

namespace NUSHPOS.Models;

public partial class SessionPaymentSummary : ObservableObject
{
    public int AutoID { get; set; }
    public int PaymentMethodID { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public decimal AmountPaid { get; set; } // System calculated (Sum - Expenses)
    public int PaymentTypeID { get; set; } // 1=Cash, 2=Card, 3=Meal Voucher, 4=Account, 5=Bonus, etc.
    public string PaymentTypeName { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Difference))]
    private decimal _countedAmount = 0m;

    public decimal Difference => CountedAmount - AmountPaid;
}
