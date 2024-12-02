// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

using System.Runtime.Versioning;
using System.Security.Cryptography;
using Xunit;

public class GettingStarted
{
    #region RuntimeCheck
    [SkippableFact]
    public void RuntimeCheck()
    {
        Skip.IfNot(Environment.GetEnvironmentVariable("RunThisTest") == "true");
    }
    #endregion

    #region ThrownExceptions
    [SkippableFact(typeof(NotSupportedException), typeof(NotImplementedException))]
    public void TestFunctionalityWhichIsNotSupportedOnSomePlatforms()
    {
        // Test functionality. If it throws any of the exceptions listed in the attribute,
        // a skip result is reported instead of a failure.
    }
    #endregion

    #region OSCheck
    public class AnyTestClass
    {
        [SkippableFact]
        [SupportedOSPlatform("Windows")]
        public void TestCngKey()
        {
            var key = CngKey.Create(CngAlgorithm.Rsa);
            Assert.NotNull(key);
        }
    }

    [SupportedOSPlatform("Windows")]
    public class WindowsOnlyTestClass
    {
        [SkippableFact]
        public void SomeTest()
        {
            // This test will only run on Windows.
        }
    }
    #endregion
}
