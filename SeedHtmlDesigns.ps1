$connStr = "Server=TERMSRV;Database=infinia;Trusted_Connection=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()

# Get all TASARIM (ReportTypeID = 1)
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT ReportID, ReportKey, ReportName FROM Reports WHERE ReportTypeID = 1"
$reader = $cmd.ExecuteReader()

$reports = @()
while ($reader.Read()) {
    $reports += @{
        ReportID = $reader.GetInt32(0)
        ReportKey = $reader.GetGuid(1)
        ReportName = $reader.GetString(2)
    }
}
$reader.Close()

foreach ($r in $reports) {
    $rId = $r.ReportID
    $rKey = $r.ReportKey
    $rName = $r.ReportName

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

    $html = @"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <style>
        body { font-family: 'Segoe UI', Tahoma, sans-serif; padding: 20px; background-color: #f9f9f9; }
        .report-header { text-align: center; color: #d32f2f; margin-bottom: 30px; font-weight:bold; font-size:24px; }
        .section { background: white; padding: 15px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); margin-bottom: 20px; }
        .section-title { font-weight: bold; font-size: 18px; border-bottom: 2px solid #ffc107; padding-bottom: 5px; margin-bottom: 15px; text-transform: uppercase; }
    </style>
</head>
<body>
    <div class="report-header">$rName</div>
"@

    foreach ($q in $queries) {
        $html += @"
`n    <div class="section">
        <div class="section-title">$q</div>
        {{$q}}
    </div>
"@
    }

    $html += @"
`n</body>
</html>
"@

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
    Write-Host "Seeded HTML for Report: $rName"
}

$conn.Close()
Write-Host "Done!"
