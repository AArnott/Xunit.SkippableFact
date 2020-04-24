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
#if NET45
                    yield return new SkippableTheoryTestCase(skippingExceptionNames, this.diagnosticMessageSink, defaultMethodDisplay, testCase.TestMethod);
#else
                    yield return new SkippableTheoryTestCase(skippingExceptionNames, this.diagnosticMessageSink, defaultMethodDisplay, discoveryOptions.MethodDisplayOptionsOrDefault(), testCase.TestMethod);
#endif
                }
                else
                {
#if NET45
                    yield return new SkippableFactTestCase(skippingExceptionNames, this.diagnosticMessageSink, defaultMethodDisplay, testCase.TestMethod, testCase.TestMethodArguments);
#else
                    yield return new SkippableFactTestCase(skippingExceptionNames, this.diagnosticMessageSink, defaultMethodDisplay, discoveryOptions.MethodDisplayOptionsOrDefault(), testCase.TestMethod, testCase.TestMethodArguments);
#endif
                }
            }
        }
    }
}
