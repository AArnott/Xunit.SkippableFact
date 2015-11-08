// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

namespace Xunit.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Validation;

    /// <summary>
    /// Transforms <see cref="SkippableFactAttribute"/> test methods into test cases.
    /// </summary>
    public class SkippableFactDiscoverer : IXunitTestCaseDiscoverer
    {
        /// <summary>
        /// The diagnostic message sink provided to the constructor.
        /// </summary>
        private readonly IMessageSink diagnosticMessageSink;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkippableFactDiscoverer"/> class.
        /// </summary>
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
        public SkippableFactDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        /// <inheritdoc />
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            string[] skippingExceptionNames = GetSkippableExceptionNames(factAttribute);
            yield return new SkippableFactTestCase(skippingExceptionNames, this.diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
        }

        internal static string[] GetSkippableExceptionNames(IAttributeInfo factAttribute)
        {
            var firstArgument = (object[])factAttribute.GetConstructorArguments().FirstOrDefault();
            var skippingExceptions = firstArgument?.Cast<Type>().ToArray() ?? new Type[0];
            Array.Resize(ref skippingExceptions, skippingExceptions.Length + 1);
            skippingExceptions[skippingExceptions.Length - 1] = typeof(SkipException);

            var skippingExceptionNames = skippingExceptions.Select(ex => ex.FullName).ToArray();
            return skippingExceptionNames;
        }

        /// <summary>
        /// A test case that interprets <see cref="SkipException"/> as a <see cref="TestSkipped"/> result.
        /// </summary>
        internal class SkippableFactTestCase : XunitTestCase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SkippableFactTestCase"/> class,
            /// to be called only by the deserializer.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            [Obsolete("Called by the de-serializer", true)]
            public SkippableFactTestCase()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SkippableFactTestCase"/> class.
            /// </summary>
            /// <param name="skippingExceptionNames">An array of the full names of the exception types which should be interpreted as a skipped test-.</param>
            /// <param name="diagnosticMessageSink">The diagnostic message sink.</param>
            /// <param name="defaultMethodDisplay">The preferred test name derivation.</param>
            /// <param name="testMethod">The test method.</param>
            /// <param name="testMethodArguments">The test method arguments.</param>
            public SkippableFactTestCase(string[] skippingExceptionNames, IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, object[] testMethodArguments = null)
                : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, testMethodArguments)
            {
                Requires.NotNull(skippingExceptionNames, nameof(skippingExceptionNames));
                this.SkippingExceptionNames = skippingExceptionNames;
            }

            internal string[] SkippingExceptionNames { get; private set; }

            /// <inheritdoc />
            public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            {
                var messageBusInterceptor = new SkippableTestMessageBus(messageBus, this.SkippingExceptionNames);
                var result = await base.RunAsync(diagnosticMessageSink, messageBusInterceptor, constructorArguments, aggregator, cancellationTokenSource);
                result.Failed -= messageBusInterceptor.SkippedCount;
                result.Skipped += messageBusInterceptor.SkippedCount;
                return result;
            }

            public override void Serialize(IXunitSerializationInfo data)
            {
                base.Serialize(data);
                data.AddValue(nameof(this.SkippingExceptionNames), this.SkippingExceptionNames);
            }

            public override void Deserialize(IXunitSerializationInfo data)
            {
                base.Deserialize(data);
                this.SkippingExceptionNames = data.GetValue<string[]>(nameof(this.SkippingExceptionNames));
            }
        }
    }
}
