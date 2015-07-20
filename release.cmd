@pushd "%~dp0"

git clean -fdx src
nuget restore src
msbuild /nologo /v:minimal /m /fl src\Xunit.SkippableFact.sln /p:configuration=release,UseNonZeroBuildNumber=true
vstest.console.exe src\Xunit.SkippableFact.Tests\bin\release\Xunit.SkippableFact.Tests.dll /TestAdapterPath:packages

@echo Last steps to release:
@echo git tag v1.0.******
@echo git push origin v1.0.******
@echo nuget push src\Xunit.SkippableFact.NuGet\bin\release\Xunit.SkippableFact.*.nupkg

@popd
