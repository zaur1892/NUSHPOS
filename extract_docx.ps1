$word = New-Object -ComObject Word.Application
$word.Visible = $false
$doc = $word.Documents.Open("c:\Users\Zaur\Desktop\NUSHPOS\Inplementation Plan.docx")
$text = $doc.Content.Text
$text | Out-File -FilePath "c:\Users\Zaur\Desktop\NUSHPOS\implementation_plan_text.txt" -Encoding UTF8
$doc.Close()
$word.Quit()
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($word) | Out-Null
