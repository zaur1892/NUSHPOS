using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("StationPrinterSettings")]
public class StationPrinterSettings
{
    [Key]
    public int AutoID { get; set; }
    public int StationID { get; set; } = 1;
    public string? CheckPrinterName { get; set; }
    public string? CheckAltPrinterName { get; set; }
    public int? CheckPrinterID { get; set; }
    public string? CheckDesignPath { get; set; }
    public string? DeliveryPrinterName { get; set; }
    public string? DeliveryAltPrinterName { get; set; }
    public int? DeliveryPrinterID { get; set; }
    public string? DeliveryDesignPath { get; set; }
    public string? InvoicePrinterName { get; set; }
    public string? InvoiceAltPrinterName { get; set; }
    public int? InvoicePrinterID { get; set; }
    public string? InvoiceDesignPath { get; set; }
    public int? InvoiceRowCount { get; set; }
    public string? ReportPrinterName { get; set; }
    public string? ReportAltPrinterName { get; set; }
    public int? ReportPrinterID { get; set; }
    public string? ReportDesignPath { get; set; }
    public string? AdditionPrinterName { get; set; }
    public int? AdditionPrinterID { get; set; }
    public string? AdditionDesignPath { get; set; }
    public int? AdditionRowCount { get; set; }
    public string? ReportA4PrinterName { get; set; }
    public string? ReportA4AltPrinterName { get; set; }
    public int? ReportA4PrinterID { get; set; }
    public string? ReportA4DesignPath { get; set; }

    public string? Kitchen1PrinterName { get; set; }
    public string? Kitchen1AltPrinterName { get; set; }
    public int? Kitchen1PrinterID { get; set; }
    public string? Kitchen1DesignPath { get; set; }

    public string? Kitchen2PrinterName { get; set; }
    public string? Kitchen2AltPrinterName { get; set; }
    public int? Kitchen2PrinterID { get; set; }
    public string? Kitchen2DesignPath { get; set; }

    public string? Kitchen3PrinterName { get; set; }
    public string? Kitchen3AltPrinterName { get; set; }
    public int? Kitchen3PrinterID { get; set; }
    public string? Kitchen3DesignPath { get; set; }

    public string? Kitchen4PrinterName { get; set; }
    public string? Kitchen4AltPrinterName { get; set; }
    public int? Kitchen4PrinterID { get; set; }
    public string? Kitchen4DesignPath { get; set; }

    public string? Kitchen5PrinterName { get; set; }
    public string? Kitchen5AltPrinterName { get; set; }
    public int? Kitchen5PrinterID { get; set; }
    public string? Kitchen5DesignPath { get; set; }

    public string? Kitchen6PrinterName { get; set; }
    public string? Kitchen6AltPrinterName { get; set; }
    public int? Kitchen6PrinterID { get; set; }
    public string? Kitchen6DesignPath { get; set; }

    public string? Kitchen7PrinterName { get; set; }
    public string? Kitchen7AltPrinterName { get; set; }
    public int? Kitchen7PrinterID { get; set; }
    public string? Kitchen7DesignPath { get; set; }

    public string? Kitchen8PrinterName { get; set; }
    public string? Kitchen8AltPrinterName { get; set; }
    public int? Kitchen8PrinterID { get; set; }
    public string? Kitchen8DesignPath { get; set; }

    public string? Kitchen9PrinterName { get; set; }
    public string? Kitchen9AltPrinterName { get; set; }
    public int? Kitchen9PrinterID { get; set; }
    public string? Kitchen9DesignPath { get; set; }

    public string? Kitchen10PrinterName { get; set; }
    public string? Kitchen10AltPrinterName { get; set; }
    public int? Kitchen10PrinterID { get; set; }
    public string? Kitchen10DesignPath { get; set; }

    public int? InvoiceTopFeed { get; set; }
    public int? AdditionTopFeed { get; set; }
    public bool PrintDineInOrdersKitchen { get; set; } = true;
    public bool PrintBarTableOrdersKitchen { get; set; } = true;
    public bool PrintTakeOutOrdersKitchen { get; set; } = true;
    public bool PrintDriveThruOrdersKitchen { get; set; } = true;
    public bool PrintDeliveryOrdersKitchen { get; set; } = true;
    public string? EditKey { get; set; }
    public string? SyncKey { get; set; }
    public int? LabelPrinterID { get; set; }
    public string? LabelPrinterName { get; set; }
    public string? LabelDesignPath { get; set; }
    public int? ReturnPrinterID { get; set; }
    public string? ReturnPrinterName { get; set; }
    public string? ReturnDesignPath { get; set; }
}
