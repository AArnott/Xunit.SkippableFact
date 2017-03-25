// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

namespace Xunit.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Abstractions;
    using Validation;

    /// <summary>
    /// Intercepts test results on the message bus and re-interprets
    /// <see cref="SkipException"/> as a <see cref="TestSkipped"/> result.
    /// </summary>
    internal class SkippableTestMessageBus : IMessageBus
    {
        /// <summary>
        /// The original message bus to which all messages should be forwarded.
        /// </summary>
        private readonly IMessageBus inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkippableTestMessageBus"/> class.
        /// </summary>
        /// <param name="inner">The original message bus to which all messages should be forwarded.</param>
        /// <param name="skippingExceptionNames">An array of the full names of the exception types which should be interpreted as a skipped test-.</param>
        internal SkippableTestMessageBus(IMessageBus inner, string[] skippingExceptionNames)
        {
            Requires.NotNull(inner, nameof(inner));
            Requires.NotNull(skippingExceptionNames, nameof(skippingExceptionNames));

            this.inner = inner;
            this.SkippingExceptionNames = skippingExceptionNames;
        }

        internal string[] SkippingExceptionNames { get; }

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
                var outerException = failed.ExceptionTypes.FirstOrDefault();
                if (outerException != null && Array.IndexOf(this.SkippingExceptionNames, outerException) >= 0)
                {
                    this.SkippedCount++;
                    return this.inner.QueueMessage(new TestSkipped(failed.Test, failed.Messages[0]));
                }
            }

            return this.inner.QueueMessage(message);
        }
    }
}
