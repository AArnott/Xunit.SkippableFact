# Getting Started

## Installation

Consume this library via its NuGet Package.
Click on the badge to find its latest version and the instructions for consuming it that best apply to your project.

[![NuGet package](https://img.shields.io/nuget/v/Xunit.SkippableFact.svg)](https://nuget.org/packages/Xunit.SkippableFact)

## Usage

Always use the @Xunit.SkippableFactAttribute or @Xunit.SkippableTheoryAttribute on test methods that you want to potentially skip for.

### Conditional runtime skipping

Use methods on the @Xunit.Skip class to skip a test based on some runtime check.

This can be useful when you need to test for specific runtime conditions.
It can also be useful to skip certain test cases in a theory when input combinations are invalid.

[!code-csharp[](../../samples/GettingStarted.cs#RuntimeCheck)]

### Exceptions thrown

You can also automatically report tests as skipped based on specific exception types.
This is particularly useful when the test runs against multiple target frameworks and
your library is not expected to be implemented in some of them. For example:

[!code-csharp[](../../samples/GettingStarted.cs#ThrownExceptions)]

### Supported platforms

Apply the @System.Runtime.Versioning.SupportedOSPlatformAttribute and/or @System.Runtime.Versioning.UnsupportedOSPlatformAttribute to a test method or test class to skip it based on the platform the test is running on.

[!code-csharp[](../../samples/GettingStarted.cs#OSCheck)]

Without `[SupportedOSPlatform("Windows")]` the [CA1416][CA1416] code analysis warning would trigger:

> This call site is reachable on all platforms. 'CngKey. Create(CngAlgorithm)' is only supported on: 'windows'.

Adding `[SupportedOSPlatform("Windows")]` both suppresses this platform compatibility warning and skips the test when running on Linux or macOS.

[NuPkg]: https://www.nuget.org/packages/Xunit.SkippableFact
[CA1416]: https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416
