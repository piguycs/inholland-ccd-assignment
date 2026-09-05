#!/usr/bin/env pwsh

$env:SUFFIX = "kd26"
$env:LOCATION = "belgiumcentral" # westeurope is not allowed
$env:RG = "inh-ccd-lab2-$($env:SUFFIX)"

$bicepFile = Join-Path $PSScriptRoot "basic.bicep"

az group create -n $env:RG -l $env:LOCATION

az deployment group create -g $env:RG -f $bicepFile --parameters "namePrefix=$($env:SUFFIX)"
