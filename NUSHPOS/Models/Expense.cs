using System;

namespace NUSHPOS.Models;

public class Expense
{
    public int AutoID { get; set; }
    public int? ExpenseID { get; set; }
    public string? ExpenseKey { get; set; }
    public DateTime? ExpenseDatetime { get; set; }
    public int? RegisterSessionID { get; set; }
    public int? PaymentMethodID { get; set; }
    public string? ExpenseName { get; set; }
    public decimal? ExpenseAmount { get; set; }
    public string? ExpenseDescription { get; set; }
    public int? StationID { get; set; }
    public int? LineDeleted { get; set; }
    public string? StationName { get; set; }
    public string? PaymentMethodName { get; set; }
    public string? AddEmployeeName { get; set; }
}
