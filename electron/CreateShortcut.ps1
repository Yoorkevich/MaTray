# Creates a TrayManager.lnk in the Start Menu programs folder.
# Usage: powershell -ExecutionPolicy Bypass -File CreateShortcut.ps1

$scriptDir     = Split-Path -Parent $MyInvocation.MyCommand.Definition
$electronExe   = Join-Path $scriptDir "node_modules\.bin\electron.cmd"
$shortcutPath  = Join-Path $env:APPDATA "Microsoft\Windows\Start Menu\Programs\TrayManager.lnk"

$shell = New-Object -ComObject WScript.Shell
$lnk   = $shell.CreateShortcut($shortcutPath)
$lnk.TargetPath       = $electronExe
$lnk.Arguments         = "."
$lnk.WorkingDirectory  = $scriptDir
$lnk.Description       = "TrayManager - Background Process Manager"
$lnk.Save()

Write-Host "Created shortcut: $shortcutPath"
Write-Host "Target: $electronExe ."
