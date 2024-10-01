// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

using System;
using Xunit.Sdk;

namespace Xunit;

/// <summary>
/// Attribute that is applied to a method to indicate that it is a fact that should
/// be run by the test runner.
/// The test may produce a "skipped test" result by calling
/// <see cref="Skip.If(bool, string)"/> or otherwise throwing a <see cref="SkipException"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("Xunit.Sdk.SkippableFactDiscoverer", ThisAssembly.AssemblyName)]
public class SkippableFactAttribute : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkippableFactAttribute"/> class.
    /// </summary>
    /// <param name="skippingExceptions">
    /// Exception types that, if thrown, should cause the test to register as skipped.
    /// </param>
#pragma warning disable CA1801 // Review unused parameters - they are used via reflection elsewhere.
    public SkippableFactAttribute(params Type[] skippingExceptions)
#pragma warning restore CA1801 // Review unused parameters
    {
    }
}
