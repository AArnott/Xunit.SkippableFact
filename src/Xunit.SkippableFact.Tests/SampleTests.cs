using System;

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

        [SkippableFact(typeof(NotImplementedException))]
        public void SkipByOtherException()
        {
            throw new NotImplementedException();
        }

        [SkippableFact(typeof(NotImplementedException), typeof(NotSupportedException))]
        public void SkipByOtherException_Ex2()
        {
            throw new NotSupportedException();
        }

        [SkippableFact(typeof(NotImplementedException))]
        public void SkipByOtherException_NotSkipped()
        {
        }

        [SkippableFact(typeof(NotSupportedException))]
        public void SkipByOtherException_Nested()
        {
            try
            {
                throw new InvalidOperationException();
            }
            catch (InvalidOperationException ex)
            {
                throw new NotSupportedException(ex.Message, ex);
            }
        }

        [SkippableTheory(typeof(NotImplementedException))]
        [InlineData(true)]
        [InlineData(false)]
        public void SkipTheoryMaybe_OtherExceptions(bool skip)
        {
            if (skip)
            {
                throw new NotImplementedException();
            }
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
