#!/usr/bin/env pwsh

[CmdletBinding()]
param(
    [string] $ResourceGroup = 'weather-image-app-rg',
    [string] $FunctionAppName = 'weather-image-app',
    [string] $Location = 'belgiumcentral'
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

$temp = Join-Path ([IO.Path]::GetTempPath()) "weather-$([guid]::NewGuid())"
$publish = Join-Path $temp 'publish'
$zip = Join-Path $temp 'app.zip'

try {
    New-Item -ItemType Directory -Path $temp | Out-Null

    dotnet publish "$PSScriptRoot/RequestImage/RequestImage.csproj" `
        -c Release -r linux-x64 --self-contained false -o $publish

    # Include hidden .azurefunctions files and put host.json at the ZIP root.
    [IO.Compression.ZipFile]::CreateFromDirectory($publish, $zip)

    az group create --name $ResourceGroup --location $Location --output none
    az deployment group create --resource-group $ResourceGroup `
        --template-file "$PSScriptRoot/infra/main.bicep" `
        --parameters "functionAppName=$FunctionAppName" "location=$Location" --output none
    az functionapp deployment source config-zip --resource-group $ResourceGroup `
        --name $FunctionAppName --src $zip --build-remote false --output none
} finally {
    if (Test-Path $temp) { Remove-Item $temp -Recurse -Force }
}
