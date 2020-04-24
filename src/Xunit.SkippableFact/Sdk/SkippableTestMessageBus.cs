// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

namespace Xunit.Sdk
{
    using System;
    using System.Linq;
    using Validation;
    using Xunit.Abstractions;

    /// <summary>
    /// Intercepts test results on the message bus and re-interprets
    /// <see cref="SkipException"/> as a <see cref="TestSkipped"/> result.
    /// </summary>
    public class SkippableTestMessageBus : IMessageBus
    {
        /// <summary>
        /// The original message bus to which all messages should be forwarded.
        /// </summary>
        private readonly IMessageBus inner;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkippableTestMessageBus"/> class.
        /// </summary>
        /// <param name="inner">The original message bus to which all messages should be forwarded.</param>
        /// <param name="skippingExceptionNames">An array of the full names of the exception types which should be interpreted as a skipped test-.</param>
        public SkippableTestMessageBus(IMessageBus inner, string[] skippingExceptionNames)
        {
            Requires.NotNull(inner, nameof(inner));
            Requires.NotNull(skippingExceptionNames, nameof(skippingExceptionNames));

            this.inner = inner;
            this.SkippingExceptionNames = skippingExceptionNames;
        }

        /// <summary>
        /// Gets the number of tests that have been dynamically skipped.
        /// </summary>
        public int SkippedCount { get; private set; }

        /// <summary>
        /// Gets an array of full names to exception types that should be interpreted as a skip result.
        /// </summary>
        internal string[] SkippingExceptionNames { get; }

        /// <summary>
        /// Disposes the inner message bus.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public bool QueueMessage(IMessageSinkMessage message)
        {
            if (message is TestFailed failed)
            {
                var outerException = failed.ExceptionTypes.FirstOrDefault();
                bool skipTest = false;
                string? skipReason = null;
                switch (outerException)
                {
                    case string _ when this.ShouldSkipException(outerException):
                        skipTest = true;
                        skipReason = failed.Messages?.FirstOrDefault();
                        break;
                    case "Xunit.Sdk.ThrowsException" when failed.ExceptionTypes.Length > 1:
                        outerException = failed.ExceptionTypes[1];
                        if (this.ShouldSkipException(outerException))
                        {
                            skipTest = true;
                            skipReason = failed.Messages?.Length > 1 ? failed.Messages[1] : null;
                        }

                        break;
                }

                if (skipTest)
                {
                    this.SkippedCount++;
                    return this.inner.QueueMessage(new TestSkipped(failed.Test, skipReason));
                }
            }

            return this.inner.QueueMessage(message);
        }

        /// <summary>
        /// The bulk of the clean-up code is implemented in Dispose(bool).
        /// </summary>
        /// <param name="disposing">If the managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // free managed resources
                this.inner.Dispose();
            }

            this.isDisposed = true;
        }

        private bool ShouldSkipException(string exceptionType) =>
            Array.IndexOf(this.SkippingExceptionNames, exceptionType) >= 0;
    }
}
