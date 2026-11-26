using SportEvents.Core.Parsing;

namespace SportEvents.Tests;

public sealed class RegistrationLineParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("MARATHON-2026;2")]
    [InlineData("MARATHON-2026;0;150.00")]
    [InlineData("MARATHON-2026;-3;150.00")]
    [InlineData("MARATHON-2026;501;150.00")]
    [InlineData("MARATHON-2026;2;-10.50")]
    public void TryParse_InvalidInput_ReturnsFalse(string raw)
    {
        bool ok = RegistrationLineParser.TryParse(raw, out ParsedRegistrationLine? line);

        Assert.False(ok);
        Assert.Null(line);
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsParsedLine()
    {
        bool ok = RegistrationLineParser.TryParse("MARATHON-2026;3;250.50", out ParsedRegistrationLine? line);

        Assert.True(ok);
        Assert.NotNull(line);
        Assert.Equal("MARATHON-2026", line.EventCode);
        Assert.Equal(3, line.ParticipantsCount);
        Assert.Equal(250.50m, line.EntryFee);
    }
}
