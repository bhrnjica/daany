

dotnet restore ../daany.sln
dotnet build -c Release ../daany.sln
dotnet test -c Release ../daany.sln


# build this nuget packs
../nuget.exe pack Daany.DataFrame_v2.nuspec  -Properties Configuration=Release -IncludeReferencedProjects -OutputDirectory ../nuget/ 

../nuget.exe pack Daany.DataFrame.Ext_v2.nuspec  -Properties Configuration=Release -IncludeReferencedProjects -OutputDirectory ../nuget/

../nuget.exe pack Daany.Stat_v2.nuspec  -Properties Configuration=Release -IncludeReferencedProjects -OutputDirectory ../nuget/

../nuget.exe pack Daany.LinA.win-x64_v2.nuspec  -Properties Configuration=Release -IncludeReferencedProjects -OutputDirectory ../nuget/

dotnet build -c Linux_Release ../daany.sln


../nuget.exe pack Daany.LinA.linux-x64_v2.nuspec  -Properties Configuration=Linux_Release -IncludeReferencedProjects -OutputDirectory ../nuget/