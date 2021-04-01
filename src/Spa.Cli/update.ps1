dotnet tool uninstall -g Spa.Cli
dotnet pack
dotnet tool install --global --add-source ./nupkg Spa.Cli