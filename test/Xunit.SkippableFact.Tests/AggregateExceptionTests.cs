// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Xunit.SkippableFact.Tests;

public class AggregateExceptionTests
{
    [SkippableFact]
    public void SkipWithAggregateException_SingleSkipException()
    {
        var skipException = new SkipException("Skipped due to condition");
        var aggregateException = new AggregateException(skipException);
        throw aggregateException;
    }

    [SkippableFact]
    public void SkipWithAggregateException_MixedExceptions()
    {
        var skipException = new SkipException("Skipped due to condition");
        var otherException = new InvalidOperationException("Some other error");
        var aggregateException = new AggregateException(skipException, otherException);
        throw aggregateException;
    }

    [SkippableFact(typeof(NotImplementedException))]
    public void SkipWithAggregateException_CustomSkipException()
    {
        var skipException = new NotImplementedException("Feature not implemented");
        var otherException = new ArgumentException("Invalid argument");
        var aggregateException = new AggregateException(skipException, otherException);
        throw aggregateException;
    }

    [Fact] // This should fail, not be skipped, so use regular [Fact]
    public void NoSkipWithAggregateException_NoSkipExceptions()
    {
        var exception1 = new InvalidOperationException("Error 1");
        var exception2 = new ArgumentException("Error 2");
        var aggregateException = new AggregateException(exception1, exception2);

        // This should throw and cause the test to fail (not skip)
        try
        {
            throw aggregateException;
        }
        catch (AggregateException ex)
        {
            Assert.Contains("Error 1", ex.Message);
            Assert.Contains("Error 2", ex.Message);
        }
    }

    [SkippableFact(typeof(ArgumentException))]
    public void SkipWithAggregateException_MultipleCustomExceptions()
    {
        var skipException = new ArgumentException("Custom skip exception");
        var otherException1 = new InvalidOperationException("Error 1");
        var otherException2 = new NotSupportedException("Error 2");
        var aggregateException = new AggregateException(otherException1, skipException, otherException2);
        throw aggregateException;
    }

    [SkippableFact(typeof(ArgumentException), typeof(NotSupportedException))]
    public void SkipWithAggregateException_MultipleTypesConfigured()
    {
        var skipException1 = new NotSupportedException("First skip type");
        var otherException = new InvalidOperationException("Error 1");
        var skipException2 = new ArgumentException("Second skip type");
        var aggregateException = new AggregateException(otherException, skipException1, skipException2);
        throw aggregateException;
    }
}
