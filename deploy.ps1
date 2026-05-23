# Deploy ArgentinaLightHouses to Azure App Service
# Prerequisites: Azure CLI installed, .NET SDK installed

$ResourceGroup  = "ArgentinaLightHousesRg"
$AppServiceName = "ArgentinaLightHouses"
$SubscriptionId = "bb5ffe61-553c-4019-a657-79878bed7e08"
$PublishFolder  = "./publish"
$ZipPath        = "./publish.zip"

# 1. Login and set subscription
az login
az account set --subscription $SubscriptionId

# 2. Build and publish the app
Write-Host "Building and publishing the .NET app..." -ForegroundColor Cyan
dotnet publish ArgentinaLightHouses.csproj -c Release -o $PublishFolder

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed. Aborting."
    exit 1
}

# 3. Zip the publish output
Write-Host "Creating deployment package..." -ForegroundColor Cyan
if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
Compress-Archive -Path "$PublishFolder/*" -DestinationPath $ZipPath

# 4. Deploy to Azure App Service
Write-Host "Deploying to Azure App Service '$AppServiceName'..." -ForegroundColor Cyan
az webapp deploy `
    --resource-group $ResourceGroup `
    --name $AppServiceName `
    --src-path $ZipPath `
    --type zip `
    --subscription $SubscriptionId

if ($LASTEXITCODE -ne 0) {
    Write-Error "Deployment failed."
    exit 1
}

Write-Host "Deployment complete! https://$AppServiceName.azurewebsites.net" -ForegroundColor Green

# 5. Cleanup
Remove-Item $ZipPath -Force
Remove-Item $PublishFolder -Recurse -Force
