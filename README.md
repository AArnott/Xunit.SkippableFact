Xunit.SkippableFact
======================

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
