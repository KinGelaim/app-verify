nuget.exe pack HealthCheck.nuspec
nuget delete HealthCheck 1.0.0-alpha1 -source c:\nuget -noninteractive
nuget add HealthCheck.1.0.0-alpha1.nupkg -source c:\nuget 