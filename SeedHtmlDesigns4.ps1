$connStr = "Server=TERMSRV;Database=infinia;Trusted_Connection=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()

# Get all TASARIM (ReportTypeID = 1)
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT ReportID, ReportKey, ReportName, ISNULL(IsPosReport, 0) FROM Reports WHERE ReportTypeID = 1"
$reader = $cmd.ExecuteReader()

$reports = @()
while ($reader.Read()) {
    $reports += @{
        ReportID = $reader.GetInt32(0)
        ReportKey = $reader.GetGuid(1)
        ReportName = $reader.GetString(2)
        IsPos = $reader.GetBoolean(3)
    }
}
$reader.Close()

foreach ($r in $reports) {
    $rId = $r.ReportID
    $rKey = $r.ReportKey
    $rName = $r.ReportName
    $isPos = $r.IsPos

    $qCmd = $conn.CreateCommand()
    $qCmd.CommandText = "SELECT QueryName FROM ReportQueries WHERE ReportID = $rId"
    $qReader = $qCmd.ExecuteReader()
    
    $queries = @()
    while ($qReader.Read()) {
        $qName = $qReader.GetString(0)
        if ($qName -ne "XAML_TEMPLATE") {
            $queries += $qName
        }
    }
    $qReader.Close()

    if ($isPos) {
        $html = @"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <style>
        body { background-color: #555; margin: 20px 0; text-align: center; font-family: 'Consolas', 'Courier New', monospace; font-size: 11px; color: #000; }
        .receipt-container { background-color: #fff; width: 80mm; margin: 0 auto; padding: 5mm; text-align: left; box-shadow: 0 4px 8px rgba(0,0,0,0.5); overflow: hidden; }
        .report-header { text-align: center; font-weight: bold; margin-bottom: 10px; font-size: 14px; }
        .section-title { font-weight: bold; text-align: center; text-transform: uppercase; border-bottom: 1px dashed #000; padding-bottom: 2px; margin-top: 15px; margin-bottom: 5px; }
        .report-table { width: 100%; border-collapse: collapse; margin-bottom: 10px; table-layout: fixed; word-wrap: break-word; }
        .report-table th { border-bottom: 1px dashed #000; padding: 2px 1px; font-weight: bold; font-size: 10px; }
        .report-table td { padding: 2px 1px; font-size: 10px; }
        .text-left { text-align: left; }
        .text-right { text-align: right; }
    </style>
</head>
<body>
    <div class="receipt-container">
        <div class="report-header">
            $rName<br/>
            ** GÜN SONU RAPORU **
        </div>
"@
    } else {
        $html = @"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <style>
        body { font-family: 'Segoe UI', Tahoma, sans-serif; font-size: 14px; padding: 20px; background-color: #f9f9f9; color: #333; }
        .page-container { background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); max-width: 1000px; margin: 0 auto; }
        .report-header { text-align: center; color: #d32f2f; margin-bottom: 30px; font-weight: bold; font-size: 24px; }
        .section { margin-bottom: 20px; }
        .section-title { font-weight: bold; font-size: 18px; border-bottom: 2px solid #ffc107; padding-bottom: 5px; margin-bottom: 15px; text-transform: uppercase; }
        .report-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }
        .report-table th { background-color: #f2f2f2; border-bottom: 2px solid #ddd; padding: 8px; font-weight: bold; }
        .report-table td { border-bottom: 1px solid #ddd; padding: 8px; }
        .text-left { text-align: left; }
        .text-right { text-align: right; }
    </style>
</head>
<body>
    <div class="page-container">
        <div class="report-header">$rName</div>
"@
    }

    foreach ($q in $queries) {
        if ($isPos) {
            $html += @"
`n        <div class="section-title">$q</div>
        {{$q}}
"@
        } else {
            $html += @"
`n        <div class="section">
            <div class="section-title">$q</div>
            {{$q}}
        </div>
"@
        }
    }

    if ($isPos) {
        $html += @"
`n    </div>
</body>
</html>
"@
    } else {
        $html += @"
`n    </div>
</body>
</html>
"@
    }

    # Save to ReportDesigns
    $delCmd = $conn.CreateCommand()
    $delCmd.CommandText = "DELETE FROM ReportDesigns WHERE ReportID = $rId"
    $delCmd.ExecuteNonQuery() > $null

    $insCmd = $conn.CreateCommand()
    $insCmd.CommandText = "INSERT INTO ReportDesigns (ReportDesignKey, ReportID, ReportKey, DocumentTypeID, DesignName, DesignData, IsDefault) VALUES (NEWID(), @ID, @Key, 0, @Name, @Data, 0)"
    
    $pID = $insCmd.Parameters.AddWithValue("@ID", $rId)
    $pKey = $insCmd.Parameters.AddWithValue("@Key", $rKey)
    $pName = $insCmd.Parameters.AddWithValue("@Name", $rName)
    $pData = $insCmd.Parameters.AddWithValue("@Data", [System.Text.Encoding]::UTF8.GetBytes($html))
    
    $insCmd.ExecuteNonQuery() > $null
    Write-Host "Seeded HTML for Report (POS=$isPos): $rName"
}

$conn.Close()
Write-Host "Done!"
