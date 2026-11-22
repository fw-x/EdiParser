$EdiParserFiles = Get-ChildItem "C:\Users\floyd\sandbox\EdiParser" -Recurse -Filter "*.cs" | Select-Object -ExpandProperty FullName
Add-Type -Path $EdiParserFiles

function Fast834Parser {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Path,

        [Parameter(Mandatory = $true)]
        [string[]]$Find,

        [Parameter(Mandatory = $false)]
        [int]$MaxDegreeOfParallelism = [Environment]::ProcessorCount,

        [ValidateSet("Display", "Grid", "Csv")]
        [string]$OutputMode = default,

        [Parameter(Mandatory = $false)]
        [string]$CsvPath
        
    )


    # Validate that CsvPath is provided if OutputMode is Csv
    if ($OutputMode -eq "Csv" -and [string]::IsNullOrWhiteSpace($CsvPath)) {
        throw "The -CsvPath parameter is required when OutputMode is set to 'Csv'."
    }

    # 2. Call your static C# master entry point directly
    $results = [EdiParser.Fast834Parser]::ParseFiles(
        $FilePaths,
        $Find,
        $MaxDegreeOfParallelism
    )


    switch ($OutputMode) {
        "Grid"   { 
            $results | Out-GridView
            break 
        }
        "Csv"    {
            $results | Export-Csv -Path $CsvPath -NoTypeInformation
            Write-Host "Results exported to $CsvPath" -ForegroundColor Green
            break
        }
        "Display"  {
            $results | Format-Table -AutoSize
            break
        }

        default { return $results}
    }
}