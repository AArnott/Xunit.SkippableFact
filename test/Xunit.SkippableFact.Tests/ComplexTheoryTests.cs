// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL). See LICENSE.txt file in the project root for full license information.

namespace Xunit.SkippableFact.Tests;

public class ComplexTheoryTests
{
    public static IEnumerable<object[]> Packets =>
    [
        [new ValueType(0x0001)],
        [new ValueType(0x0002)]
    ];

    [SkippableTheory]
    [MemberData(nameof(Packets))]
    public void SkipsCorrectly(ValueType packetId)
    {
        Skip.If(packetId.Value == 0x0001, "Test packet ID to skip.");
    }

    public record struct ValueType(ushort Value);
}
