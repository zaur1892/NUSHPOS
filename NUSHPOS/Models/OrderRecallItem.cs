using System;

namespace NUSHPOS.Models
{
    public class OrderRecallItem
    {
        public int OrderID { get; set; }
        public int AutoID { get; set; }
        public string ReceiptNo { get; set; }
        public string OrderKey { get; set; }
        public DateTime? OrderDateTime { get; set; }
        public string EmployeeName { get; set; }

        public string DisplayCheckNo
        {
            get
            {
                if (!string.IsNullOrEmpty(ReceiptNo)) return $"#{OrderID} ({ReceiptNo})";
                return $"#{OrderID}";
            }
        }
        public decimal GrandTotal { get; set; }
        public int OrderStatus { get; set; }
        public string OrderStatusName { get; set; }
        public int OrderType { get; set; }
        public string OrderTypeName { get; set; }
        public string PaymentMethodName { get; set; }
        public string InfoText
        {
            get
            {
                var text = OrderStatusName;
                if (!string.IsNullOrEmpty(PaymentMethodName))
                {
                    text += $" ({PaymentMethodName})";
                }
                return text;
            }
        }
    }
}
