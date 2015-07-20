// Copyright (c) 2015 Andrew Arnott
// Licensed under the Ms-PL

namespace Xunit.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
            yield return new SkippableFactTestCase(this.diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
        }

        /// <summary>
        /// A test case that interprets <see cref="SkipException"/> as a <see cref="TestSkipped"/> result.
        /// </summary>
        private class SkippableFactTestCase : XunitTestCase
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
            /// <param name="diagnosticMessageSink">The diagnostic message sink.</param>
            /// <param name="defaultMethodDisplay">The preferred test name derivation.</param>
            /// <param name="testMethod">The test method.</param>
            /// <param name="testMethodArguments">The test method arguments.</param>
            public SkippableFactTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, object[] testMethodArguments = null)
                : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, testMethodArguments)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SkippableFactTestCase"/> class.
            /// </summary>
            /// <param name="diagnosticMessageSink">The diagnostic message sink.</param>
            /// <param name="defaultMethodDisplay">The preferred test name derivation.</param>
            /// <param name="testMethod">The test method.</param>
            /// <param name="skipReason">The reason the test should be (statically) skipped.</param>
            public SkippableFactTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, string skipReason)
                : base(diagnosticMessageSink, defaultMethodDisplay, testMethod)
            {
                this.SkipReason = skipReason;
            }

            /// <inheritdoc />
            public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            {
                var messageBusInterceptor = new MessageBusInterceptor(messageBus);
                var result = await base.RunAsync(diagnosticMessageSink, messageBusInterceptor, constructorArguments, aggregator, cancellationTokenSource);
                result.Failed -= messageBusInterceptor.SkippedCount;
                result.Skipped += messageBusInterceptor.SkippedCount;
                return result;
            }

            /// <summary>
            /// Intercepts test results on the message bus and re-interprets
            /// <see cref="SkipException"/> as a <see cref="TestSkipped"/> result.
            /// </summary>
            private class MessageBusInterceptor : IMessageBus
            {
                /// <summary>
                /// The original message bus to which all messages should be forwarded.
                /// </summary>
                private readonly IMessageBus inner;

                /// <summary>
                /// Initializes a new instance of the <see cref="MessageBusInterceptor"/> class.
                /// </summary>
                /// <param name="inner">The original message bus to which all messages should be forwarded.</param>
                internal MessageBusInterceptor(IMessageBus inner)
                {
                    Requires.NotNull(inner, nameof(inner));

                    this.inner = inner;
                }

                /// <summary>
                /// Gets the number of tests that have been dynamically skipped.
                /// </summary>
                internal int SkippedCount { get; private set; }

                /// <summary>
                /// Disposes the inner message bus.
                /// </summary>
                public void Dispose()
                {
                    this.inner.Dispose();
                }

                /// <inheritdoc />
                public bool QueueMessage(IMessageSinkMessage message)
                {
                    var failed = message as TestFailed;
                    if (failed != null)
                    {
                        if (failed.ExceptionTypes.Length == 1 && failed.ExceptionTypes[0] == typeof(SkipException).FullName)
                        {
                            this.SkippedCount++;
                            return this.inner.QueueMessage(new TestSkipped(failed.Test, failed.Messages[0]));
                        }
                    }

                    return this.inner.QueueMessage(message);
                }
            }
        }
    }
}
