// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

namespace Xunit.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Validation;

    /// <summary>
    /// Transforms <see cref="SkippableTheoryAttribute"/> test theories into test cases.
    /// </summary>
    public class SkippableTheoryDiscoverer : IXunitTestCaseDiscoverer
    {
        /// <summary>
        /// The diagnostic message sink provided to the constructor.
        /// </summary>
        private readonly IMessageSink diagnosticMessageSink;

        /// <summary>
        /// The complex theory discovery process that we wrap.
        /// </summary>
        private readonly TheoryDiscoverer theoryDiscoverer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkippableTheoryDiscoverer"/> class.
        /// </summary>
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
        public SkippableTheoryDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
            this.theoryDiscoverer = new TheoryDiscoverer(diagnosticMessageSink);
        }

        /// <inheritdoc />
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            string[] skippingExceptionNames = SkippableFactDiscoverer.GetSkippableExceptionNames(factAttribute);
            TestMethodDisplay defaultMethodDisplay = discoveryOptions.MethodDisplayOrDefault();

            var basis = this.theoryDiscoverer.Discover(discoveryOptions, testMethod, factAttribute);
            foreach (var testCase in basis)
            {
                if (testCase is XunitTheoryTestCase)
                {
                    yield return new SkippableTheoryTestCase(skippingExceptionNames, this.diagnosticMessageSink, defaultMethodDisplay, testCase.TestMethod);
                }
                else
                {
                    yield return new SkippableFactDiscoverer.SkippableFactTestCase(skippingExceptionNames, this.diagnosticMessageSink, defaultMethodDisplay, testCase.TestMethod, testCase.TestMethodArguments);
                }
            }
        }

        /// <summary>
        /// A theory test case that will wrap the message bus.
        /// </summary>
        private class SkippableTheoryTestCase : XunitTheoryTestCase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SkippableTheoryTestCase"/> class,
            /// to be called only by the deserializer.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            [Obsolete("Called by the de-serializer", true)]
            public SkippableTheoryTestCase()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SkippableTheoryTestCase"/> class.
            /// </summary>
            /// <param name="skippingExceptionNames">An array of the full names of the exception types which should be interpreted as a skipped test-.</param>
            /// <param name="diagnosticMessageSink">The diagnostic message sink.</param>
            /// <param name="defaultMethodDisplay">The preferred test name derivation.</param>
            /// <param name="testMethod">The test method.</param>
            public SkippableTheoryTestCase(string[] skippingExceptionNames, IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod)
                : base(diagnosticMessageSink, defaultMethodDisplay, testMethod)
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

            protected override string GetSkipReason(IAttributeInfo factAttribute)
            {
                var reason = this.TestMethod.GetSkipTestsReason();
                if (string.IsNullOrEmpty(reason))
                {
                    reason = base.GetSkipReason(factAttribute);
                    if (!string.IsNullOrEmpty(reason))
                    {
                        reason = $"Method: {reason}";
                    }
                }

                return reason;
            }
        }
    }
}
