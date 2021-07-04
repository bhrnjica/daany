$nversion = "1.1.0"

dotnet clean -c Debug
dotnet clean -c Release
dotnet clean -c Linux

dotnet restore

dotnet build -c Release
dotnet test -c Release

dotnet build -c Linux
#dotnet test -c Linux

dotnet pack -p:PackageVersion=$nversion -p:NuspecFile=../../Daany.DataFrame.nuspec -p:Configuration=Release  -o nuget/
dotnet pack -p:PackageVersion=$nversion -p:NuspecFile=../../Daany.DataFrame.Ext.nuspec -p:Configuration=Release  -o nuget/
dotnet pack -p:PackageVersion=$nversion -p:NuspecFile=../../Daany.Stat.nuspec -p:Configuration=Release  -o nuget/
dotnet pack -p:PackageVersion=$nversion -p:NuspecFile=../../Daany.LinA-win-x64.nuspec -p:Configuration=Release  -o nuget/
dotnet pack -p:PackageVersion=$nversion -p:NuspecFile=../../Daany.LinA-linux-x64.nuspec -p:Configuration=Linux  -o nuget/

dotnet clean -c Debug
dotnet clean -c Release
dotnet clean -c Linux