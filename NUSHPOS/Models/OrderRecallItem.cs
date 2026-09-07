using System;
using System.Collections.Generic;

namespace NUSHPOS.Models
{
    public class OrderRecallItem
    {
        public int OrderID { get; set; }
        public int AutoID { get; set; }
        public string? ReceiptNo { get; set; }
        public string? OrderKey { get; set; }
        public DateTime? OrderDateTime { get; set; }
        public string? EmployeeName { get; set; }
        public string? DineInTableName { get; set; }
        public string? CustomerName { get; set; }
        public string? OrderPhone { get; set; }
        public string? BarTabName { get; set; }

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
        public string? OrderStatusName { get; set; }
        public int OrderType { get; set; }
        public string? OrderTypeName { get; set; }
        public string? PaymentMethodName { get; set; }

        public string SaleTypeDisplay
        {
            get
            {
                return OrderType switch
                {
                    1 => !string.IsNullOrWhiteSpace(DineInTableName) ? $"MASA ({DineInTableName})" : "MASA SATIŞI",
                    2 => "BAR SATIŞI",
                    3 => !string.IsNullOrWhiteSpace(CustomerName) ? $"AL GÖTÜR ({CustomerName})" : "AL GÖTÜR",
                    4 => "TƏZGAH / SÜR-KEÇ",
                    5 => !string.IsNullOrWhiteSpace(CustomerName) ? $"PAKET: {CustomerName}" : "PAKET SATIŞI",
                    66 => "İADƏ",
                    _ => !string.IsNullOrWhiteSpace(OrderTypeName) ? OrderTypeName : "SATIŞ"
                };
            }
        }

        public string InfoText
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(OrderStatusName)) parts.Add(OrderStatusName);
                if (!string.IsNullOrWhiteSpace(BarTabName)) parts.Add(BarTabName);
                if (!string.IsNullOrWhiteSpace(PaymentMethodName)) parts.Add(PaymentMethodName);
                if (!string.IsNullOrWhiteSpace(OrderPhone)) parts.Add($"Tel: {OrderPhone}");
                return parts.Count > 0 ? string.Join(" | ", parts) : "-";
            }
        }
    }
}
