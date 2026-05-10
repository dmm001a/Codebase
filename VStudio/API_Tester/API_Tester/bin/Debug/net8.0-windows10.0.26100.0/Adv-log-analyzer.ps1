# Enhanced Advantage Log Analyzer for Windows PowerShell
# Compatible with PowerShell 5.1
# Usage: powershell -ExecutionPolicy Bypass .\log-analyzer.ps1
# Or: .\log-analyzer.ps1
# Latest-Version: July 18 2025

param()

# Global variables
$global:CurrentLogPath = ""
$global:SelectedEnvironment = ""
$global:LogFiles = @()
$global:LastTimePattern = ""
$global:LastTimeDescription = ""
$global:PageSize = 20

# Environment configurations
$global:Environments = @{
    "Dev" = @{
        Name = "Development"
        Path = "\\AUSE2DADVAN02.gsdwkglobal.com\Upgrade\pub\work\services\advapi\apiworker"
    }
    "QA" = @{
        Name = "QA"
        Path = "\\AUSE1SADVAN01.gsdwkglobal.com\QA\Pub\work\services\advapi\apiworker"
    }
    "Stage" = @{
        Name = "Stage"
        Path = "\\AUSE1SADVAN01.gsdwkglobal.com\Stage2020\Pub\work\services\advapi\apiworker"
    }
    "Production" = @{
        Name = "Production"
        Path = "\\AUSE1PADVAN01.gsdwkglobal.com\Production\Pub\work\services\advapi\apiworker"
    }
}

# Color functions for better output
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

# Progress indicator function
function Show-Progress {
    param(
        [int]$Current,
        [int]$Total,
        [string]$Activity = "Processing"
    )
    
    if ($Total -eq 0) { return }
    
    $percentComplete = [Math]::Round(($Current / $Total) * 100, 0)
    $progressBar = "[" + ("=" * [Math]::Floor($percentComplete / 5)) + (" " * (20 - [Math]::Floor($percentComplete / 5))) + "]"
    
    Write-Host "`r$Activity... $progressBar $percentComplete% ($Current of $Total files)" -NoNewline -ForegroundColor Cyan
}

# Environment selection function
function Select-Environment {
    Clear-Host
    Write-ColorOutput "=== Advantage Web Log Analyzer ===" "Blue"
    Write-ColorOutput "Environment Selection" "Yellow"
    Write-Host "===================="
    Write-Host ""
    
    $envKeys = @("Dev", "QA", "Stage", "Production")
    
    do {
        for ($i = 0; $i -lt $envKeys.Count; $i++) {
            $key = $envKeys[$i]
            Write-Host "$($i + 1). $($global:Environments[$key].Name)"
        }
        Write-Host "0. Exit"
        Write-Host ""
        
        $choice = Read-Host "Select environment (1-4)"
        
        switch ($choice) {
            "1" { $selectedEnv = "Dev"; break }
            "2" { $selectedEnv = "QA"; break }
            "3" { $selectedEnv = "Stage"; break }
            "4" { $selectedEnv = "Production"; break }
            "0" { return $false }
            default {
                Write-ColorOutput "Invalid option. Please choose 1-4 or 0 to exit." "Red"
                continue
            }
        }
        
        $global:SelectedEnvironment = $selectedEnv
        $global:CurrentLogPath = $global:Environments[$selectedEnv].Path
        
        Write-Host ""
        Write-ColorOutput "Query will be run against <<$($global:Environments[$selectedEnv].Name)>>" "Green"
        Write-Host "Path: $global:CurrentLogPath" -ForegroundColor Gray
        
        # Validate environment path accessibility
        Write-Host ""
        Write-Host "Validating environment access..." -ForegroundColor Yellow
        
        if (Test-Path $global:CurrentLogPath -ErrorAction SilentlyContinue) {
            Write-ColorOutput "Environment path validated successfully." "Green"
            Start-Sleep -Seconds 1
            return $true
        } else {
            Write-ColorOutput "Warning: Cannot access environment path." "Red"
            Write-ColorOutput "This may be due to network issues or permissions." "Yellow"
            $retry = Read-Host "Try another environment? (y/n)"
            if ($retry -notmatch "^[Yy]") {
                return $false
            }
        }
        
    } while ($true)
}

# Enhanced timeframe selection with custom date validation
function Get-TimeFrameSelection {
    do {
        Write-Host ""
        Write-Host "Time Period Selection:" -ForegroundColor Cyan
        Write-Host "1. Last hour"
        Write-Host "2. Last 24 hours" 
        Write-Host "3. Last 7 days"
        Write-Host "4. Custom date range"
        if ($global:LastTimePattern) {
            Write-Host "5. Use previous: $global:LastTimeDescription" -ForegroundColor Green
        }
        Write-Host "0. Cancel"
        
        $choice = Read-Host "Choose option"
        
        $timePattern = ""
        $timeDescription = ""
        $startDate = $null
        $endDate = $null
        
        switch ($choice) {
            "1" { 
                $startDate = (Get-Date).AddHours(-1)
                $endDate = Get-Date
                $timeDescription = "Last Hour"
            }
            "2" { 
                $startDate = (Get-Date).AddDays(-1)
                $endDate = Get-Date
                $timeDescription = "Last 24 Hours"
            }
            "3" {
                $startDate = (Get-Date).AddDays(-7)
                $endDate = Get-Date
                $timeDescription = "Last 7 Days"
            }
            "4" { 
                # Custom date range implementation
                Write-Host ""
                $startDateStr = Read-Host "Enter start date (yyyy-MM-dd)"
                
                try {
                    $startDate = [DateTime]::ParseExact($startDateStr, "yyyy-MM-dd", $null)
                    
                    # Validate not more than 1 month in the past
                    $maxPastDate = (Get-Date).AddMonths(-1)
                    if ($startDate -lt $maxPastDate) {
                        Write-ColorOutput "Start date cannot be more than 1 month in the past." "Red"
                        continue
                    }
                } catch {
                    Write-ColorOutput "Invalid date format. Please use yyyy-MM-dd format." "Red"
                    continue
                }
                
                $endDateStr = Read-Host "Enter end date (yyyy-MM-dd)"
                
                try {
                    $endDate = [DateTime]::ParseExact($endDateStr, "yyyy-MM-dd", $null).AddDays(1).AddSeconds(-1)
                    
                    # Validate end date is after start date
                    if ($endDate -le $startDate) {
                        Write-ColorOutput "End date must be after start date." "Red"
                        continue
                    }
                    
                    # Validate not in the future
                    if ($endDate -gt (Get-Date)) {
                        $endDate = Get-Date
                    }
                } catch {
                    Write-ColorOutput "Invalid date format. Please use yyyy-MM-dd format." "Red"
                    continue
                }
                
                $timeDescription = "Custom: $startDateStr to $endDateStr"
            }
            "5" {
                if ($global:LastTimePattern) {
                    return @{
                        Pattern = $global:LastTimePattern
                        Description = $global:LastTimeDescription
                        StartDate = $global:LastStartDate
                        EndDate = $global:LastEndDate
                    }
                }
            }
            "0" {
                return $null
            }
            default {
                Write-ColorOutput "Invalid option." "Red"
                continue
            }
        }
        
        if ($startDate -and $endDate) {
            # Generate pattern for date range
            $timePattern = $startDate.ToString("yyyy-MM-dd")
            
            # Store for future use
            $global:LastTimePattern = $timePattern
            $global:LastTimeDescription = $timeDescription
            $global:LastStartDate = $startDate
            $global:LastEndDate = $endDate
            
            # Auto-execute without confirmation
            Write-Host ""
            Write-ColorOutput "Starting search for: $timeDescription" "Yellow"
            
            return @{
                Pattern = $timePattern
                Description = $timeDescription
                StartDate = $startDate
                EndDate = $endDate
            }
        }
    } while ($true)
}

# Load log files with progress
function Load-LogFiles {
    param(
        [hashtable]$Timeframe = $null
    )
    
    try {
        Write-Host "Scanning for log files..." -ForegroundColor Yellow
        $files = @(Get-ChildItem -Path $global:CurrentLogPath -Filter "*.log" -File -ErrorAction Stop)
        
        if ($files.Count -eq 0) {
            Write-ColorOutput "No .log files found in $global:CurrentLogPath" "Red"
            return $false
        }
        
        if ($Timeframe) {
            Write-Host "Filtering files by timeframe: $($Timeframe.Description)" -ForegroundColor Green
            $files = $files | Where-Object { $_.LastWriteTime -ge $Timeframe.StartDate -and $_.LastWriteTime -le $Timeframe.EndDate }
            Write-ColorOutput "Filtered to $($files.Count) files within the timeframe." "Green"
        }
        
        $global:LogFiles = $files
        Write-ColorOutput "Loaded $($global:LogFiles.Count) log files" "Green"
        return $true
    }
    catch {
        Write-ColorOutput "Cannot access log directory: $global:CurrentLogPath" "Red"
        Write-ColorOutput "Error: $($_.Exception.Message)" "Red"
        return $false
    }
}

# Function to get a limited set of log files based on user input
function Get-LimitedLogFiles {
    param(
        [array]$FilesToLimit
    )

    $limitedFiles = $FilesToLimit

    if ($FilesToLimit.Count -gt 100) {
        Write-ColorOutput "Found $($FilesToLimit.Count) log files. Large number of files may affect performance." "Yellow"
        $limit = Read-Host "Limit to most recent N files for this operation? (Enter number or press Enter for all)"
        if ($limit -match '^\d+$') {
            $limitedFiles = $FilesToLimit | Sort-Object LastWriteTime -Descending | Select-Object -First ([int]$limit)
            Write-ColorOutput "Limited to $($limitedFiles.Count) most recent files for this operation." "Green"
        }
    }
    return $limitedFiles
}

# Enhanced menu with breadcrumb
function Show-Menu {
    Clear-Host
    Write-ColorOutput "=== Advantage Log Analyzer ===" "Blue"
    Write-Host "Environment: $($global:Environments[$global:SelectedEnvironment].Name)" -ForegroundColor Green
    Write-Host "Log Path: $global:CurrentLogPath"
    Write-Host "Files: $($global:LogFiles.Count) log files"
    Write-Host ""
    Write-ColorOutput "Main Menu" "Cyan"
    Write-ColorOutput "=========" "Cyan"
    Write-ColorOutput "1. Analyze Time Period (Recommended)" "Green"
    Write-Host "2. Quick Error Summary"
    Write-Host "3. Search for Timeouts"
    Write-Host "4. Search for Specific Term"
    Write-Host "5. Show Largest Log Files"
    Write-Host "6. Real-time Monitor (latest file)"
    Write-Host "7. Generate Full Report"
    Write-Host "8. Change Environment"
    Write-Host "9. Exit"
    Write-Host ""
    Write-Host "H. Help" -ForegroundColor Gray
    Write-Host "========================"
}

# Pagination function
function Show-PaginatedResults {
    param(
        [array]$Results,
        [string]$Title,
        [scriptblock]$DisplayBlock,
        [string]$EmptyMessage = "No results found."
    )
    
    if ($Results.Count -eq 0) {
        Write-Host ""
        Write-ColorOutput $EmptyMessage "Yellow"
        Write-ColorOutput "Search completed. Files searched: $($global:LogFiles.Count)" "Gray"
        return
    }
    
    Write-Host ""
    Write-ColorOutput "$Title - Total Results: $($Results.Count)" "Yellow"
    Write-Host "=" * 50
    
    $currentPage = 1
    $totalPages = [Math]::Ceiling($Results.Count / $global:PageSize)
    
    do {
        # Calculate page bounds
        $startIndex = ($currentPage - 1) * $global:PageSize
        $endIndex = [Math]::Min($startIndex + $global:PageSize - 1, $Results.Count - 1)
        
        # Display current page
        Write-Host ""
        Write-ColorOutput "Page $currentPage of $totalPages (Results $($startIndex + 1)-$($endIndex + 1) of $($Results.Count))" "Cyan"
        Write-Host "-" * 50
        
        for ($i = $startIndex; $i -le $endIndex; $i++) {
            & $DisplayBlock $Results[$i] ($i + 1)
        }
        
        # Navigation menu
        Write-Host ""
        Write-Host "Navigation: " -NoNewline -ForegroundColor Yellow
        Write-Host "[N]ext [P]revious [F]irst [L]ast [G]oto [A]ll [R]ead file [M]ain menu"
        
        $nav = Read-Host "Choose action"
        
        switch ($nav.ToUpper()) {
            "N" { 
                if ($currentPage -lt $totalPages) {
                    $currentPage++
                } else {
                    Write-ColorOutput "Already on last page" "Red"
                }
            }
            "P" { 
                if ($currentPage -gt 1) {
                    $currentPage--
                } else {
                    Write-ColorOutput "Already on first page" "Red"
                }
            }
            "F" { $currentPage = 1 }
            "L" { $currentPage = $totalPages }
            "G" {
                $goto = Read-Host "Enter page number (1-$totalPages)"
                if ($goto -match '^\d+$') {
                    $pageNum = [int]$goto
                    if ($pageNum -ge 1 -and $pageNum -le $totalPages) {
                        $currentPage = $pageNum
                    } else {
                        Write-ColorOutput "Invalid page number" "Red"
                    }
                }
            }
            "A" {
                if ($Results.Count -gt 100) {
                    Write-ColorOutput "Warning: Displaying $($Results.Count) results may be slow." "Yellow"
                    $confirm = Read-Host "Continue? (y/n)"
                    if ($confirm -notmatch "^[Yy]") {
                        continue
                    }
                }
                Write-Host ""
                for ($i = 0; $i -lt $Results.Count; $i++) {
                    & $DisplayBlock $Results[$i] ($i + 1)
                }
                Read-Host "Press Enter to return to pagination"
            }
            "R" {
                $readSpecificFileResult = Read-SpecificFile -Results $Results
                if ($readSpecificFileResult -eq "MainMenu") {
                    return # Exit Show-PaginatedResults, which will lead back to Main menu
                }
            }
            "M" { return }
            default {
                Write-ColorOutput "Invalid option" "Red"
            }
        }
        
    } while ($true)
}

# File reading feature
function Read-SpecificFile {
    param(
        [array]$Results
    )
    
    $filePattern = Read-Host "Enter filename to read (or partial name)"
    $filePattern = $filePattern.Trim() # Trim whitespace from user input
    
    if ([string]::IsNullOrWhiteSpace($filePattern)) {
        Write-ColorOutput "No filename provided" "Red"
        return
    }
    
    # Find matching files from $global:LogFiles (all loaded files for the environment/timeframe)
    # $global:LogFiles contains FileInfo objects, so we check against their Name property.
    $matchingFileObjects = $global:LogFiles | Where-Object { $_.Name -like "*$filePattern*" }
    
    if ($matchingFileObjects.Count -eq 0) {
        Write-ColorOutput "No files matching '$filePattern' found among loaded log files for the current environment/timeframe." "Red"
        Write-ColorOutput "Path searched: $global:CurrentLogPath" "Yellow"
        Write-ColorOutput "Ensure the file exists, matches the timeframe, and the pattern is correct." "Yellow"
        return
    }
    
    $uniqueFileNames = $matchingFileObjects | Select-Object -ExpandProperty Name -Unique
    
    # Ensure $uniqueFileNames is an array for .Count and indexing, even if only one item.
    if ($uniqueFileNames -is [string]) { $uniqueFileNames = @($uniqueFileNames) }

    if ($uniqueFileNames.Count -gt 1) {
        Write-Host "Multiple files found:" -ForegroundColor Yellow
        for ($i = 0; $i -lt $uniqueFileNames.Count; $i++) {
            # Display full path for clarity if multiple matches
            Write-Host "$($i + 1). $(Join-Path $global:CurrentLogPath $uniqueFileNames[$i])"
        }
        $selection = Read-Host "Select file number"
        if ($selection -match '^\d+$') {
            $index = [int]$selection - 1
            if ($index -ge 0 -and $index -lt $uniqueFileNames.Count) {
                $selectedFile = $uniqueFileNames[$index] # This is just the filename
            } else {
                Write-ColorOutput "Invalid selection" "Red"
                return
            }
        } else {
            return # User cancelled or entered non-numeric
        }
    } else {
        $selectedFile = $uniqueFileNames[0] # This is just the filename
    }
    
    # Read file options
    Write-Host ""
    Write-Host "File: $(Join-Path $global:CurrentLogPath $selectedFile)" -ForegroundColor Green
    Write-Host "1. View full file"
    Write-Host "2. View last N lines"
    Write-Host "3. Search within file"
    Write-Host "4. View line range"
    Write-Host "0. Cancel"
    
    $viewChoice = Read-Host "Choose option"
    
    $filePath = Join-Path $global:CurrentLogPath $selectedFile
    
    switch ($viewChoice) {
        "1" {
            $content = Get-Content $filePath -ErrorAction SilentlyContinue
            if ($content) {
                $content | Out-Host -Paging
                Read-Host "Press Enter to continue..."
            } else {
                Write-ColorOutput "File is empty or could not be read." "Red"
            }
        }
        "2" {
            $lines = Read-Host "Enter number of lines"
            if ($lines -match '^\d+$') {
                Get-Content $filePath -Tail ([int]$lines) -ErrorAction SilentlyContinue | Out-Host -Paging
                Read-Host "Press Enter to continue..."
            } else {
                Write-ColorOutput "Invalid number of lines." "Red"
            }
        }
        "3" {
            $searchTerm = Read-Host "Enter search term"
            if (-not [string]::IsNullOrWhiteSpace($searchTerm)) {
                Select-String -Path $filePath -Pattern $searchTerm -Context 2,2 | Out-Host -Paging
                Read-Host "Press Enter to continue..."
            }
        }
        "4" {
            $startLine = Read-Host "Enter start line number"
            $endLine = Read-Host "Enter end line number"
            if ($startLine -match '^\d+$' -and $endLine -match '^\d+$') {
                $content = Get-Content $filePath -ErrorAction SilentlyContinue
                if ($content) {
                    $start = [int]$startLine - 1
                    $end = [int]$endLine
                    $content[$start..$end] | Out-Host -Paging
                    Read-Host "Press Enter to continue..."
                } else {
                    Write-ColorOutput "File is empty or could not be read." "Red"
                }
            } else {
                Write-ColorOutput "Invalid line numbers." "Red"
            }
        }
        "0" {
            # Cancelled
            return $null # Explicitly return to avoid the next prompt
        }
        default {
            Write-ColorOutput "Invalid view option." "Red"
            return $null # Explicitly return to avoid the next prompt
        }
    }

    # This prompt should only appear if a view action was taken and not cancelled
    if ($viewChoice -in @("1","2","3","4")) {
        Write-Host ""
        $nextAction = Read-Host "Press [Enter] to return to results, or [M] for Main Menu"
    } else {
        # If cancelled or invalid option, $nextAction won't be set, so ensure it's $null
        $nextAction = $null
    }
    
    if ($nextAction.ToUpper() -eq "M") {
        return "MainMenu" # Signal to Show-PaginatedResults to exit to Main Menu
    }
    return $null # Default return, continue pagination
}

# Enhanced Quick Summary with progress
function Get-QuickSummary {
    Write-ColorOutput "Generating detailed error summary for: $($global:LastTimeDescription)" "Yellow"
    
    $errorPatterns = @(
        "error",
        "warn",
        "fail",
        "timeout",
        "-- Exception Details --",
        "Advantage\.Framework\.AdvantageException",
        "Advantage\.Framework\.FieldValidationException",
        "\b\w*Exception\b", # General pattern for any word ending in Exception
        "ErrorCode:",
        "Can't commit, nested transaction was rolled back",
        "is not within the valid date range for the promotion",
        "Invalid Donor Type",
        "operation took \d+\.\d+s"
    )
    $errorCounts = @{}
    $detailedErrors = @()
    $fileCount = 0
    
    foreach ($file in $global:LogFiles) {
        $fileCount++
        Show-Progress -Current $fileCount -Total $global:LogFiles.Count -Activity "Analyzing errors"
        
        $content = Get-Content $file.FullName -ErrorAction SilentlyContinue
        if ($content) {
            $lineNumber = 0
            foreach ($line in $content) {
                $lineNumber++
                
                # Check if line is within date range
                $lineDate = $null
                if ($line -match '(\d{4}-\d{2}-\d{2}[\s\t]\d{2}:\d{2}:\d{2})') {
                    try {
                        $lineDate = [DateTime]::Parse($matches[1])
                    } catch { }
                }
                
                if ($lineDate -and $lineDate -ge $global:LastStartDate -and $lineDate -le $global:LastEndDate) {
                    foreach ($pattern in $errorPatterns) {
                        if ($line -match $pattern) {
                            if ($errorCounts.ContainsKey($pattern)) {
                                $errorCounts[$pattern]++
                            } else {
                                $errorCounts[$pattern] = 1
                            }
                            
                            $detailedErrors += [PSCustomObject]@{
                                File = $file.Name
                                Line = $lineNumber
                                Pattern = $pattern
                                Timestamp = $matches[1]
                                Content = $line.Trim()
                            }
                            break
                        }
                    }
                }
            }
        }
    }
    
    Write-Host "`r" + (" " * 80) + "`r" -NoNewline  # Clear progress line
    
    if ($detailedErrors.Count -eq 0) {
        Write-Host ""
        Write-ColorOutput "Search completed. No errors detected in the specified timeframe." "Green"
        Write-ColorOutput "Files searched: $($global:LogFiles.Count)" "Gray"
        Write-ColorOutput "Time taken: $(New-TimeSpan -Start $global:LastStartDate -End $global:LastEndDate)" "Gray"
        return
    }
    
    # Display summary
    Write-Host ""
    Write-Host "Error Pattern Counts:" -ForegroundColor Cyan
    Write-Host "--------------------"
    $errorCounts.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object {
        Write-Host ("{0,8}: {1}" -f $_.Value, $_.Key.ToUpper()) -ForegroundColor Yellow
    }
    
    # Show paginated results
    $displayBlock = {
        param($item, $index)
        Write-Host "[$($item.Pattern.ToUpper())] " -NoNewline -ForegroundColor Red
        Write-Host "$($item.File):$($item.Line) " -NoNewline -ForegroundColor White
        if ($item.Timestamp) {
            Write-Host "[$($item.Timestamp)]" -ForegroundColor Green
        }
        Write-Host "  $($item.Content)" -ForegroundColor Gray
        Write-Host ""
    }
    
    Show-PaginatedResults -Results $detailedErrors -Title "Error Details" -DisplayBlock $displayBlock
}

# Enhanced Search Timeouts with progress
function Search-Timeouts {
    Write-ColorOutput "Searching for timeout issues in: $($global:LastTimeDescription)" "Yellow"
    
    $timeoutPatterns = @(
        @{Pattern = "timeout"; Description = "General Timeout"},
        @{Pattern = "timed out"; Description = "Timed Out"},
        @{Pattern = "connection.*timeout"; Description = "Connection Timeout"},
        @{Pattern = "request.*timeout"; Description = "Request Timeout"},
        @{Pattern = "read.*timeout"; Description = "Read Timeout"},
        @{Pattern = "socket.*timeout"; Description = "Socket Timeout"}
    )
    
    $allTimeouts = @()
    $fileCount = 0
    
    foreach ($file in $global:LogFiles) {
        $fileCount++
        Show-Progress -Current $fileCount -Total $global:LogFiles.Count -Activity "Searching for timeouts"
        
        $content = Get-Content $file.FullName -ErrorAction SilentlyContinue
        if ($content) {
            $lineNumber = 0
            foreach ($line in $content) {
                $lineNumber++
                
                # Check if line is within date range
                $lineDate = $null
                if ($line -match '(\d{4}-\d{2}-\d{2}[\s\t]\d{2}:\d{2}:\d{2})') {
                    try {
                        $lineDate = [DateTime]::Parse($matches[1])
                        $timestamp = $matches[1]
                    } catch { }
                }
                
                if ($lineDate -and $lineDate -ge $global:LastStartDate -and $lineDate -le $global:LastEndDate) {
                    foreach ($patternObj in $timeoutPatterns) {
                        if ($line -match $patternObj.Pattern) {
                            # Extract severity
                            $severity = "INFO"
                            if ($line -match '\b(ERROR|WARN|FATAL|DEBUG|INFO)\b') {
                                $severity = $matches[1]
                            }
                            
                            $allTimeouts += [PSCustomObject]@{
                                File = $file.Name
                                Line = $lineNumber
                                Type = $patternObj.Description
                                Severity = $severity
                                Timestamp = $timestamp
                                Content = $line.Trim()
                            }
                            break
                        }
                    }
                }
            }
        }
    }
    
    Write-Host "`r" + (" " * 80) + "`r" -NoNewline  # Clear progress line
    
    if ($allTimeouts.Count -eq 0) {
        Write-Host ""
        Write-ColorOutput "Search completed. No timeout issues found in the selected period." "Green"
        Write-ColorOutput "Files searched: $($global:LogFiles.Count)" "Gray"
        Write-ColorOutput "Search parameters: $($global:LastTimeDescription)" "Gray"
        return
    }
    
    # Show summary
    Write-Host ""
    Write-Host "Timeout Summary by Type:" -ForegroundColor Cyan
    Write-Host "-----------------------"
    $timeoutSummary = $allTimeouts | Group-Object Type | Sort-Object Count -Descending
    foreach ($group in $timeoutSummary) {
        Write-Host ("{0,3} occurrences: {1}" -f $group.Count, $group.Name) -ForegroundColor Yellow
    }
    
    # Show paginated results
    $displayBlock = {
        param($item, $index)
        $severityColor = switch ($item.Severity) {
            "ERROR" { "Red" }
            "WARN" { "Yellow" }
            "FATAL" { "Magenta" }
            default { "White" }
        }
        
        Write-Host "[$($item.Type)] " -NoNewline -ForegroundColor Cyan
        Write-Host "[$($item.Severity)] " -NoNewline -ForegroundColor $severityColor
        Write-Host "$($item.File):$($item.Line) " -NoNewline -ForegroundColor White
        if ($item.Timestamp) {
            Write-Host "[$($item.Timestamp)]" -ForegroundColor Green
        }
        Write-Host "  $($item.Content)" -ForegroundColor Gray
        Write-Host ""
    }
    
    Show-PaginatedResults -Results $allTimeouts -Title "Timeout Details" -DisplayBlock $displayBlock
}

# Enhanced Custom Search with progress
function Search-Custom {
    $searchTerm = Read-Host "Enter search term (supports regex)"
    if ([string]::IsNullOrEmpty($searchTerm)) {
        Write-ColorOutput "No search term provided" "Red"
        return
    }
    
    Write-ColorOutput "Searching for: '$searchTerm' in files modified within timeframe: $($global:LastTimeDescription)" "Yellow"
    
    $results = @()
    $fileCount = 0
    
    foreach ($file in $global:LogFiles) {
        $fileCount++
        Show-Progress -Current $fileCount -Total $global:LogFiles.Count -Activity "Searching"
        
        $content = Get-Content $file.FullName -ErrorAction SilentlyContinue
        if ($content) {
            $lineNumber = 0
            foreach ($line in $content) {
                $lineNumber++
                
                # Search all lines in the already filtered files.
                # The $global:LogFiles are filtered by LastWriteTime in Load-LogFiles.
                if ($line -match $searchTerm) {
                    # Try to get a timestamp if available for display, but don't filter by it.
                    $timestamp = "" # Default to empty timestamp
                    if ($line -match '(\d{4}-\d{2}-\d{2}[\s\t]\d{2}:\d{2}:\d{2})') {
                        # $matches[1] will be the timestamp string if found
                        $timestamp = $matches[1] 
                    }

                    $results += [PSCustomObject]@{
                        File = $file.Name
                        Line = $lineNumber
                        Timestamp = $timestamp # Include if found, otherwise empty
                        Content = $line.Trim()
                    }
                }
            }
        }
    }
    
    Write-Host "`r" + (" " * 80) + "`r" -NoNewline  # Clear progress line
    
    if ($results.Count -eq 0) {
        Write-Host ""
        Write-ColorOutput "Search completed. No matching entries found for '$searchTerm'." "Yellow"
        Write-ColorOutput "Files searched (modified within $global:LastTimeDescription): $($global:LogFiles.Count)" "Gray"
        return
    }
    
    # Show paginated results
    $displayBlock = {
        param($item, $index)
        Write-Host "$($item.File):$($item.Line) " -NoNewline -ForegroundColor Cyan
        if ($item.Timestamp) {
            Write-Host "[$($item.Timestamp)]" -ForegroundColor Green
        } else {
            Write-Host ""
        }
        Write-Host "  $($item.Content)" -ForegroundColor Gray
        Write-Host ""
    }
    
    Show-PaginatedResults -Results $results -Title "Search Results for '$searchTerm'" -DisplayBlock $displayBlock
}

# Help function
function Show-Help {
    Clear-Host
    Write-ColorOutput "=== Advantage Log Analyzer Help ===" "Blue"
    Write-Host ""
    Write-Host "Navigation Tips:" -ForegroundColor Cyan
    Write-Host "- All menus support returning to previous menu or main menu"
    Write-Host "- Pagination allows viewing large result sets efficiently"
    Write-Host "- Press Ctrl+C to cancel long-running operations"
    Write-Host ""
    Write-Host "Date Format Examples:" -ForegroundColor Cyan
    Write-Host "- Custom date: 2025-01-15"
    Write-Host "- Date range maximum: 1 month in the past"
    Write-Host ""
    Write-Host "Search Tips:" -ForegroundColor Cyan
    Write-Host "- Search terms support regular expressions"
    Write-Host "- Timeframe filters apply to all search operations"
    Write-Host "- Use quotes for exact phrase matching"
    Write-Host ""
    Write-Host "Performance Tips:" -ForegroundColor Cyan
    Write-Host "- Limit file count for large directories"
    Write-Host "- Use specific timeframes to reduce search scope"
    Write-Host "- Export large result sets instead of viewing all"
    Write-Host ""
    Read-Host "Press Enter to continue"
}

# Enhanced Analyze TimeFrame function
function Analyze-TimeFrame {
    Write-ColorOutput "Analyzing logs for timeframe: $($global:LastTimeDescription)" "Yellow"
    
    $errorPatterns = @(
        "error",
        "warn",
        "fail",
        "timeout",
        "-- Exception Details --",
        "Advantage\.Framework\.AdvantageException",
        "Advantage\.Framework\.FieldValidationException",
        "\b\w*Exception\b",
        "ErrorCode:",
        "Can't commit, nested transaction was rolled back",
        "is not within the valid date range for the promotion",
        "Invalid Donor Type",
        "operation took \d+\.\d+s"
    )
    
    $timeFrameResults = @()
    $fileCount = 0
    
    foreach ($file in $global:LogFiles) {
        $fileCount++
        Show-Progress -Current $fileCount -Total $global:LogFiles.Count -Activity "Analyzing timeframe"
        
        $content = Get-Content $file.FullName -ErrorAction SilentlyContinue
        if ($content) {
            $lineNumber = 0
            foreach ($line in $content) {
                $lineNumber++
                
                # Check if line is within date range
                $lineDate = $null
                $timestamp = ""
                if ($line -match '(\d{4}-\d{2}-\d{2}[\s\t]\d{2}:\d{2}:\d{2})') {
                    try {
                        $lineDate = [DateTime]::Parse($matches[1])
                        $timestamp = $matches[1]
                    } catch { }
                }
                
                if ($lineDate -and $lineDate -ge $global:LastStartDate -and $lineDate -le $global:LastEndDate) {
                    foreach ($pattern in $errorPatterns) {
                        if ($line -match $pattern) {
                            $timeFrameResults += [PSCustomObject]@{
                                File = $file.Name
                                Line = $lineNumber
                                Timestamp = $timestamp
                                Content = $line.Trim()
                                MatchedPattern = $pattern 
                            }
                            break 
                        }
                    }
                }
            }
        }
    }
    
    Write-Host "`r" + (" " * 80) + "`r" -NoNewline  # Clear progress line
    
    if ($timeFrameResults.Count -eq 0) {
        Write-Host ""
        Write-ColorOutput "No errors found for the specified timeframe." "Green"
        Write-ColorOutput "Timeframe: $($global:LastTimeDescription)" "Gray"
        Write-ColorOutput "Files searched: $($global:LogFiles.Count)" "Gray"
        return
    }
    
    # Show paginated results
    $displayBlock = {
        param($item, $index)
        Write-Host "$($item.File):$($item.Line) " -NoNewline -ForegroundColor Cyan
        if ($item.Timestamp) {
            Write-Host "[$($item.Timestamp)]" -ForegroundColor Green
        } else {
            Write-Host ""
        }
        Write-Host "  $($item.Content)" -ForegroundColor Gray
        Write-Host ""
    }
    
    Show-PaginatedResults -Results $timeFrameResults -Title "Timeframe Analysis Results" -DisplayBlock $displayBlock
}

# Enhanced Show File Sizes with better formatting
function Show-FileSizes {
    Write-ColorOutput "Analyzing log files..." "Yellow"
    
    $fileInfo = @()
    $fileCount = 0
    
    foreach ($file in $global:LogFiles) {
        $fileCount++
        Show-Progress -Current $fileCount -Total $global:LogFiles.Count -Activity "Analyzing file sizes"
        
        $lineCount = 0
        if ($file.Length -lt 10MB) {
            $lineCount = (Get-Content $file.FullName -ErrorAction SilentlyContinue | Measure-Object -Line).Lines
        } else {
            # For large files, estimate line count
            $sample = Get-Content $file.FullName -TotalCount 1000 -ErrorAction SilentlyContinue
            if ($sample) {
                $avgLineLength = ($sample | Measure-Object -Property Length -Average).Average
                $lineCount = [Math]::Round($file.Length / $avgLineLength)
            }
        }
        
        $fileInfo += [PSCustomObject]@{
            Name = $file.Name
            SizeBytes = $file.Length
            Size = if ($file.Length -gt 1GB) {
                "{0:N2} GB" -f ($file.Length / 1GB)
            } elseif ($file.Length -gt 1MB) {
                "{0:N2} MB" -f ($file.Length / 1MB)
            } elseif ($file.Length -gt 1KB) {
                "{0:N2} KB" -f ($file.Length / 1KB)
            } else {
                "{0:N0} B" -f $file.Length
            }
            Lines = "{0:N0}" -f $lineCount
            LastModified = $file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
        }
    }
    
    Write-Host "`r" + (" " * 80) + "`r" -NoNewline  # Clear progress line
    
    Write-Host ""
    Write-ColorOutput "Log File Information (Sorted by Size):" "Yellow"
    Write-Host "======================================"
    
    $sortedFiles = $fileInfo | Sort-Object SizeBytes -Descending
    
    # Show top 10 largest
    Write-Host ""
    Write-ColorOutput "Top 10 Largest Files:" "Cyan"
    $sortedFiles | Select-Object -First 10 | Format-Table Name, Size, Lines, LastModified -AutoSize
    
    # Summary statistics
    Write-Host ""
    Write-ColorOutput "Summary Statistics:" "Cyan"
    Write-Host "Total Files: $($fileInfo.Count)"
    Write-Host "Total Size: $(if (($fileInfo.SizeBytes | Measure-Object -Sum).Sum -gt 1GB) {
        '{0:N2} GB' -f (($fileInfo.SizeBytes | Measure-Object -Sum).Sum / 1GB)
    } else {
        '{0:N2} MB' -f (($fileInfo.SizeBytes | Measure-Object -Sum).Sum / 1MB)
    })"
    Write-Host "Average File Size: $('{0:N2} MB' -f (($fileInfo.SizeBytes | Measure-Object -Average).Average / 1MB))"
    Write-Host "Largest File: $($sortedFiles[0].Name) ($($sortedFiles[0].Size))"
    
    # Warning for large files
    $largeFiles = $sortedFiles | Where-Object { $_.SizeBytes -gt 100MB }
    if ($largeFiles) {
        Write-Host ""
        Write-ColorOutput "Warning: $($largeFiles.Count) files larger than 100MB may impact search performance" "Yellow"
    }
}

# Enhanced Monitor Real-Time
function Monitor-RealTime {
    $latestFile = $global:LogFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    
    if (-not $latestFile) {
        Write-ColorOutput "No log files found for monitoring" "Red"
        return
    }
    
    Write-ColorOutput "Monitoring: $($latestFile.Name)" "Yellow"
    Write-ColorOutput "Press Ctrl+C to stop" "Blue"
    Write-Host ""
    Write-Host "Filter: Showing only errors, warnings, failures, and timeouts" -ForegroundColor Gray
    Write-Host "=" * 60
    
    try {
        $lastPosition = $latestFile.Length
        $noActivityCount = 0
        
        while ($true) {
            Start-Sleep -Seconds 1
            
            # Check if file still exists
            if (-not (Test-Path $latestFile.FullName)) {
                Write-ColorOutput "File no longer exists. Monitoring stopped." "Red"
                break
            }
            
            $currentFile = Get-Item $latestFile.FullName
            
            if ($currentFile.Length -gt $lastPosition) {
                # File has grown
                $stream = [System.IO.FileStream]::new($currentFile.FullName, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
                $reader = [System.IO.StreamReader]::new($stream)
                $reader.BaseStream.Seek($lastPosition, [System.IO.SeekOrigin]::Begin) | Out-Null
                
                while ($null -ne ($line = $reader.ReadLine())) {
                    if ($line -match "error|warn|fail|timeout") {
                        $timestamp = Get-Date -Format "HH:mm:ss"
                        Write-Host "[$timestamp] " -NoNewline -ForegroundColor Gray
                        
                        if ($line -match "error") {
                            Write-Host $line -ForegroundColor Red
                        } elseif ($line -match "warn") {
                            Write-Host $line -ForegroundColor Yellow
                        } elseif ($line -match "timeout") {
                            Write-Host $line -ForegroundColor Magenta
                        } else {
                            Write-Host $line -ForegroundColor Cyan
                        }
                    }
                }
                
                $lastPosition = $currentFile.Length
                $reader.Close()
                $stream.Close()
                $noActivityCount = 0
            } else {
                $noActivityCount++
                if ($noActivityCount % 30 -eq 0) {
                    Write-Host "." -NoNewline -ForegroundColor Gray
                }
            }
        }
    } catch {
        if ($_.Exception.Message -notlike "*The operation was canceled*") {
            Write-ColorOutput "Error during monitoring: $($_.Exception.Message)" "Red"
        }
    }
    
    Write-Host ""
    Write-ColorOutput "Monitoring stopped." "Yellow"
}

# Enhanced Generate Report with export options
function Generate-Report {
    Write-ColorOutput "Report Generation Options:" "Cyan"
    Write-Host "1. Full comprehensive report"
    Write-Host "2. Error summary report"
    Write-Host "3. Timeout analysis report"
    Write-Host "4. Custom timeframe report"
    Write-Host "0. Cancel"
    
    $reportType = Read-Host "Choose report type"
    
    if ($reportType -eq "0") {
        return
    }
    
    $exportFormat = Read-Host "Export format (TXT/CSV) [Default: TXT]"
    if ([string]::IsNullOrWhiteSpace($exportFormat)) {
        $exportFormat = "TXT"
    }
    
    $timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
    $reportFile = "log_analysis_$($global:SelectedEnvironment)_$timestamp.$($exportFormat.ToLower())"
    
    Write-ColorOutput "Generating report..." "Yellow"
    
    switch ($reportType) {
        "1" { Generate-FullReport -OutputFile $reportFile -Format $exportFormat }
        "2" { Generate-ErrorReport -OutputFile $reportFile -Format $exportFormat }
        "3" { Generate-TimeoutReport -OutputFile $reportFile -Format $exportFormat }
        "4" { 
            $timeframe = Get-TimeFrameSelection
            if ($timeframe) {
                Generate-TimeframeReport -OutputFile $reportFile -Format $exportFormat -Timeframe $timeframe
            }
        }
        default {
            Write-ColorOutput "Invalid option" "Red"
            return
        }
    }
    
    if (Test-Path $reportFile) {
        Write-ColorOutput "Report saved to: $reportFile" "Green"
        $open = Read-Host "Open report now? (y/n)"
        if ($open -match "^[Yy]") {
            Invoke-Item $reportFile
        }
    }
}

# Generate Full Report
function Generate-FullReport {
    param(
        [string]$OutputFile,
        [string]$Format
    )
    
    $report = @()
    $report += "Advantage Log Analysis Report - Full"
    $report += "=" * 50
    $report += "Generated: $(Get-Date)"
    $report += "Environment: $($global:SelectedEnvironment)"
    $report += "Log Path: $global:CurrentLogPath"
    $report += "Files Analyzed: $($global:LogFiles.Count)"
    $report += ""
    
    # Error Summary
    $report += "ERROR SUMMARY"
    $report += "-" * 30

    $errorPatternsForReport = @(
        "error", "warn", "fail", "timeout",
        "-- Exception Details --",
        "Advantage\.Framework\.AdvantageException",
        "Advantage\.Framework\.FieldValidationException",
        "\b\w*Exception\b",
        "ErrorCode:",
        "Can't commit, nested transaction was rolled back",
        "is not within the valid date range for the promotion",
        "Invalid Donor Type",
        "operation took \d+\.\d+s"
    )
    $errorCountsInReport = @{}
    $totalErrorLinesInReport = 0
    $processedFilesCount = 0

    foreach ($file in $global:LogFiles) {
        $processedFilesCount++
        Show-Progress -Current $processedFilesCount -Total $global:LogFiles.Count -Activity "Generating Full Report (Error Summary)"
        
        $content = Get-Content $file.FullName -ErrorAction SilentlyContinue
        if ($content) {
            foreach ($line in $content) {
                $lineMatched = $false
                foreach ($pattern in $errorPatternsForReport) {
                    if ($line -match $pattern) {
                        if ($errorCountsInReport.ContainsKey($pattern)) {
                            $errorCountsInReport[$pattern]++
                        } else {
                            $errorCountsInReport[$pattern] = 1
                        }
                        if (-not $lineMatched) { 
                            $totalErrorLinesInReport++
                            $lineMatched = $true
                        }
                        break 
                    }
                }
            }
        }
    }
    
    $report += "Total Log Lines with Errors/Warnings: $totalErrorLinesInReport"
    $report += "Error Pattern Counts:"
    $errorCountsInReport.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object {
        $report += "  $($_.Key): $($_.Value) occurrences"
    }
    $report += ""
    
    # File Information
    $report += "FILE INFORMATION"
    $report += "-" * 30
    
    $totalSize = ($global:LogFiles | Measure-Object -Property Length -Sum).Sum
    $report += "Total Size: $('{0:N2} MB' -f ($totalSize / 1MB))"
    $report += "Largest File: $(($global:LogFiles | Sort-Object Length -Descending | Select-Object -First 1).Name)"
    $report += "Oldest File: $(($global:LogFiles | Sort-Object LastWriteTime | Select-Object -First 1).Name)"
    $report += "Newest File: $(($global:LogFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1).Name)"
    
    Write-Host "`r" + (" " * 80) + "`r" -NoNewline  # Clear progress line
    
    if ($Format -eq "CSV") {
        # Convert to CSV format
        $csvData = @()
        $csvData += [PSCustomObject]@{
            ReportType = "Full"
            Generated = Get-Date
            Environment = $global:SelectedEnvironment
            TotalFiles = $global:LogFiles.Count
            TotalErrors = $totalErrors
            TotalSize = $totalSize
        }
        $csvData | Export-Csv -Path $OutputFile -NoTypeInformation
    } else {
        $report | Out-File -FilePath $OutputFile -Encoding UTF8
    }
}

# Main execution
function Main {
    # Select environment first
    if (-not (Select-Environment)) {
        Write-ColorOutput "Failed to select environment. Exiting." "Red"
        Read-Host "Press Enter to exit"
        return
    }
    
    # Main menu loop
    while ($true) {
        Show-Menu
        $choice = Read-Host "Choose option (1-9)"
        Write-Host ""
        
        $timeframe = $null
        if ($choice -in @("1","2","3","4","5","6","7")) {
            $timeframe = Get-TimeFrameSelection
            if (-not $timeframe) {
                Write-ColorOutput "Operation cancelled." "Yellow"
                continue
            }
        }
        
        # Load log files
        if (-not (Load-LogFiles -Timeframe $timeframe)) {
            Write-ColorOutput "Failed to load log files. Exiting." "Red"
            Read-Host "Press Enter to exit"
            return
        }
        
        switch ($choice.ToUpper()) {
            "1" { 
                Analyze-TimeFrame
                Read-Host "Press Enter to continue"
            }
            "2" { 
                Get-QuickSummary
                Read-Host "Press Enter to continue"
            }
            "3" { 
                Search-Timeouts
                Read-Host "Press Enter to continue"
            }
            "4" { 
                Search-Custom
                Read-Host "Press Enter to continue"
            }
            "5" { 
                Show-FileSizes
                Read-Host "Press Enter to continue"
            }
            "6" { 
                Monitor-RealTime
                Read-Host "Press Enter to continue"
            }
            "7" { 
                Generate-Report
                Read-Host "Press Enter to continue"
            }
            "8" { 
                if (Select-Environment) {
                    Load-LogFiles -Timeframe $null | Out-Null
                }
            }
            "9" { 
                Write-ColorOutput "Thank you for using Advantage Log Analyzer!" "Green"
                return
            }
            "H" {
                Show-Help
            }
            default { 
                Write-ColorOutput "Invalid option. Please choose 1-9 or H for help." "Red"
                Read-Host "Press Enter to continue"
            }
        }
    }
}

# Additional helper functions for report generation
function Generate-ErrorReport {
    param(
        [string]$OutputFile,
        [string]$Format
    )
    
    Write-Host "Generating error report..." -ForegroundColor Yellow

    $errorReportPatterns = @(
        "error", "warn", "fail",
        "-- Exception Details --",
        "Advantage\.Framework\.AdvantageException",
        "Advantage\.Framework\.FieldValidationException",
        "\b\w*Exception\b",
        "ErrorCode:",
        "Can't commit, nested transaction was rolled back",
        "is not within the valid date range for the promotion",
        "Invalid Donor Type",
        "operation took \d+\.\d+s"
    )
    
    $errors = @()
    $fileCount = 0
    
    foreach ($file in $global:LogFiles) {
        $fileCount++
        Show-Progress -Current $fileCount -Total $global:LogFiles.Count -Activity "Analyzing errors for report"
        
        $content = Get-Content $file.FullName -ErrorAction SilentlyContinue
        if ($content) {
            $lineNumber = 0
            foreach ($line in $content) {
                $lineNumber++
                foreach ($pattern in $errorReportPatterns) {
                    if ($line -match $pattern) {
                        $errors += [PSCustomObject]@{
                            File = $file.Name
                            Line = $lineNumber
                            Type = $pattern 
                            Content = $line.Trim()
                        }
                        break 
                    }
                }
            }
        }
    }
    
    Write-Host "`r" + (" " * 80) + "`r" -NoNewline
    
    if ($Format -eq "CSV") {
        $errors | Export-Csv -Path $OutputFile -NoTypeInformation
    } else {
        $report = @()
        $report += "Error Analysis Report"
        $report += "=" * 50
        $report += "Total Errors Found: $($errors.Count)"
        $report += ""
        
        $errors | Group-Object Type | ForEach-Object {
            $report += "$($_.Name): $($_.Count) occurrences"
        }
        
        $report | Out-File -FilePath $OutputFile -Encoding UTF8
    }
}

function Generate-TimeoutReport {
    param(
        [string]$OutputFile,
        [string]$Format
    )
    
    Write-Host "Generating timeout report..." -ForegroundColor Yellow
    
    $timeouts = @()
    $fileCount = 0
    
    foreach ($file in $global:LogFiles) {
        $fileCount++
        Show-Progress -Current $fileCount -Total $global:LogFiles.Count -Activity "Analyzing timeouts"
        
        $matches = Select-String -Path $file.FullName -Pattern "timeout|timed out" -ErrorAction SilentlyContinue
        foreach ($match in $matches) {
            $timeouts += [PSCustomObject]@{
                File = $file.Name
                Line = $match.LineNumber
                Content = $match.Line.Trim()
            }
        }
    }
    
    Write-Host "`r" + (" " * 80) + "`r" -NoNewline
    
    if ($Format -eq "CSV") {
        $timeouts | Export-Csv -Path $OutputFile -NoTypeInformation
    } else {
        $report = @()
        $report += "Timeout Analysis Report"
        $report += "=" * 50
        $report += "Total Timeouts Found: $($timeouts.Count)"
        $report += ""
        
        $timeouts | Select-Object -First 100 | ForEach-Object {
            $report += "$($_.File):$($_.Line) - $($_.Content)"
        }
        
        $report | Out-File -FilePath $OutputFile -Encoding UTF8
    }
}

function Generate-TimeframeReport {
    param(
        [string]$OutputFile,
        [string]$Format,
        [hashtable]$Timeframe
    )
    
    Write-Host "Generating timeframe report for: $($Timeframe.Description)" -ForegroundColor Yellow

    $errorPatternsForTimeframeReport = @(
        "error", "warn", "fail", "timeout",
        "-- Exception Details --",
        "Advantage\.Framework\.AdvantageException",
        "Advantage\.Framework\.FieldValidationException",
        "\b\w*Exception\b",
        "ErrorCode:",
        "Can't commit, nested transaction was rolled back",
        "is not within the valid date range for the promotion",
        "Invalid Donor Type",
        "operation took \d+\.\d+s"
    )
    
    $results = @()
    $fileCount = 0
    
    foreach ($file in $global:LogFiles) {
        $fileCount++
        Show-Progress -Current $fileCount -Total $global:LogFiles.Count -Activity "Analyzing timeframe"
        
        $content = Get-Content $file.FullName -ErrorAction SilentlyContinue
        if ($content) {
            $lineNumber = 0
            foreach ($line in $content) {
                $lineNumber++
                
                $lineDate = $null
                if ($line -match '(\d{4}-\d{2}-\d{2}[\s\t]\d{2}:\d{2}:\d{2})') {
                    try {
                        $lineDate = [DateTime]::Parse($matches[1])
                    } catch { }
                }
                
                if ($lineDate -and $lineDate -ge $Timeframe.StartDate -and $lineDate -le $Timeframe.EndDate) {
                    foreach ($pattern in $errorPatternsForTimeframeReport) {
                        if ($line -match $pattern) {
                            $results += [PSCustomObject]@{
                                File = $file.Name
                                Line = $lineNumber
                                Timestamp = $matches[1] 
                                Content = $line.Trim()
                                MatchedPattern = $pattern 
                            }
                            break 
                        }
                    }
                }
            }
        }
    }
    
    Write-Host "`r" + (" " * 80) + "`r" -NoNewline
    
    if ($Format -eq "CSV") {
        $results | Export-Csv -Path $OutputFile -NoTypeInformation
    } else {
        $report = @()
        $report += "Timeframe Analysis Report"
        $report += "=" * 50
        $report += "Timeframe: $($Timeframe.Description)"
        $report += "Start: $($Timeframe.StartDate)"
        $report += "End: $($Timeframe.EndDate)"
        $report += "Results Found: $($results.Count)"
        $report += ""
        
        $results | Select-Object -First 500 | ForEach-Object {
            $report += "$($_.File):$($_.Line) [$($_.Timestamp)] - $($_.Content)"
        }
        
        $report | Out-File -FilePath $OutputFile -Encoding UTF8
    }
}

# Run the main function
Main
