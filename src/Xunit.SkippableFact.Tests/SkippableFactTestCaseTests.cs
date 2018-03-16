using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.SkippableFact.Tests
{
    public class SkippableFactTestCaseTests
    {
        private static readonly DiscoveryOptions DefaultDiscoveryOptions = new DiscoveryOptions();
        private static readonly SkippableFactDiscoverer Discoverer = GetDiscoverer();
        private static readonly Type FactAttributeType = typeof(FactAttribute);

        public static IEnumerable<object[]> MethodSkipExpectations
        {
            get
            {
                yield return new object[] { typeof(SkippableFactTestCaseTests), nameof(SkipFact), null, "Method: method skip" };
                yield return new object[] { typeof(SkippableFactTestCaseTests), nameof(SkipTheory), null, "Method: method skip" };
                yield return new object[] { typeof(SkippableFactTestCaseTests), nameof(SkipTheoryWithNonSerializableArguments), null, "Method: method skip" };
                yield return new object[] { typeof(SkipClassTests), nameof(SkipClassTests.SkipFactWithMethodSkip), null, "Class: skipped by class" };
                yield return new object[] { typeof(SkipClassTests), nameof(SkipClassTests.SkipTheoryWithMethodSkip), null, "Class: skipped by class" };
                yield return new object[] { typeof(SkipClassTests), nameof(SkipClassTests.SkipTheoryWithNonSerializableArgumentsWithMethodSkip), null, "Class: skipped by class" };
                yield return new object[] { typeof(SkipClassTests), nameof(SkipClassTests.SkipFactWithMethodSkip), typeof(SkipCollectionDefinition), "Collection: skipped by collection" };
                yield return new object[] { typeof(SkipClassTests), nameof(SkipClassTests.SkipTheoryWithMethodSkip), typeof(SkipCollectionDefinition), "Collection: skipped by collection" };
                yield return new object[] { typeof(SkipClassTests), nameof(SkipClassTests.SkipTheoryWithNonSerializableArgumentsWithMethodSkip), typeof(SkipCollectionDefinition), "Collection: skipped by collection" };
                yield return new object[] { typeof(SkipAssemblyTests.SkipAssemblyTests), nameof(SkipAssemblyTests.SkipAssemblyTests.SkipFactWithMethodSkip), typeof(SkipCollectionDefinition), "Assembly: skipped by assembly" };
                yield return new object[] { typeof(SkipAssemblyTests.SkipAssemblyTests), nameof(SkipAssemblyTests.SkipAssemblyTests.SkipTheoryWithMethodSkip), typeof(SkipCollectionDefinition), "Assembly: skipped by assembly" };
                yield return new object[] { typeof(SkipAssemblyTests.SkipAssemblyTests), nameof(SkipAssemblyTests.SkipAssemblyTests.SkipTheoryWithNonSerializableArgumentsWithMethodSkip), typeof(SkipCollectionDefinition), "Assembly: skipped by assembly" };
            }
        }

        [SkippableTheory]
        [MemberData(nameof(MethodSkipExpectations))]
        public void GivenTest_SkipReason_Is_Expected(Type testClass, string methodName, Type collectionType,
            string expected)
        {
            // Arrange
            var methodInfo = GetMethodInfo(testClass, methodName);
            var testMethod = GetTestMethod(testClass, methodInfo, collectionType);
            var factAttribute = GetFactAttribute(methodInfo);
            var testCase = Discoverer.Discover(DefaultDiscoveryOptions, testMethod, factAttribute).Single();

            // Act
            var skipReason = testCase.SkipReason;

            // Asssert
            Assert.Equal(expected, skipReason);
        }

        [SkippableFact(Skip = "method skip")]
        public void SkipFact()
        {
        }

        [SkippableTheory(Skip = "method skip")]
        [InlineData("should have been skipped")]
        public void SkipTheory(string message)
        {
        }

        public static IEnumerable<object[]> NonSerializableMemberData
        {
            get
            {
                yield return new object[] { (Func<string>)(() => "should have been skipped") };
            }
        }

        [SkippableTheory(Skip = "method skip")]
        [MemberData(nameof(NonSerializableMemberData))]
        public void SkipTheoryWithNonSerializableArguments(Func<string> message)
        {
        }

        private static ReflectionTypeInfo GetCollectionTypeInfo(Type collectionType)
        {
            var retval = collectionType == null ? null : new ReflectionTypeInfo(collectionType);
            return retval;
        }

        private static SkippableFactDiscoverer GetDiscoverer()
        {
            var diagnosticMessageSink = new NullMessageSink();
            var retval = new SkippableFactDiscoverer(diagnosticMessageSink);
            return retval;
        }

        private static IAttributeInfo GetFactAttribute(ReflectionMethodInfo methodInfo)
        {
            var customAttributes = methodInfo.GetCustomAttributes(FactAttributeType);
            var retval = customAttributes.Single();
            return retval;
        }

        private static ReflectionMethodInfo GetMethodInfo(Type testClass, string methodName)
        {
            var method = testClass.GetMethod(methodName);
            var retval = new ReflectionMethodInfo(method);
            return retval;
        }

        private static TestAssembly GetTestAssembly(Type testClass)
        {
            var assembly = testClass.GetTypeInfo().Assembly;
            var reflectionAssemblyInfo = new ReflectionAssemblyInfo(assembly);
            var retval = new TestAssembly(reflectionAssemblyInfo);
            return retval;
        }


        private static TestClass GetTestClass(Type testClass, Type collectionType)
        {
            var testCollection = GetTestCollection(testClass, collectionType);
            var typeInfo = new ReflectionTypeInfo(testClass);
            var retval = new TestClass(testCollection, typeInfo);
            return retval;
        }

        private static TestCollection GetTestCollection(Type testClass, Type collectionType)
        {
            var reflectionTypeInfo = GetCollectionTypeInfo(collectionType);
            var testAssembly = GetTestAssembly(testClass);
            var retval = new TestCollection(testAssembly, reflectionTypeInfo, "test collection displayName");
            return retval;
        }

        private static TestMethod GetTestMethod(Type testClass, IMethodInfo methodInfo, Type collectionType)
        {
            var @class = GetTestClass(testClass, collectionType);
            var retval = new TestMethod(@class, methodInfo);
            return retval;
        }

        private class DiscoveryOptions : ITestFrameworkDiscoveryOptions
        {
            public TValue GetValue<TValue>(string name)
            {
                return default(TValue);
            }

            public void SetValue<TValue>(string name, TValue value)
            {
                throw new NotImplementedException();
            }
        }
    }
}
