// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Validation;
using Xunit.Abstractions;

namespace Xunit.Sdk;

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

#if !NET45
    /// <summary>
    /// Initializes a new instance of the <see cref="SkippableTheoryTestCase"/> class.
    /// </summary>
    /// <param name="skippingExceptionNames">An array of the full names of the exception types which should be interpreted as a skipped test-.</param>
    /// <param name="diagnosticMessageSink">The diagnostic message sink.</param>
    /// <param name="defaultMethodDisplay">The preferred test name derivation.</param>
    /// <param name="defaultMethodDisplayOptions">Default method display options to use (when not customized).</param>
    /// <param name="testMethod">The test method.</param>
    public SkippableTheoryTestCase(string[] skippingExceptionNames, IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, TestMethodDisplayOptions defaultMethodDisplayOptions, ITestMethod testMethod)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
    {
        Requires.NotNull(skippingExceptionNames, nameof(skippingExceptionNames));
        this.SkippingExceptionNames = skippingExceptionNames;
    }
#endif

    /// <summary>
    /// Gets an array of the full names of the exception types which should be interpreted as a skipped test.
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
        Requires.NotNull(data, nameof(data));
        base.Serialize(data);
        data.AddValue(nameof(this.SkippingExceptionNames), this.SkippingExceptionNames);
    }

    /// <inheritdoc/>
    public override void Deserialize(IXunitSerializationInfo data)
    {
        Requires.NotNull(data, nameof(data));
        base.Deserialize(data);
        this.SkippingExceptionNames = data.GetValue<string[]>(nameof(this.SkippingExceptionNames));
    }
}
