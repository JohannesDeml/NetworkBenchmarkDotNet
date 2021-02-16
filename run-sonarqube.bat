
dotnet C:\sonarqube\scanner\SonarScanner.MSBuild.dll begin /k:"network-benchmark-dotnet" /d:sonar.host.url="http://localhost:9000"
dotnet build NetworkBenchmarkDotNet\NetworkBenchmarkDotNet.csproj  
dotnet C:\sonarqube\scanner\SonarScanner.MSBuild.dll end
PAUSE