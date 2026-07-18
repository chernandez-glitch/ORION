using FluentAssertions;
using Orion.Application.Commands;
using Xunit;

namespace Orion.Tests.Application;

public sealed class CommandLineParserTests
{
    [Fact]
    public void Tokenize_ShouldSplitOnWhitespace()
    {
        var tokens = CommandLineParser.Tokenize("abrir-app notepad");

        tokens.Should().Equal("abrir-app", "notepad");
    }

    [Fact]
    public void Tokenize_ShouldHonorQuotesForPathsWithSpaces()
    {
        var tokens = CommandLineParser.Tokenize("abrir-carpeta \"C:\\Mis Documentos\"");

        tokens.Should().Equal("abrir-carpeta", "C:\\Mis Documentos");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Tokenize_ShouldReturnEmptyForBlankInput(string input)
    {
        CommandLineParser.Tokenize(input).Should().BeEmpty();
    }

    [Fact]
    public void Tokenize_ShouldCollapseMultipleSpaces()
    {
        CommandLineParser.Tokenize("a    b").Should().Equal("a", "b");
    }
}
