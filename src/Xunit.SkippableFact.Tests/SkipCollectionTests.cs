using System;
using System.Collections.Generic;

namespace Xunit.SkippableFact.Tests
{
    [Collection(nameof(SkipCollectionDefinition))]
    public class SkipCollectionTests
    {
        [SkippableFact]
        public void SkipFact()
        {
            throw new Exception("should have been skipped");
        }

        [SkippableTheory]
        [InlineData("should have been skipped")]
        public void SkipTheory(string message)
        {
            throw new Exception(message);
        }

        public static IEnumerable<object[]> NonSerializableMemberData
        {
            get
            {
                yield return new object[] { (Func<string>)(() => "should have been skipped") };
            }
        }

        [SkippableTheory]
        [MemberData(nameof(NonSerializableMemberData))]
        public void SkipTheoryWithNonSerializableArguments(Func<string> message)
        {
            throw new Exception(message());
        }
    }
}