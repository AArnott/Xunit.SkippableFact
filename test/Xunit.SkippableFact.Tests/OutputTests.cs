// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

using Xunit.Abstractions;

namespace Xunit.SkippableFact.Tests;

public class OutputTests(ITestOutputHelper output)
{
    [SkippableFact]
    public void OutputBeforeSkip()
    {
        output.WriteLine("Output written before skipping.");
        Skip.If(true, "Skipped after writing output.");
    }
}
