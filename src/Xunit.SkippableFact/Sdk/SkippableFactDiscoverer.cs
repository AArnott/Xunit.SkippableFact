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
    using Validation;
    using Xunit.Abstractions;

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
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages.</param>
        public SkippableFactDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        /// <inheritdoc />
        public virtual IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            Requires.NotNull(factAttribute, nameof(factAttribute));
            string[] skippingExceptionNames = GetSkippableExceptionNames(factAttribute);
            yield return new SkippableFactTestCase(skippingExceptionNames, this.diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
        }

        /// <summary>
        /// Translates the types of exceptions that should be considered as "skip" exceptions into their full names.
        /// </summary>
        /// <param name="factAttribute">The <see cref="SkippableFactAttribute"/>.</param>
        /// <returns>An array of full names of types.</returns>
        public static string[] GetSkippableExceptionNames(IAttributeInfo factAttribute)
        {
            object[]? firstArgument = (object[])factAttribute.GetConstructorArguments().FirstOrDefault();
            Type[]? skippingExceptions = firstArgument?.Cast<Type>().ToArray() ?? Type.EmptyTypes;
            Array.Resize(ref skippingExceptions, skippingExceptions.Length + 1);
            skippingExceptions[skippingExceptions.Length - 1] = typeof(SkipException);

            var skippingExceptionNames = skippingExceptions.Select(ex => ex.FullName).ToArray();
            return skippingExceptionNames;
        }

        /// <summary>
        /// A test case that interprets <see cref="SkipException"/> as a <see cref="TestSkipped"/> result.
        /// </summary>
        public class SkippableFactTestCase : XunitTestCase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SkippableFactTestCase"/> class,
            /// to be called only by the deserializer.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            [Obsolete("Called by the de-serializer", true)]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
            public SkippableFactTestCase()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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
            public SkippableFactTestCase(string[] skippingExceptionNames, IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, object[]? testMethodArguments = null)
#if NET45
                : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, testMethodArguments)
#else
                : base(diagnosticMessageSink, defaultMethodDisplay, TestMethodDisplayOptions.None, testMethod, testMethodArguments)
#endif
            {
                Requires.NotNull(skippingExceptionNames, nameof(skippingExceptionNames));
                this.SkippingExceptionNames = skippingExceptionNames;
            }

            /// <summary>
            /// Gets an array of full names to exception types that should be interpreted as a skip result.
            /// </summary>
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

            /// <inheritdoc/>
            public override void Serialize(IXunitSerializationInfo data)
            {
                base.Serialize(data);
                data.AddValue(nameof(this.SkippingExceptionNames), this.SkippingExceptionNames);
            }

            /// <inheritdoc/>
            public override void Deserialize(IXunitSerializationInfo data)
            {
                base.Deserialize(data);
                this.SkippingExceptionNames = data.GetValue<string[]>(nameof(this.SkippingExceptionNames));
            }
        }
    }
}
