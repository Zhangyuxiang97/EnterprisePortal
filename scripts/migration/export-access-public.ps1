param(
    [Parameter(Mandatory = $true)][string]$DatabasePath,
    [Parameter(Mandatory = $true)][string]$OutputDirectory
)
$ErrorActionPreference = 'Stop'
$sourcePath = (Resolve-Path -LiteralPath $DatabasePath).Path
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$copyPath = Join-Path (Resolve-Path -LiteralPath $OutputDirectory).Path 'source.mdb'
if ($sourcePath -eq $copyPath) { throw '导出目录不能与原始 MDB 文件路径相同。' }
Copy-Item -LiteralPath $sourcePath -Destination $copyPath -Force
$sourceHash = (Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256).Hash
if ((Get-FileHash -LiteralPath $copyPath -Algorithm SHA256).Hash -ne $sourceHash) { throw '备份副本校验失败。' }
$connection = New-Object -ComObject ADODB.Connection
$connection.Mode = 1
$connection.Open("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$copyPath;Mode=Read;")
try {
    # 只导出旧站公开展示表，不读取管理员口令、留言联系方式或订单个人信息。
    $exports = [ordered]@{
        news = 'id, news_title, news_content, news_bigcid, news_smallcid, news_author, news_image, news_showtop, news_flash, news_hits, news_addtime, px_id, html_url, news_stickies, tuijian, jianjie'
        news_bigclass = 'ID, bigtitle, px_id'
        news_smallclass = 'ID, bigcid, smalltitle, px_id'
        about = 'ID, title, about_smallcid, content, html_url, px_id, showtop'
        LINK = 'ID, link_title, link_url, link_image, px_id'
    }
    foreach ($table in $exports.Keys) {
        $fields = $exports[$table].Split(',') | ForEach-Object { $_.Trim() }
        $projection = ($fields | ForEach-Object { '[' + $_ + ']' }) -join ', '
        $records = $connection.Execute("SELECT $projection FROM [$table]")
        $rows = New-Object System.Collections.Generic.List[object]
        while (-not $records.EOF) {
            $row = [ordered]@{}
            foreach ($field in $fields) {
                $value = $records.Fields.Item($field).Value
                if ($value -is [DBNull]) { $value = $null }
                if ($value -is [DateTime]) { $value = $value.ToString('o') }
                $row[$field] = $value
            }
            $rows.Add([PSCustomObject]$row)
            $records.MoveNext()
        }
        $records.Close()
        $json = ConvertTo-Json -InputObject $rows.ToArray() -Depth 5 -Compress
        [System.IO.File]::WriteAllText((Join-Path $OutputDirectory ($table + '.json')), $json, (New-Object System.Text.UTF8Encoding($false)))
        Write-Output ($table + ': exported ' + $rows.Count)
    }
} finally { $connection.Close() }
