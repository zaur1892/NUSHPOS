using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FastReport;

namespace NUSHPOS.Services;

public class FastReportService
{
    private static readonly string[] PossibleDesignerPaths = new[]
    {
        @"C:\Program Files (x86)\FastReports\FastReport.Net\Designer.exe",
        @"C:\Program Files\FastReports\FastReport.Net\Designer.exe",
        @"C:\Program Files (x86)\FastReports\FastReport Community\Designer.exe",
        @"C:\Program Files\FastReports\FastReport Community\Designer.exe"
    };

    public string GetDefaultReceiptTemplate(string designName)
    {
        if (designName.Contains("Mətbəx", StringComparison.OrdinalIgnoreCase) ||
            designName.Contains("Kitchen", StringComparison.OrdinalIgnoreCase) ||
            designName.Contains("Sifariş", StringComparison.OrdinalIgnoreCase) ||
            designName.Contains("Bar", StringComparison.OrdinalIgnoreCase))
        {
            return GetKitchenOrderTemplate();
        }

        return GetHesabReceiptTemplate();
    }

    public string GetHesabReceiptTemplate()
    {
        return @"<?xml version=""1.0"" encoding=""utf-8""?>
<Report ScriptLanguage=""CSharp"" ReportInfo.Created=""2026-09-05 00:00:00"" ReportInfo.Modified=""2026-09-05 00:00:00"" ReportInfo.CreatorVersion=""2026.2.3.0"">
  <Dictionary>
    <TableDataSource Name=""OrderTransactions"" ReferenceName=""OrderTransactions"" DataType=""System.Int32"" Enabled=""true"">
      <Column Name=""MenuItemText"" DataType=""System.String""/>
      <Column Name=""Quantity"" DataType=""System.Decimal""/>
      <Column Name=""MenuItemUnitPrice"" DataType=""System.Decimal""/>
      <Column Name=""ExtendedPrice"" DataType=""System.Decimal""/>
      <Column Name=""Notes"" DataType=""System.String""/>
      <Column Name=""DisplayText"" DataType=""System.String""/>
      <Column Name=""AddDateTime"" DataType=""System.DateTime""/>
    </TableDataSource>
    <Parameter Name=""RestaurantName"" DataType=""System.String"" Expression=""&quot;NUSH RESTORAN&quot;""/>
    <Parameter Name=""OrderID"" DataType=""System.String"" Expression=""&quot;5296&quot;""/>
    <Parameter Name=""ReceiptDate"" DataType=""System.String"" Expression=""&quot;05/09/2026 00:45&quot;""/>
    <Parameter Name=""WaiterName"" DataType=""System.String"" Expression=""&quot;Kassir&quot;""/>
    <Parameter Name=""TableName"" DataType=""System.String"" Expression=""&quot;MASA 1&quot;""/>
    <Parameter Name=""GuestNumber"" DataType=""System.Int32"" Expression=""2""/>
    <Parameter Name=""DiscountText"" DataType=""System.String"" Expression=""&quot;&quot;""/>
    <Parameter Name=""DiscountAmount"" DataType=""System.Decimal"" Expression=""0.00""/>
    <Parameter Name=""GrandTotal"" DataType=""System.Decimal"" Expression=""45.00""/>
    <Parameter Name=""OrderNotes"" DataType=""System.String"" Expression=""&quot;&quot;""/>
    <Parameter Name=""PhoneInfo"" DataType=""System.String"" Expression=""&quot;Tel: (012) 000-00-00&quot;""/>
  </Dictionary>
  <ReportPage Name=""Page1"" PaperWidth=""80"" PaperHeight=""297"" LeftMargin=""2"" TopMargin=""2"" RightMargin=""2"" BottomMargin=""2"" FirstPageSource=""1"" OtherPagesSource=""1"">
    <ReportTitleBand Name=""ReportTitle1"" Width=""287.28"" Height=""130"">
      <TextObject Name=""TxtTitle"" Left=""0"" Top=""5"" Width=""287.28"" Height=""26"" Text=""[RestaurantName]"" Font=""Arial, 13pt, style=Bold"" HorzAlign=""Center""/>
      <LineObject Name=""LineTop"" Left=""0"" Top=""35"" Width=""287.28""/>
      <TextObject Name=""TxtOrderID"" Left=""0"" Top=""40"" Width=""287.28"" Height=""16"" Text=""Çek №: [OrderID]"" Font=""Arial, 9pt, style=Bold""/>
      <TextObject Name=""TxtDate"" Left=""0"" Top=""57"" Width=""287.28"" Height=""16"" Text=""Tarix: [ReceiptDate]"" Font=""Arial, 8.5pt""/>
      <TextObject Name=""TxtWaiter"" Left=""0"" Top=""74"" Width=""287.28"" Height=""16"" Text=""Personal: [WaiterName]"" Font=""Arial, 8.5pt""/>
      <TextObject Name=""TxtTable"" Left=""0"" Top=""91"" Width=""170"" Height=""16"" Text=""Masa №: [TableName]"" Font=""Arial, 9pt, style=Bold""/>
      <TextObject Name=""TxtGuest"" Left=""170"" Top=""91"" Width=""117.28"" Height=""16"" Text=""Qonaq: [GuestNumber]"" Font=""Arial, 8.5pt"" HorzAlign=""Right""/>
      <LineObject Name=""LineHeader"" Left=""0"" Top=""125"" Width=""287.28"" Style=""Dash""/>
    </ReportTitleBand>
    <PageHeaderBand Name=""PageHeader1"" Top=""134"" Width=""287.28"" Height=""22"">
      <TextObject Name=""TxtHeaderName"" Left=""0"" Top=""2"" Width=""135"" Height=""16"" Text=""Məhsul"" Font=""Arial, 8.5pt, style=Bold""/>
      <TextObject Name=""TxtHeaderQty"" Left=""135"" Top=""2"" Width=""40"" Height=""16"" Text=""Miq."" Font=""Arial, 8.5pt, style=Bold"" HorzAlign=""Center""/>
      <TextObject Name=""TxtHeaderPrice"" Left=""175"" Top=""2"" Width=""50"" Height=""16"" Text=""Qiy."" Font=""Arial, 8.5pt, style=Bold"" HorzAlign=""Right""/>
      <TextObject Name=""TxtHeaderTotal"" Left=""225"" Top=""2"" Width=""62.28"" Height=""16"" Text=""Cəmi"" Font=""Arial, 8.5pt, style=Bold"" HorzAlign=""Right""/>
      <LineObject Name=""LineCols"" Left=""0"" Top=""20"" Width=""287.28""/>
    </PageHeaderBand>
    <DataBand Name=""Data1"" Top=""160"" Width=""287.28"" Height=""20"" DataSource=""OrderTransactions"">
      <TextObject Name=""TxtItemName"" Left=""0"" Top=""2"" Width=""135"" Height=""16"" Text=""[OrderTransactions.MenuItemText]"" Font=""Arial, 8.5pt""/>
      <TextObject Name=""TxtQty"" Left=""135"" Top=""2"" Width=""40"" Height=""16"" Text=""[OrderTransactions.Quantity]"" Font=""Arial, 8.5pt"" HorzAlign=""Center"" Format=""Number"" Format.UseLocale=""false"" Format.DecimalDigits=""0""/>
      <TextObject Name=""TxtPrice"" Left=""175"" Top=""2"" Width=""50"" Height=""16"" Text=""[OrderTransactions.MenuItemUnitPrice]"" Font=""Arial, 8.5pt"" HorzAlign=""Right"" Format=""Custom"" Format.Format=""0.00""/>
      <TextObject Name=""TxtTotal"" Left=""225"" Top=""2"" Width=""62.28"" Height=""16"" Text=""[OrderTransactions.ExtendedPrice]"" Font=""Arial, 8.5pt"" HorzAlign=""Right"" Format=""Custom"" Format.Format=""0.00""/>
    </DataBand>
    <ReportSummaryBand Name=""ReportSummary1"" Top=""184"" Width=""287.28"" Height=""110"">
      <LineObject Name=""LineSum"" Left=""0"" Top=""2"" Width=""287.28""/>
      <TextObject Name=""TxtGrandTotal"" Left=""0"" Top=""10"" Width=""287.28"" Height=""24"" Text=""CƏMİ : [GrandTotal] AZN"" Font=""Arial, 12pt, style=Bold"" HorzAlign=""Center""/>
      <LineObject Name=""LineBottom"" Left=""0"" Top=""38"" Width=""287.28"" Style=""Dash""/>
      <TextObject Name=""TxtNotes"" Left=""0"" Top=""44"" Width=""287.28"" Height=""16"" Text=""[OrderNotes]"" Font=""Arial, 8pt, style=Italic"" HorzAlign=""Center""/>
      <TextObject Name=""TxtThanks"" Left=""0"" Top=""62"" Width=""287.28"" Height=""20"" Text=""TƏŞƏKKÜR EDİRİK!"" Font=""Arial, 9.5pt, style=Bold"" HorzAlign=""Center""/>
      <TextObject Name=""TxtPhone"" Left=""0"" Top=""84"" Width=""287.28"" Height=""16"" Text=""[PhoneInfo]"" Font=""Arial, 8pt, style=Italic"" HorzAlign=""Center""/>
    </ReportSummaryBand>
  </ReportPage>
</Report>";
    }

    public string GetKitchenOrderTemplate()
    {
        return @"<?xml version=""1.0"" encoding=""utf-8""?>
<Report ScriptLanguage=""CSharp"" ReportInfo.Created=""2026-09-05 00:00:00"" ReportInfo.Modified=""2026-09-05 00:00:00"" ReportInfo.CreatorVersion=""2026.2.3.0"">
  <Dictionary>
    <TableDataSource Name=""OrderTransactions"" ReferenceName=""OrderTransactions"" DataType=""System.Int32"" Enabled=""true"">
      <Column Name=""MenuItemText"" DataType=""System.String""/>
      <Column Name=""Quantity"" DataType=""System.Decimal""/>
      <Column Name=""MenuItemUnitPrice"" DataType=""System.Decimal""/>
      <Column Name=""ExtendedPrice"" DataType=""System.Decimal""/>
      <Column Name=""Notes"" DataType=""System.String""/>
      <Column Name=""DisplayText"" DataType=""System.String""/>
      <Column Name=""AddDateTime"" DataType=""System.DateTime""/>
    </TableDataSource>
    <Parameter Name=""Title"" DataType=""System.String"" Expression=""&quot;MƏTBƏX SİFARİŞİ&quot;""/>
    <Parameter Name=""OrderID"" DataType=""System.String"" Expression=""&quot;5296&quot;""/>
    <Parameter Name=""ReceiptDate"" DataType=""System.String"" Expression=""&quot;05.09.2026&quot;""/>
    <Parameter Name=""ReceiptTime"" DataType=""System.String"" Expression=""&quot;00:45:00&quot;""/>
    <Parameter Name=""WaiterName"" DataType=""System.String"" Expression=""&quot;Kassir&quot;""/>
    <Parameter Name=""TableName"" DataType=""System.String"" Expression=""&quot;MASA 1&quot;""/>
    <Parameter Name=""GuestNumber"" DataType=""System.Int32"" Expression=""2""/>
    <Parameter Name=""OrderNotes"" DataType=""System.String"" Expression=""&quot;&quot;""/>
  </Dictionary>
  <ReportPage Name=""Page1"" PaperWidth=""80"" PaperHeight=""297"" LeftMargin=""2"" TopMargin=""2"" RightMargin=""2"" BottomMargin=""2"" FirstPageSource=""1"" OtherPagesSource=""1"">
    <ReportTitleBand Name=""ReportTitle1"" Width=""287.28"" Height=""145"">
      <TextObject Name=""TxtTitle"" Left=""0"" Top=""5"" Width=""287.28"" Height=""26"" Text=""[Title]"" Font=""Arial, 13pt, style=Bold"" HorzAlign=""Center""/>
      <LineObject Name=""LineTop"" Left=""0"" Top=""33"" Width=""287.28""/>
      <TextObject Name=""TxtOrderID"" Left=""0"" Top=""38"" Width=""287.28"" Height=""18"" Text=""ÇEK №: [OrderID]"" Font=""Arial, 10pt, style=Bold"" HorzAlign=""Center""/>
      <TextObject Name=""TxtDate"" Left=""0"" Top=""58"" Width=""287.28"" Height=""16"" Text=""Tarix: [ReceiptDate]"" Font=""Arial, 9pt""/>
      <TextObject Name=""TxtTime"" Left=""0"" Top=""75"" Width=""287.28"" Height=""16"" Text=""Sifariş Saatı: [ReceiptTime]"" Font=""Arial, 9pt""/>
      <TextObject Name=""TxtWaiter"" Left=""0"" Top=""92"" Width=""287.28"" Height=""16"" Text=""Personal: [WaiterName]"" Font=""Arial, 9pt""/>
      <TextObject Name=""TxtTable"" Left=""0"" Top=""110"" Width=""170"" Height=""18"" Text=""Masa: [TableName]"" Font=""Arial, 11pt, style=Bold""/>
      <TextObject Name=""TxtGuest"" Left=""170"" Top=""110"" Width=""117.28"" Height=""18"" Text=""Qonaq: [GuestNumber]"" Font=""Arial, 9pt"" HorzAlign=""Right""/>
      <LineObject Name=""LineHeader"" Left=""0"" Top=""140"" Width=""287.28"" Style=""Solid""/>
    </ReportTitleBand>
    <DataBand Name=""Data1"" Top=""150"" Width=""287.28"" Height=""28"" DataSource=""OrderTransactions"">
      <TextObject Name=""TxtQty"" Left=""0"" Top=""2"" Width=""35"" Height=""22"" Text=""[OrderTransactions.Quantity]"" Font=""Arial, 12pt, style=Bold"" HorzAlign=""Left"" Format=""Number"" Format.UseLocale=""false"" Format.DecimalDigits=""0""/>
      <TextObject Name=""TxtItemName"" Left=""38"" Top=""2"" Width=""249.28"" Height=""22"" Text=""x  [OrderTransactions.MenuItemText]"" Font=""Arial, 11pt, style=Bold""/>
      <LineObject Name=""LineItem"" Left=""0"" Top=""26"" Width=""287.28"" Style=""Dot""/>
    </DataBand>
    <ReportSummaryBand Name=""ReportSummary1"" Top=""185"" Width=""287.28"" Height=""50"">
      <LineObject Name=""LineSum"" Left=""0"" Top=""5"" Width=""287.28""/>
      <TextObject Name=""TxtPrintTime"" Left=""0"" Top=""15"" Width=""287.28"" Height=""24"" Text=""SAAT : [ReceiptTime]"" Font=""Arial, 12pt, style=Bold"" HorzAlign=""Center""/>
    </ReportSummaryBand>
  </ReportPage>
</Report>";
    }

    public string ExportToTempFrx(string designName, string designData)
    {
        string safeName = string.Join("_", designName.Split(Path.GetInvalidFileNameChars()));
        if (string.IsNullOrWhiteSpace(safeName)) safeName = "ReceiptDesign";

        string tempFolder = Path.Combine(Path.GetTempPath(), "NUSHPOS_Designs");
        if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);

        string filePath = Path.Combine(tempFolder, $"{safeName}.frx");
        File.WriteAllText(filePath, designData, Encoding.UTF8);
        return filePath;
    }

    public bool TryLaunchExternalDesigner(string frxFilePath)
    {
        foreach (var path in PossibleDesignerPaths)
        {
            if (File.Exists(path))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = $"\"{frxFilePath}\"",
                        UseShellExecute = true
                    });
                    return true;
                }
                catch { }
            }
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = frxFilePath,
                UseShellExecute = true
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GeneratePreviewPdf(string designData, string designName)
    {
        using var report = new FastReport.Report();
        report.LoadFromString(designData);

        var dt = new System.Data.DataTable("OrderTransactions");
        dt.Columns.Add("MenuItemText", typeof(string));
        dt.Columns.Add("DisplayText", typeof(string));
        dt.Columns.Add("Quantity", typeof(decimal));
        dt.Columns.Add("MenuItemUnitPrice", typeof(decimal));
        dt.Columns.Add("ExtendedPrice", typeof(decimal));
        dt.Columns.Add("Notes", typeof(string));
        dt.Columns.Add("AddDateTime", typeof(DateTime));

        dt.Rows.Add("Mərci Şorbası", "Mərci Şorbası", 2m, 4.00m, 8.00m, "", DateTime.Now);
        dt.Rows.Add("Lülə Kabab", "Lülə Kabab", 3m, 9.00m, 27.00m, "", DateTime.Now);
        dt.Rows.Add("Çoban Salatı", "Çoban Salatı", 1m, 5.00m, 5.00m, "", DateTime.Now);
        dt.Rows.Add("Coca-Cola 0.5L", "Coca-Cola 0.5L", 2m, 2.50m, 5.00m, "", DateTime.Now);

        var dataSet = new System.Data.DataSet("OrderTransactions");
        dataSet.Tables.Add(dt);

        report.RegisterData(dataSet, "OrderTransactions");
        report.RegisterData(dt, "OrderTransactions");

        // Also register OrderItems for compatibility
        var dtItems = dt.Copy();
        dtItems.TableName = "OrderItems";
        dtItems.Columns.Add("ItemName", typeof(string));
        dtItems.Columns.Add("Price", typeof(decimal));
        dtItems.Columns.Add("LineTotal", typeof(decimal));
        foreach (System.Data.DataRow row in dtItems.Rows)
        {
            row["ItemName"] = row["MenuItemText"];
            row["Price"] = row["MenuItemUnitPrice"];
            row["LineTotal"] = row["ExtendedPrice"];
        }
        report.RegisterData(dtItems, "OrderItems");

        // Enable and connect all data sources
        if (report.Dictionary != null)
        {
            foreach (FastReport.Data.DataSourceBase ds in report.Dictionary.DataSources)
            {
                ds.Enabled = true;
                if (ds is FastReport.Data.TableDataSource tds)
                {
                    if (tds.Name.Equals("OrderItems", StringComparison.OrdinalIgnoreCase))
                    {
                        tds.Table = dtItems;
                    }
                    else
                    {
                        tds.Table = dt;
                    }
                }
            }
        }

        // Safely assign parameters if they exist in report dictionary
        SetParameterIfExists(report, "RestaurantName", "NUSH RESTORAN");
        SetParameterIfExists(report, "Title", "MƏTBƏX SİFARİŞİ");
        SetParameterIfExists(report, "OrderID", "5296");
        SetParameterIfExists(report, "TableName", "MASA 5");
        SetParameterIfExists(report, "WaiterName", "Kassir");
        SetParameterIfExists(report, "GuestNumber", 2);
        SetParameterIfExists(report, "GrandTotal", 45.00m);
        SetParameterIfExists(report, "DiscountText", "");
        SetParameterIfExists(report, "DiscountAmount", 0.00m);
        SetParameterIfExists(report, "OrderNotes", "");
        SetParameterIfExists(report, "PhoneInfo", "Tel: (012) 000-00-00");
        SetParameterIfExists(report, "ReceiptDate", DateTime.Now.ToString("dd.MM.yyyy"));
        SetParameterIfExists(report, "ReceiptTime", DateTime.Now.ToString("HH:mm:ss"));

        report.Prepare();

        string safeName = string.Join("_", designName.Split(Path.GetInvalidFileNameChars()));
        if (string.IsNullOrWhiteSpace(safeName)) safeName = "ReceiptDesign";

        string tempFolder = Path.Combine(Path.GetTempPath(), "NUSHPOS_Designs");
        if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);

        string pdfPath = Path.Combine(tempFolder, $"{safeName}_Preview.pdf");
        
        using var pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
        pdfExport.Export(report, pdfPath);

        return pdfPath;
    }

    public async Task<bool> PrintReportAsync(string designData, string printerName, Dictionary<string, object>? parameters, DataTable dataTable, int copies = 1)
    {
        return await Task.Run(() => PrintReport(designData, printerName, parameters, dataTable, copies));
    }

    public bool PrintReport(string designData, string printerName, Dictionary<string, object>? parameters, DataTable dataTable, int copies = 1)
    {
        if (string.IsNullOrWhiteSpace(printerName) || string.IsNullOrWhiteSpace(designData))
        {
            return false;
        }

        try
        {
            using var report = new FastReport.Report();
            report.LoadFromString(designData);

            var dataSet = new DataSet("OrderTransactions");
            var dt = dataTable.Copy();
            dt.TableName = "OrderTransactions";
            dataSet.Tables.Add(dt);

            report.RegisterData(dataSet, "OrderTransactions");
            report.RegisterData(dt, "OrderTransactions");

            // Also register OrderItems for compatibility
            var dtItems = dt.Copy();
            dtItems.TableName = "OrderItems";
            if (!dtItems.Columns.Contains("ItemName"))
                dtItems.Columns.Add("ItemName", typeof(string));
            if (!dtItems.Columns.Contains("Price"))
                dtItems.Columns.Add("Price", typeof(decimal));
            if (!dtItems.Columns.Contains("LineTotal"))
                dtItems.Columns.Add("LineTotal", typeof(decimal));

            foreach (DataRow row in dtItems.Rows)
            {
                if (dtItems.Columns.Contains("MenuItemText")) row["ItemName"] = row["MenuItemText"];
                if (dtItems.Columns.Contains("MenuItemUnitPrice")) row["Price"] = row["MenuItemUnitPrice"];
                if (dtItems.Columns.Contains("ExtendedPrice")) row["LineTotal"] = row["ExtendedPrice"];
            }
            report.RegisterData(dtItems, "OrderItems");

            // Enable and connect data sources
            if (report.Dictionary != null)
            {
                foreach (FastReport.Data.DataSourceBase ds in report.Dictionary.DataSources)
                {
                    ds.Enabled = true;
                    if (ds is FastReport.Data.TableDataSource tds)
                    {
                        if (tds.Name.Equals("OrderItems", StringComparison.OrdinalIgnoreCase))
                        {
                            tds.Table = dtItems;
                        }
                        else
                        {
                            tds.Table = dt;
                        }
                    }
                }
            }

            // Set parameters
            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    SetParameterIfExists(report, kvp.Key, kvp.Value);
                }
            }

            report.Prepare();

            string tempFolder = Path.Combine(Path.GetTempPath(), "NUSHPOS_Prints");
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);

            string orderIdStr = parameters != null && parameters.TryGetValue("OrderID", out var oVal) ? oVal?.ToString() ?? "0" : "0";
            string titleStr = parameters != null && parameters.TryGetValue("Title", out var tVal) ? tVal?.ToString() ?? "Hesab" : "Hesab";
            string safeTitle = string.Join("_", titleStr.Split(Path.GetInvalidFileNameChars()));
            string fileName = $"{safeTitle}_{orderIdStr}_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString().Substring(0, 4)}.pdf";
            string pdfPath = Path.Combine(tempFolder, fileName);

            using var pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
            pdfExport.Export(report, pdfPath);

            // If virtual PDF printer (e.g. Microsoft Print to PDF) or testing
            if (printerName.Contains("PDF", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string desktopFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "NUSHPOS_Prints");
                    if (!Directory.Exists(desktopFolder)) Directory.CreateDirectory(desktopFolder);
                    string desktopPdf = Path.Combine(desktopFolder, fileName);
                    File.Copy(pdfPath, desktopPdf, true);
                    pdfPath = desktopPdf;
                }
                catch { }

                Process.Start(new ProcessStartInfo
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                });
                return true;
            }

            // For physical printer: send to printer via Edge headless
            string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            if (!File.Exists(edgePath))
            {
                edgePath = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";
            }

            if (File.Exists(edgePath))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = edgePath,
                    Arguments = $"--headless --disable-gpu --print-to-printer --printer-name=\"{printerName}\" \"{pdfPath}\"",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false
                };
                using var proc = Process.Start(psi);
                proc?.WaitForExit(10000);
                return true;
            }

            // Fallback to PrintTo shell verb
            var printPsi = new ProcessStartInfo
            {
                FileName = pdfPath,
                Verb = "PrintTo",
                Arguments = $"\"{printerName}\"",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };
            using var printProc = Process.Start(printPsi);
            printProc?.WaitForExit(10000);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FastReport Print Error: {ex}");
            return false;
        }
    }

    private static void SetParameterIfExists(FastReport.Report report, string name, object value)
    {
        try
        {
            if (report.Parameters.FindByName(name) != null)
            {
                report.SetParameterValue(name, value);
            }
        }
        catch { }
    }
}

public class SampleOrderTransaction
{
    public string MenuItemText { get; set; } = string.Empty;
    public string DisplayText { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal MenuItemUnitPrice { get; set; }
    public decimal ExtendedPrice { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime AddDateTime { get; set; } = DateTime.Now;
}
