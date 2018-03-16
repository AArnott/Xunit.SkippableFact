namespace Xunit.SkippableFact.Tests
{
    [CollectionDefinition(nameof(SkipCollectionDefinition))]
    // ReSharper disable once InconsistentNaming
    [SkipTests("skipped by collection")]
    public class SkipCollectionDefinition
    {
    }
}