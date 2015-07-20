namespace Xunit.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;

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
            TestMethodDisplay defaultMethodDisplay = discoveryOptions.MethodDisplayOrDefault();

            var basis = this.theoryDiscoverer.Discover(discoveryOptions, testMethod, factAttribute);
            foreach (var testCase in basis)
            {
                if (testCase is XunitTheoryTestCase)
                {
                    yield return new SkippableTheoryTestCase(this.diagnosticMessageSink, defaultMethodDisplay, testCase.TestMethod);
                }
                else
                {
                    yield return new SkippableFactDiscoverer.SkippableFactTestCase(this.diagnosticMessageSink, defaultMethodDisplay, testCase.TestMethod, testCase.TestMethodArguments);
                }
            }
        }

        /// <summary>
        /// A theory test case that will wrap the message bus.
        /// </summary>
        private class SkippableTheoryTestCase : XunitTheoryTestCase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SkippableTheoryTestCase"/> class.
            /// </summary>
            /// <param name="diagnosticMessageSink">The diagnostic message sink.</param>
            /// <param name="defaultMethodDisplay">The preferred test name derivation.</param>
            /// <param name="testMethod">The test method.</param>
            public SkippableTheoryTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod)
                : base(diagnosticMessageSink, defaultMethodDisplay, testMethod)
            {
            }

            /// <inheritdoc />
            public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            {
                var messageBusInterceptor = new SkippableTestMessageBus(messageBus);
                var result = await base.RunAsync(diagnosticMessageSink, messageBusInterceptor, constructorArguments, aggregator, cancellationTokenSource);
                result.Failed -= messageBusInterceptor.SkippedCount;
                result.Skipped += messageBusInterceptor.SkippedCount;
                return result;
            }
        }
    }
}
