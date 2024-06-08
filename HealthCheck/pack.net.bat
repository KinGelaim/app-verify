nuget.exe pack HealthCheck.nuspec
nuget delete HealthCheck 1.0.0-alpha1 Password -source https://nuget/api/v1/package -noninteractive
nuget push HealthCheck.1.0.0-alpha1.nupkg Password -source https://nuget/api/v1/package