# Remove a specific container
docker rm -f NodeController

# Get all container IDs (including stopped ones)
$containers = docker ps -a -q

# Loop through each container ID and start a job to remove it
foreach ($container in $containers)
{
    Start-Job -ScriptBlock { param($c) docker rm -f $c } -ArgumentList $container
}

# Wait for all jobs to complete
While (Get-Job -State "Running")
{
    Start-Sleep 2
}

# Clean up the completed jobs
Get-Job | Remove-Job
