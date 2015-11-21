namespace Xunit.SkippableFact.Tests
{
    public class SkipTests
    {
        [Fact]
        public void If()
        {
            Assert.Throws<SkipException>(() => Skip.If(true));
            Skip.If(false);
        }

        [Fact]
        public void If_WithReason()
        {
            string reason = "some reason";
            try
            {
                Skip.If(true, reason);
                Assert.True(false, "If should have thrown.");
            }
            catch (SkipException ex)
            {
                Assert.Equal(reason, ex.Message);
            }
        }

        [Fact]
        public void IfNot()
        {
            Assert.Throws<SkipException>(() => Skip.IfNot(false));
            Skip.IfNot(true);
        }

        [Fact]
        public void IfNot_WithReason()
        {
            string reason = "some reason";
            try
            {
                Skip.IfNot(false, reason);
                Assert.True(false, "IfNot should have thrown.");
            }
            catch (SkipException ex)
            {
                Assert.Equal(reason, ex.Message);
            }
        }
    }
}
