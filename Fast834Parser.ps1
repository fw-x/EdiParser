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
        [string]$OutputMode = "Display"
        
    )

    # 2. Call your static C# master entry point directly
    $results = [EdiParser.Fast834Parser]::ParseFiles(
        $FilePaths,
        $TargetIds,
        $MaxDegreeOfParallelism
    )


    switch ($OutputMode) {
        "Grid"   { 
            $results | Out-GridView
            break 
        }
        "Csv"    {
            $results | Export-Csv "C:\Temp\ParsedResults.csv" -NoTypeInformation
            Write-Host "Results exported to C:\Temp\ParsedResults.csv" -ForegroundColor Green
            break
        }
        "Display"  {
            $results | Format-Table -AutoSize
            break
        }

        default { return $results}
    }
}