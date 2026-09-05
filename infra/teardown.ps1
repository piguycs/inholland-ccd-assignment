#!/usr/bin/env pwsh

$env:SUFFIX = "kd26"
$env:RG = "inh-ccd-lab2-$($env:SUFFIX)"

az group delete -n $env:RG --yes --no-wait
