Xunit.SkippableFact
======================

[![GitHub Actions status](https://github.com/aarnott/Xunit.SkippableFact/actions/workflows/build.yml/badge.svg)](https://github.com/AArnott/Xunit.SkippableFact/actions/workflows/build.yml)
[![NuGet package](https://img.shields.io/nuget/v/xunit.skippablefact.svg)](https://nuget.org/packages/xunit.skippablefact)

This project allows for Xunit tests that can determine during execution that they should report a "skipped" result.
This can be useful when a precondition is not satisfied, or the test is over functionality that does not exist on the platform being tested.

## Installation

This project is available as a [NuGet package][NuPkg]

## Usage

[Learn more at our documentation site](https://aarnott.github.io/Xunit.SkippableFact/).

Below is a sampling of uses.

Skip based on a runtime check:

```csharp
[SkippableFact]
public void SomeMoodyTest()
{
    Skip.IfNot(InTheMood);
}
```

Skip based on a thrown exception:

```csharp
[SkippableFact(typeof(NotSupportedException))]
public void TestFunctionalityWhichIsNotSupportedOnSomePlatforms()
{
    // Test functionality. If it throws any of the exceptions listed in the attribute,
    // a skip result is reported instead of a failure.
}
```

Skip based on `SupportedOSPlatformAttribute`:

```csharp
[SkippableFact, SupportedOSPlatform("Windows")]
public void TestCngKey()
{
    var key = CngKey.Create(CngAlgorithm.Rsa);
    Assert.NotNull(key);
}
```

[NuPkg]: https://www.nuget.org/packages/Xunit.SkippableFact
