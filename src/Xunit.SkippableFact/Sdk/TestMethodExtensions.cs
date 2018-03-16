// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

namespace Xunit.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;

    internal static class TestMethodExtensions
    {
        public static string GetSkipTestsReason(this ITestMethod testMethod)
        {
            var reason = GetReason(
                testMethod.TestClass.Class.Assembly.GetCustomAttributes,
                "Assembly");

            if (string.IsNullOrEmpty(reason) && testMethod.TestClass.TestCollection.CollectionDefinition != null)
            {
                reason = GetReason(
                    testMethod.TestClass.TestCollection.CollectionDefinition.GetCustomAttributes,
                    "Collection");
            }

            if (string.IsNullOrEmpty(reason))
            {
                reason = GetReason(testMethod.TestClass.Class.GetCustomAttributes, "Class");
            }

            return reason;
        }

        private static string GetReason(Func<Type, IEnumerable<IAttributeInfo>> customAttributes, string source)
        {
            var skipTestsAttribute = customAttributes(typeof(SkipTestsAttribute)).SingleOrDefault();
            if (skipTestsAttribute != null)
            {
                var reason = (string)skipTestsAttribute.GetConstructorArguments().Single();
                return $"{source}: {reason}";
            }

            return null;
        }
    }
}