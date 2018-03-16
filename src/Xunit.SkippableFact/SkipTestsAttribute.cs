// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

namespace Xunit
{
    using System;

    /// <summary>
    /// Skips all tests contained in class, collection or assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    public class SkipTestsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkipTestsAttribute"/> class.
        /// </summary>
        /// <param name="reason">
        /// The reason the tests are being skipped
        /// </param>
        // ReSharper disable once UnusedParameter.Local
        public SkipTestsAttribute(string reason)
        {
        }
    }
}