using System;
using System.Collections.Generic;

namespace Xunit.SkippableFact.Tests
{
    [SkipTests("skipped by class")]
    public class SkipClassTests
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

        [SkippableFact(Skip = "method skip")]
        public void SkipFactWithMethodSkip()
        {
            throw new Exception("should have been skipped");
        }

        [SkippableTheory(Skip = "method skip")]
        [InlineData("should have been skipped")]
        public void SkipTheoryWithMethodSkip(string message)
        {
            throw new Exception(message);
        }

        [SkippableTheory(Skip = "method skip")]
        [MemberData(nameof(NonSerializableMemberData))]
        public void SkipTheoryWithNonSerializableArgumentsWithMethodSkip(Func<string> message)
        {
            throw new Exception(message());
        }
    }
}