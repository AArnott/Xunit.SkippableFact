# Copilot instructions for this repository

## High level guidance

* Review the `CONTRIBUTING.md` file for instructions to build and test the software.
* Run the `.github/Prime-ForCopilot.ps1` script (once) before running any `dotnet` or `msbuild` commands.
  If you see any build errors about not finding git objects or a shallow clone, it may be time to run this script again.

## Software Design

* Design APIs to be highly testable, and all functionality should be tested.
* Avoid introducing binary breaking changes in public APIs of projects under `src` unless their project files have `IsPackable` set to `false`.

## Testing

**IMPORTANT**: This repository uses xunit v2 with the `YTest.MTP.XUnit2` adapter for Microsoft.Testing.Platform (MTP v2). Pass the adapter's `--filter` expressions after `--`.

* There should generally be one test project (under the `test` directory) per shipping project (under the `src` directory). Test projects are named after the project being tested with a `.Tests` suffix.
* Tests must stay on xunit v2 because this repository ships an xunit v2 extension.
* Some tests are known to be unstable or intentionally fail. Exclude them with `-- --filter "TestCategory!=FailsInCloudTest"`, as the cloud test script does.

### Running Tests

**Run all tests**:
```bash
dotnet test --no-build -c Release
```

**Run tests for a specific test project**:
```bash
dotnet test --project test/Xunit.SkippableFact.Tests/Xunit.SkippableFact.Tests.csproj --no-build -c Release
```

**Run a single test method**:
```bash
dotnet test --project test/Xunit.SkippableFact.Tests/Xunit.SkippableFact.Tests.csproj --no-build -c Release -- --filter "FullyQualifiedName~ClassName.MethodName"
```

**Run all tests in a test class**:
```bash
dotnet test --project test/Xunit.SkippableFact.Tests/Xunit.SkippableFact.Tests.csproj --no-build -c Release -- --filter "FullyQualifiedName~ClassName"
```

**Run tests with a name substring**:
```bash
dotnet test --project test/Xunit.SkippableFact.Tests/Xunit.SkippableFact.Tests.csproj --no-build -c Release -- --filter "FullyQualifiedName~Pattern"
```

**Run tests with a specific trait** (equivalent to category filtering):
```bash
dotnet test --project test/Xunit.SkippableFact.Tests/Xunit.SkippableFact.Tests.csproj --no-build -c Release -- --filter "TraitName=value"
```

**Exclude tests with a specific trait** (skip unstable tests):
```bash
dotnet test --project test/Xunit.SkippableFact.Tests/Xunit.SkippableFact.Tests.csproj --no-build -c Release -- --filter "TestCategory!=FailsInCloudTest"
```

**Run tests for a specific framework only**:
```bash
dotnet test --project test/Xunit.SkippableFact.Tests/Xunit.SkippableFact.Tests.csproj --no-build -c Release --framework net8.0
```

**List all available tests without running them**:
```bash
cd test/Xunit.SkippableFact.Tests
dotnet run --no-build -c Release --framework net8.0 -- --list-tests
```

**Key points about test filtering with MTP v2 / xunit v2**:
- Options after `--` are passed to the test runner, not to `dotnet test`
- Use `--filter` with `FullyQualifiedName` or trait names
- `=` matches exactly, `~` matches a substring, and `!=` excludes a value
- Do not use xunit v3 `--filter-method` / `--filter-not-trait` or TUnit `--treenode-filter` switches with this adapter
- See `--help` for supported filter expressions

## Coding style

* Honor StyleCop rules and fix any reported build warnings *after* getting tests to pass.
* In C# files, use namespace *statements* instead of namespace *blocks* for all new files.
* Add API doc comments to all new public and internal members.
