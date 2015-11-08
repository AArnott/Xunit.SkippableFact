namespace Xunit
{
    using System;
    using Sdk;

    /// <summary>
    /// Marks a test method as being a data theory. Data theories are tests which are
    /// fed various bits of data from a data source, mapping to parameters on the test
    /// method. If the data source contains multiple rows, then the test method is executed
    /// multiple times (once with each data row). Data is provided by attributes which
    /// derive from Xunit.Sdk.DataAttribute (notably, Xunit.InlineDataAttribute and Xunit.MemberDataAttribute).
    /// The test may produce a "skipped test" result by calling
    /// <see cref="Skip.If(bool, string)"/> or otherwise throwing a <see cref="SkipException"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Xunit.Sdk.SkippableTheoryDiscoverer", "Xunit.SkippableFact.{Platform}")]
    public class SkippableTheoryAttribute : TheoryAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkippableTheoryAttribute"/> class.
        /// </summary>
        /// <param name="skippingExceptions">
        /// Exception types that, if thrown, should cause the test to register as skipped.
        /// </param>
        public SkippableTheoryAttribute(params Type[] skippingExceptions)
        {
        }
    }
}
