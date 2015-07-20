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
        internal SkippableTestMessageBus(IMessageBus inner)
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
