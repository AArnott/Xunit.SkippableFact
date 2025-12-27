namespace Xunit.SkippableFact.Tests;

public class ComplexTheoryTests
{
    public struct ValueType(ushort value)
    {
        public ushort Value { get; set; } = value;
    }

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
}
