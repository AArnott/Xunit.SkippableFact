// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

namespace Xunit
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Static methods for dynamically skipping tests identified with
    /// the <see cref="SkippableFactAttribute"/>.
    /// </summary>
    public static class Skip
    {
        /// <summary>
        /// Throws an exception that results in a "Skipped" result for the test.
        /// </summary>
        /// <param name="condition">The condition that must evaluate to <c>true</c> for the test to be skipped.</param>
        /// <param name="reason">The explanation for why the test is skipped.</param>
        public static void If(
            [DoesNotReturnIf(true)] bool condition,
            string? reason = null)
        {
            if (condition)
            {
                throw new SkipException(reason);
            }
        }

        /// <summary>
        /// Throws an exception that results in a "Skipped" result for the test.
        /// </summary>
        /// <param name="condition">The condition that must evaluate to <c>false</c> for the test to be skipped.</param>
        /// <param name="reason">The explanation for why the test is skipped.</param>
        public static void IfNot(
            [DoesNotReturnIf(false)] bool condition,
            string? reason = null)
        {
            Skip.If(!condition, reason);
        }
    }
}
