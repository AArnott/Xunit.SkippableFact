# Xunit v3 Migration

Xunit v3 has skipping built-in, so there is no need for Xunit.SkippableFact.
As part of upgrading from Xunit v2 to v3, you should remove your package reference on Xunit.SkippableFact.

The following provides a guide for switching your use of Xunit.SkippableFact to Xunit v3 when you update your Xunit dependency.

Xunit.SkippableFact API | Xunit v3 API
--|--
@Xunit.Skip.If* | Assert.SkipWhen
@Xunit.Skip.IfNot* | Assert.SkipUnless
@Xunit.SkipException | Xunit.Sdk.SkipException
@Xunit.SkippableFactAttribute | Xunit.FactAttribute[^1]
@Xunit.SkippableTheoryAttribute | Xunit.TheoryAttribute[^1]

[^1]: Xunit's built-in attributes are sufficient. Although they lack the ability to take a list of exception types that should produce a Skipped result. This [may come in the future](https://github.com/xunit/xunit/issues/3101).
