// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

namespace Xunit.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Validation;
    using Xunit.Abstractions;

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
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages.</param>
        public SkippableTheoryDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
            this.theoryDiscoverer = new TheoryDiscoverer(diagnosticMessageSink);
        }

        /// <inheritdoc />
        public virtual IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            Requires.NotNull(factAttribute, nameof(factAttribute));
            string[] skippingExceptionNames = SkippableFactDiscoverer.GetSkippableExceptionNames(factAttribute);
            TestMethodDisplay defaultMethodDisplay = discoveryOptions.MethodDisplayOrDefault();

            IEnumerable<IXunitTestCase>? basis = this.theoryDiscoverer.Discover(discoveryOptions, testMethod, factAttribute);
            foreach (IXunitTestCase? testCase in basis)
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
        public class SkippableTheoryTestCase : XunitTheoryTestCase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SkippableTheoryTestCase"/> class,
            /// to be called only by the deserializer.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            [Obsolete("Called by the de-serializer", true)]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
            public SkippableTheoryTestCase()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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
#if NET45
                : base(diagnosticMessageSink, defaultMethodDisplay, testMethod)
#else
                : base(diagnosticMessageSink, defaultMethodDisplay, TestMethodDisplayOptions.None, testMethod)
#endif
            {
                Requires.NotNull(skippingExceptionNames, nameof(skippingExceptionNames));
                this.SkippingExceptionNames = skippingExceptionNames;
            }

            internal string[] SkippingExceptionNames { get; private set; }

            /// <inheritdoc />
            public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            {
                var messageBusInterceptor = new SkippableTestMessageBus(messageBus, this.SkippingExceptionNames);
                RunSummary? result = await base.RunAsync(diagnosticMessageSink, messageBusInterceptor, constructorArguments, aggregator, cancellationTokenSource).ConfigureAwait(false);
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
