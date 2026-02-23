# Creates a TrayManager.lnk shortcut at the solution root.
# Usage: powershell -ExecutionPolicy Bypass -File CreateShortcut.ps1

$scriptDir   = Split-Path -Parent $MyInvocation.MyCommand.Definition
$targetExe   = Join-Path $scriptDir "src\TrayManager\bin\Release\net9.0-windows\TrayManager.exe"
$shortcutPath = Join-Path $scriptDir "TrayManager.lnk"

$shell = New-Object -ComObject WScript.Shell
$lnk   = $shell.CreateShortcut($shortcutPath)
$lnk.TargetPath       = $targetExe
$lnk.WorkingDirectory  = Split-Path $targetExe
$lnk.Description       = "TrayManager - Background Process Manager"
$lnk.Save()

Write-Host "Created shortcut: $shortcutPath"
Write-Host "Target: $targetExe"
