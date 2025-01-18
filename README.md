Xunit.SkippableFact
======================

[![GitHub Actions status](https://github.com/aarnott/Xunit.SkippableFact/actions/workflows/build.yml/badge.svg)](https://github.com/AArnott/Xunit.SkippableFact/actions/workflows/build.yml)
[![NuGet package](https://img.shields.io/nuget/v/xunit.skippablefact.svg)](https://nuget.org/packages/xunit.skippablefact)

This project allows for Xunit tests that can determine during execution that they should report a "skipped" result.
This can be useful when a precondition is not satisfied, or the test is over functionality that does not exist on the platform being tested.

This package targets Xunit v2.
Xunit v3 has skipping built-in.
See [our Xunit v3 migration doc](https://aarnott.github.io/Xunit.SkippableFact/docs/xunit-v3.html).

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

## Sponsorships

[<img src="https://api.gitsponsors.com/api/badge/img?id=39364524" height="20">](https://api.gitsponsors.com/api/badge/link?p=gfTAaPbnYnow49AIjp8+x6zqItEv9S5jyYxbQltcORwVT6sruhrlqC8A/BAUipwCTnxBrMnnlPhVz2HXELTWe3KqMHrfiusNcB64Wnh4efdCTfCjt5lR/fofmSyjWbMoakjbDpQOHfWcwGy1lvsUbw==) [GitHub Sponsors](https://github.com/sponsors/AArnott)
[Zcash](zcash:u1vv2ws6xhs72faugmlrasyeq298l05rrj6wfw8hr3r29y3czev5qt4ugp7kylz6suu04363ze92dfg8ftxf3237js0x9p5r82fgy47xkjnw75tqaevhfh0rnua72hurt22v3w3f7h8yt6mxaa0wpeeh9jcm359ww3rl6fj5ylqqv54uuwrs8q4gys9r3cxdm3yslsh3rt6p7wznzhky7)

[NuPkg]: https://www.nuget.org/packages/Xunit.SkippableFact
