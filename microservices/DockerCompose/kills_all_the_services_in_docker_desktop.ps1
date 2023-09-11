docker rm -f NodeController

$containers = docker ps -a -q

foreach ($container in $containers)
{
    Start-Job -ScriptBlock { param($c) docker rm -f $c } -ArgumentList $container
}

While (Get-Job -State "Running")
{
    Start-Sleep 2
}

Get-Job | Remove-Job
