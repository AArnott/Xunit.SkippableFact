Xunit.SkippableFact
======================

[![GitHub Actions status](https://github.com/aarnott/Library.Template/workflows/CI/badge.svg)](https://github.com/AArnott/Xunit.SkippableFact/actions/workflows/build.yml)
[![NuGet package](https://img.shields.io/nuget/v/xunit.skippablefact.svg)](https://nuget.org/packages/xunit.skippablefact)

This project allows for Xunit tests that can determine during execution
that they should report a "skipped" result. This can be useful when
a precondition is not satisfied, or the test is over functionality that
does not exist on the platform being tested.

## Installation

This project is available as a [NuGet package][NuPkg]

## Example

```csharp
[SkippableFact]
public void SomeTestForWindowsOnly()
{
    Skip.IfNot(Environment.IsWindows);

    // Test Windows only functionality.
}
```

You can also automatically report tests as skipped based on specific exception types.
This is particularly useful when the test runs against multiple target frameworks and
your library is not expected to be implemented in some of them. For example:

```csharp
[SkippableFact(typeof(NotSupportedException), typeof(NotImplementedException))]
public void TestFunctionalityWhichIsNotSupportedOnSomePlatforms()
{
    // Test functionality. If it throws any of the exceptions listed in the attribute,
    // a skip result is reported instead of a failure.
}
```

### The [SupportedOSPlatform] attribute

Since version 1.5, `Xunit.SkippableFact` understands the `SupportedOSPlatform` attribute to skip tests on unsupported platforms.

```csharp
[SkippableFact, SupportedOSPlatform("Windows")]
public void TestCngKey()
{
    var key = CngKey.Create(CngAlgorithm.Sha256);
    Assert.NotNull(key);
}
```

Without `[SupportedOSPlatform("Windows")` the [CA1416](CA1416) code analysis warning would trigger:
> This call site is reachable on all platforms. 'CngKey. Create(CngAlgorithm)' is only supported on: 'windows'.

Adding `[SupportedOSPlatform("Windows")` both suppresses this platform compatibility warning and skips the test when running on Linux or macOS.

[NuPkg]: https://www.nuget.org/packages/Xunit.SkippableFact
[CA1416]: https://learn.microsoft.com/en-gb/dotnet/fundamentals/code-analysis/quality-rules/ca1416
