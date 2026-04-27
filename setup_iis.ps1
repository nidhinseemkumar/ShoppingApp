$ErrorActionPreference = "Stop"

$siteName = "ShoppingApp"
$port = "8080"
$physicalPath = "C:\inetpub\wwwroot\ShoppingApp"
$publishSource = "c:\Users\midhu\OneDrive\Desktop\Nidhin\claysys_projects\ShoppingApp\publish"

Import-Module WebAdministration

Write-Host "--- Starting IIS Setup for $siteName ---" -ForegroundColor Cyan

# 0. Forcefully stop IIS services to release all file locks
Write-Host "Stopping IIS Services..." -ForegroundColor Yellow
& net stop was /y

# 1. Create directory and copy files
if (!(Test-Path $physicalPath)) {
    Write-Host "Creating directory: $physicalPath"
    New-Item -ItemType Directory -Path $physicalPath -Force
}

Write-Host "Copying published files..."
Copy-Item -Path "$publishSource\*" -Destination $physicalPath -Recurse -Force

# 2. Start IIS Services back up
Write-Host "Restarting IIS Services..." -ForegroundColor Yellow
& net start w3svc

# 3. Create Application Pool if missing
if (!(Test-Path "IIS:\AppPools\$siteName")) {
    Write-Host "Creating Application Pool: $siteName"
    New-Item "IIS:\AppPools\$siteName"
}
Set-ItemProperty "IIS:\AppPools\$siteName" -Name "managedRuntimeVersion" -Value ""

# 4. Create Website if missing
if (!(Test-Path "IIS:\Sites\$siteName")) {
    Write-Host "Creating Website: $siteName on port $port"
    New-Item "IIS:\Sites\$siteName" -bindings @{protocol="http";bindingInformation="*:$( $port ):"} -physicalPath $physicalPath
}
Set-ItemProperty "IIS:\Sites\$siteName" -Name "applicationPool" -Value $siteName

# 5. Set Permissions
Write-Host "Setting folder permissions..."
$acl = Get-Acl $physicalPath
$permission = "IIS AppPool\$siteName","ReadAndExecute","Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
$acl.SetAccessRule($accessRule)
Set-Acl $physicalPath $acl

Write-Host "--- IIS Setup Completed Successfully! ---" -ForegroundColor Green
Write-Host "You can now access your app at: http://localhost:$port" -ForegroundColor Green
