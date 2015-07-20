namespace Xunit.SkippableFact.Tests
{
    public class SampleTests
    {
        [SkippableFact]
        public void SkipMe()
        {
            Skip.If(true, "Because it's a sample.");
        }

        [SkippableFact]
        public void DoNotSkipMe()
        {
        }

        [SkippableTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void SkipTheoryMaybe(bool skip)
        {
            Skip.If(skip, "I was told to.");
        }
    }
}
