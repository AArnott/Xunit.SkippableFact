// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace Xunit.Sdk;

/// <summary>
/// Extensions methods on <see cref="ITestMethod"/>.
/// </summary>
internal static class TestMethodExtensions
{
    /// <summary>
    /// Assesses whether the test method can run on the current platform by looking at the <c>[SupportedOSPlatform]</c> attributes on the test method and on the test class.
    /// </summary>
    /// <param name="testMethod">The <see cref="ITestMethod"/>.</param>
    /// <returns>A description of the supported platforms if the test can not run on the current platform or <see langword="null"/> if the test can run on the current platform.</returns>
    internal static string? GetPlatformSkipReason(this ITestMethod testMethod)
    {
#if NET462
        return null;
#else
        HashSet<string> unsupportedPlatforms = GetPlatforms(testMethod, "UnsupportedOSPlatform");
        string? unsupportedPlatform = unsupportedPlatforms.FirstOrDefault(MatchesCurrentPlatform);
        if (unsupportedPlatform is not null)
        {
            return $"Unsupported on {unsupportedPlatform}";
        }

        HashSet<string> supportedPlatforms = GetPlatforms(testMethod, "SupportedOSPlatform");
        if (supportedPlatforms.Count == 0 || supportedPlatforms.Any(MatchesCurrentPlatform))
        {
            return null;
        }

        string platformsDescription = supportedPlatforms.Count == 1 ? supportedPlatforms.First() : string.Join(", ", supportedPlatforms.Reverse().Skip(1).Reverse()) + " and " + supportedPlatforms.Last();
        return $"Only supported on {platformsDescription}";
#endif
    }

#if !NET462
    private static bool MatchesCurrentPlatform(string platform)
    {
        int versionIndex = platform.IndexOfAny(['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']);
        bool matchesVersion;
        if (versionIndex >= 0 && Version.TryParse(platform[versionIndex..], out Version version))
        {
            platform = platform[..versionIndex];
            matchesVersion = MatchesCurrentVersion(version.Major, version.Minor, version.Build, version.Revision);
        }
        else
        {
            matchesVersion = true;
        }

        return matchesVersion && RuntimeInformation.IsOSPlatform(OSPlatform.Create(platform));
    }

    // Adapted from OperatingSystem.IsOSVersionAtLeast() which is private, see https://github.com/dotnet/runtime/blob/d6eb35426ebdb09ee5c754aa9afb9ad6e96a3dec/src/libraries/System.Private.CoreLib/src/System/OperatingSystem.cs#L326-L351
    private static bool MatchesCurrentVersion(int major, int minor, int build, int revision)
    {
        Version current = Environment.OSVersion.Version;

        if (current.Major != major)
        {
            return current.Major > major;
        }

        if (current.Minor != minor)
        {
            return current.Minor > minor;
        }

        // Unspecified build component is to be treated as zero
        int currentBuild = current.Build < 0 ? 0 : current.Build;
        build = build < 0 ? 0 : build;
        if (currentBuild != build)
        {
            return currentBuild > build;
        }

        // Unspecified revision component is to be treated as zero
        int currentRevision = current.Revision < 0 ? 0 : current.Revision;
        revision = revision < 0 ? 0 : revision;

        return currentRevision >= revision;
    }

    /// <summary>
    /// Returns the collection of platforms defined by the specified <paramref name="platformAttributeName"/> that decorate the test method and the test class.
    /// </summary>
    /// <param name="testMethod">The <see cref="ITestMethod"/>.</param>
    /// <param name="platformAttributeName">Either <c>SupportedOSPlatform</c> or <c>UnsupportedOSPlatform</c>.</param>
    /// <example>
    /// <para>
    /// Calling GetPlatforms(testMethod, "SupportedOSPlatform") where <paramref name="testMethod"/> represents <c>MyTest</c> returns ["Linux", "macOS"].
    /// </para>
    /// <code>
    /// [SupportedOSPlatform("macOS")]
    /// public class MyTests
    /// {
    ///     [SkippableFact]
    ///     [SupportedOSPlatform("Linux")]
    ///     public void MyTest()
    ///     {
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <returns>The collection of platforms defined by the specified <paramref name="platformAttributeName"/> that decorate the test method and the test class.</returns>
    private static HashSet<string> GetPlatforms(ITestMethod testMethod, string platformAttributeName)
    {
        string platformAttribute = $"System.Runtime.Versioning.{platformAttributeName}Attribute";
        var platforms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddPlatforms(platforms, testMethod.Method.GetCustomAttributes(platformAttribute));
        AddPlatforms(platforms, testMethod.Method.Type.GetCustomAttributes(platformAttribute));
        return platforms;
    }

    private static void AddPlatforms(HashSet<string> platforms, IEnumerable<IAttributeInfo> supportedPlatformAttributes)
    {
        foreach (IAttributeInfo supportedPlatformAttribute in supportedPlatformAttributes)
        {
            platforms.Add(supportedPlatformAttribute.GetNamedArgument<string>("PlatformName"));
        }
    }
#endif
}
