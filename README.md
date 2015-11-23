Xunit.SkippableFact
======================

[![Build status](https://ci.appveyor.com/api/projects/status/06titf9dsyu2xoms/branch/master?svg=true)](https://ci.appveyor.com/project/AArnott/xunit-skippablefact/branch/master)

This project allows for Xunit tests that can determine during execution
that they should report a "skipped" result. This can be useful when
a precondition is not satisfied, or the test is over functionality that
does not exist on the platform being tested.

## Installation

This project is available as a [NuGet package][NuPkg]

## Example

    [SkippableFact]
    public void SomeTestForWindowsOnly()
    {
        Skip.IfNot(Environment.IsWindows);

        // Test Windows only functionality.
    }

 [NuPkg]: https://www.nuget.org/packages/Xunit.SkippableFact
