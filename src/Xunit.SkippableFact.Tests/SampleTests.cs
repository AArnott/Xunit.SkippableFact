namespace Xunit.SkippableFact.Tests
{
    public class SampleTests
    {
        [SkippableFact]
        public void SkipMe()
        {
            Skip.If(true);
        }

        [SkippableFact]
        public void DoNotSkipMe()
        {
        }
    }
}
