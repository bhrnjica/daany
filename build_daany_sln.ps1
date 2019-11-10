$buildEnvironmentVisualStudio2019BuildTools = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise";
									 		  
$buildEnvironmentVisualStudio2019Community = "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools";
$buildEnvironmentVisualStudio2017BuildTools = "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\";
$buildEnvironmentVisualStudio2017Community = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\";

$msbuild = "";
$vstest = "";

$nuget = "./nuget.exe";
$nugetDownloadUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe";

# Detect build environment

if (Test-Path $buildEnvironmentVisualStudio2017BuildTools -PathType Container) {
    # https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=BuildTools&rel=15
    Write-Host "Visual Studio 2017 BuildTools detected";
    $msbuild = $buildEnvironmentVisualStudio2017BuildTools + "\MSBuild\15.0\Bin\MSBuild.exe";
    $vstest = $buildEnvironmentVisualStudio2017BuildTools + "\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe";
} elseif (Test-Path $buildEnvironmentVisualStudio2017Community -PathType Container) {
    Write-Host "Visual Studio 2017 Community detected";
    $msbuild = $buildEnvironmentVisualStudio2017Community + "\MSBuild\15.0\Bin\MSBuild.exe";
    $vstest = $buildEnvironmentVisualStudio2017Community + "\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe";
}elseif (Test-Path $buildEnvironmentVisualStudio2019BuildTools -PathType Container) {
    Write-Host "Visual Studio 2019 Enterprise detected";
    $msbuild = $buildEnvironmentVisualStudio2019BuildTools + "\MSBuild\Current\Bin\MSBuild.exe";
    $vstest = $buildEnvironmentVisualStudio2019BuildTools + "\Common7\IDE\Extensions\TestPlatform\vstest.console.exe";
}
 else {
    Write-Error "No compatible build environment detect";
    exit 1;
}

# Check needed file are exists

if (-Not ([System.IO.File]::Exists($msbuild))) {
    Write-Error "msbuild not found (MSBuild.exe)";
    exit 1;
}

if (-Not ([System.IO.File]::Exists($vstest))) {
    Write-Error "vstest not found (vstest.console.exe)";
    exit 1;
}

# Check is nuget available otherwise download

if (-Not (Test-Path -Path $nuget)) {
    Write-Host "nuget.exe not found download start"
    Invoke-WebRequest -Uri $nugetDownloadUrl -OutFile $nuget;
}

# Find Solution file and build this

Get-ChildItem -Path .\ -Filter *.sln -Recurse -File -Name| ForEach-Object {
    # info -> $_ = filename

    # restore nuget package for solution
    # ----------------------------------------------

    # add custom nuget source
    #&$nuget sources Add -Name "nuget.privatenuget.com" -source "http://nuget.privatenuget.com/nuget";
    # restore microsoft nuget server
    &$nuget restore $_;

    # build project
    # -----------------------------------------------

    #&$msbuild ($_, '/p:Configuration=Release', '/t:Clean,Build');
    &$msbuild ($_, '/verbosity:q', '/p:Configuration=Release', '/t:Clean,Build');

    # info -> $? = Returns True or False value indicating whether previous command ended with an error
    if (!$?) {
        Write-Error "error build $_";
        exit 1;
    }

    Write-Host "build successful $_";
}

# start unit tests

Get-ChildItem -Path .\ -Filter *.test.dll -Recurse -File -Name| ForEach-Object {
    if ($_ -like '*\bin\Release\*') {
        Write-Host "start unit test $_";
        &$vstest $_;

        if (!$?) {
            Write-Error "error unit test $_";
            exit 1;
        }
    } else {
        Write-Host "ignore unit test dll, wrong directory $_";
    }
}

# build this nuget packs
./nuget.exe pack Daany.DataFrame.nuspec -Properties Configuration=Release -IncludeReferencedProjects -OutputDirectory nuget/ 

./nuget.exe pack Daany.DataFrame.Ext.nuspec -Properties Configuration=Release -IncludeReferencedProjects -OutputDirectory nuget/

./nuget.exe pack Daany.Stat.nuspec -Properties Configuration=Release -IncludeReferencedProjects -OutputDirectory nuget/
