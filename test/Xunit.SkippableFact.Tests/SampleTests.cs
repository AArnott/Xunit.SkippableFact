// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

using System.Runtime.Versioning;

namespace Xunit.SkippableFact.Tests;

public class SampleTests
{
    [SkippableFact]
    public void SkipMe()
    {
        Skip.If(true, "Because it's a sample.");
    }

    [SkippableFact]
    public void DoNotSkipMe()
    {
    }

    [SkippableFact(typeof(NotImplementedException))]
    public void SkipByOtherException()
    {
        throw new NotImplementedException();
    }

    [SkippableFact(typeof(NotImplementedException), typeof(NotSupportedException))]
    public void SkipByOtherException_Ex2()
    {
        throw new NotSupportedException();
    }

    [SkippableFact(typeof(NotImplementedException))]
    public void SkipByOtherException_NotSkipped()
    {
    }

    [SkippableFact(typeof(NotSupportedException))]
    public void SkipByOtherException_Nested()
    {
        try
        {
            throw new InvalidOperationException();
        }
        catch (InvalidOperationException ex)
        {
            throw new NotSupportedException(ex.Message, ex);
        }
    }

    [SkippableTheory(typeof(NotImplementedException))]
    [InlineData(true)]
    [InlineData(false)]
    public void SkipTheoryMaybe_OtherExceptions(bool skip)
    {
        if (skip)
        {
            throw new NotImplementedException();
        }
    }

    [SkippableTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void SkipTheoryMaybe(bool skip)
    {
        Skip.If(skip, "I was told to.");
    }

    [SkippableFact]
    public void SkipInsideAssertThrows()
    {
        Assert.Throws<Exception>(new Action(() =>
        {
            Skip.If(true, "Skip inside Assert.Throws");
            throw new Exception();
        }));
    }

#if NET5_0_OR_GREATER
    [SkippableFact, SupportedOSPlatform("Linux")]
    public void LinuxOnly()
    {
        Assert.True(OperatingSystem.IsLinux(), "This should only run on Linux");
    }

    [SkippableFact, SupportedOSPlatform("macOS")]
    public void MacOsOnly()
    {
        Assert.True(OperatingSystem.IsMacOS(), "This should only run on macOS");
    }

    [SkippableFact, SupportedOSPlatform("macOS10.6")]
    public void MacOs10_6Minimum()
    {
        Assert.True(OperatingSystem.IsMacOSVersionAtLeast(10, 6), "This should only run on macOS 10.6 onwards");
    }

    [SkippableFact, SupportedOSPlatform("macOS77.7")]
    public void MacOs77_7Minimum()
    {
        Assert.True(OperatingSystem.IsMacOSVersionAtLeast(77, 7), "This should only run on macOS 77.7 onwards");
    }

    [SkippableFact, SupportedOSPlatform("Windows")]
    public void WindowsOnly()
    {
        Assert.True(OperatingSystem.IsWindows(), "This should only run on Windows");
    }

    [SkippableFact, SupportedOSPlatform("Windows10.0")]
    public void Windows10Minimum()
    {
        Assert.True(OperatingSystem.IsWindowsVersionAtLeast(10), "This should only run on Windows 10.0 onwards");
    }

    [SkippableFact, SupportedOSPlatform("Windows77.7")]
    public void Windows77_7Minimum()
    {
        Assert.True(OperatingSystem.IsWindowsVersionAtLeast(77, 7), "This should only run on Windows 77.7 onwards");
    }

    [SkippableFact, SupportedOSPlatform("Android"), SupportedOSPlatform("Browser")]
    public void AndroidAndBrowserFact()
    {
        Assert.True(OperatingSystem.IsAndroid() || OperatingSystem.IsBrowser(), "This should only run on Android and Browser");
    }

    [SkippableTheory, SupportedOSPlatform("Android"), SupportedOSPlatform("Browser")]
    [InlineData(1)]
    [InlineData(2)]
    public void AndroidAndBrowserTheory(int number)
    {
        _ = number;
        Assert.True(OperatingSystem.IsAndroid() || OperatingSystem.IsBrowser(), "This should only run on Android and Browser");
    }

    [SkippableFact, SupportedOSPlatform("Android"), SupportedOSPlatform("Browser"), SupportedOSPlatform("Wasi")]
    public void AndroidAndBrowserAndWasiOnly()
    {
        Assert.True(OperatingSystem.IsAndroid() || OperatingSystem.IsBrowser() || OperatingSystem.IsWasi(), "This should only run on Android, Browser and Wasi");
    }

    [SkippableFact, UnsupportedOSPlatform("Linux"), UnsupportedOSPlatform("macOS"), UnsupportedOSPlatform("Windows")]
    public void UnsupportedPlatforms()
    {
        Assert.False(OperatingSystem.IsLinux());
        Assert.False(OperatingSystem.IsMacOS());
        Assert.False(OperatingSystem.IsWindows());
    }
#endif
}
