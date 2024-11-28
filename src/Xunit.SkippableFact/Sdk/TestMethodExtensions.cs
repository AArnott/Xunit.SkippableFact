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
    public static string? GetPlatformSkipReason(this ITestMethod testMethod)
    {
#if NET462
        return null;
#else
        var platforms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddPlatforms(platforms, testMethod.Method.GetCustomAttributes("System.Runtime.Versioning.SupportedOSPlatformAttribute"));
        AddPlatforms(platforms, testMethod.Method.Type.GetCustomAttributes("System.Runtime.Versioning.SupportedOSPlatformAttribute"));

        if (platforms.Count == 0 || platforms.Any(platform => RuntimeInformation.IsOSPlatform(OSPlatform.Create(platform))))
        {
            return null;
        }

        string platformsDescription = platforms.Count == 1 ? platforms.First() : string.Join(", ", platforms.Reverse().Skip(1).Reverse()) + " and " + platforms.Last();
        return $"Only supported on {platformsDescription}";
#endif
    }

#if !NET462
    private static void AddPlatforms(HashSet<string> platforms, IEnumerable<IAttributeInfo> supportedPlatformAttributes)
    {
        foreach (IAttributeInfo supportedPlatformAttribute in supportedPlatformAttributes)
        {
            platforms.Add(supportedPlatformAttribute.GetNamedArgument<string>("PlatformName"));
        }
    }
#endif
}
