function NormalizeDateFormat {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$DateStringFormat  
    )

    $normalizedFormat = $DateStringFormat -replace 'Y', 'y' -replace 'D', 'd' -replace 'm', 'M'
        return $normalizedFormat
}

function CarrierFileAttributes {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Carrier
    )
    switch ($Carrier) {
        "Anthem"   { 
            return [PSCustomObject]@{
                date_frmt       = NormalizeDateFormat "yyyyMMdd" 
                file_pattern    = 'some_file_name_(\d{8})_(.{8})\.txt'
            }
        }
        "Kaiser"    {
            return [PSCustomObject]@{
                date_frmt       = NormalizeDateFormat "MMddyyyy" 
                file_pattern    = 'another_FileName_(\d{8})\.txt'
            }
        }
        "Aetna"  {
            return [PSCustomObject]@{
                date_frmt       = NormalizeDateFormat "MMddyyyy"
                file_pattern    = "AnotherFileName(\d{8})\.txt"
            }
        }
        "Test"  {
            return [PSCustomObject]@{
                date_frmt       = NormalizeDateFormat "YYYYMMDD"
                file_pattern    = "834_HIST_(\d{8})\.edi"
            }
        }
    }
}

function FileDateFilter {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$FolderPath,

        [Parameter(Mandatory = $true)]
        [string]$FilePattern,

        [Parameter(Mandatory = $true)]
        [datetime]$StartDate, 

        [Parameter(Mandatory = $true)]
        [datetime]$EndDate,

        [Parameter(Mandatory = $true)]
        [string]$DateFormat # Needed to know how to parse the filename date
    )

    # Filter files using regex against the filename
    $files = Get-ChildItem -Path $FolderPath -File | Where-Object {
        if ($_.Name -match $FilePattern) {
            $dateString = $Matches[1]
            $fileDate = [datetime]::ParseExact($dateString, $DateFormat, $null)

            if ($fileDate -lt $StartDate) { return $false }
            if ($fileDate -gt $EndDate)   { return $false }
            
            return $true
        }
        return $false 
    } | Select-Object -ExpandProperty FullName

    if (-not $files) {
        Write-Warning "No files matched the specified criteria or date range."
        return
    }

    return $files
}

function FetchCarrierFile {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter(Mandatory = $true)]
        [string]$Carrier,

        [Parameter(Mandatory = $false)]
        [string]$StartDate,

        [Parameter(Mandatory = $false)]
        [string]$EndDate
    )


    $carrierAttr = CarrierFileAttributes $Carrier
    if (-not $carrierAttr) {
        Write-Error "Unknown carrier: $Carrier"
        return
    }

    $dateFormat = $carrierAttr.date_frmt
    $filePattern = $carrierAttr.file_pattern


    $allowedFormats = @($dateFormat, "MM/dd/yyyy", "yyyy-MM-dd", "MM-dd-yyyy")
    
    # Use InvariantCulture so separators like '/' are evaluated strictly
    $culture = [System.Globalization.CultureInfo]::InvariantCulture
    $styles  = [System.Globalization.DateTimeStyles]::None


    $ParseDateString = {
        param([string]$dateStr, [string]$paramName)
        
        foreach ($fmt in $allowedFormats) {
            $parsedDate = [datetime]::MinValue
            if ([datetime]::TryParseExact($dateStr, $fmt, $culture, $styles, [ref]$parsedDate)) {
                return $parsedDate
            }
        }
        throw "Could not parse '$dateStr' for -$paramName. Please use a valid format (e.g., yyyyMMdd or MM/dd/yyyy)."
    }


    if ([string]::IsNullOrWhiteSpace($StartDate)) {
        $ParsedStartDate = Get-Date -Year (Get-Date).Year -Month 1 -Day 1 -Hour 0 -Minute 0 -Second 0
    } else {
        $ParsedStartDate = & $ParseDateString $StartDate "StartDate"
    }
    
    if ([string]::IsNullOrWhiteSpace($EndDate)) {
        $ParsedEndDate = Get-Date
    } else {
        $ParsedEndDate = & $ParseDateString $EndDate "EndDate"
    }


    FileDateFilter -FolderPath $Path `
                   -FilePattern $filePattern `
                   -StartDate $ParsedStartDate `
                   -EndDate $ParsedEndDate `
                   -DateFormat $dateFormat
}