using Sudoku.ConsoleApp.Commands;
using Sudoku.ConsoleApp.Presentation;
using Sudoku.Core.Domain;

namespace Sudoku.Tests.ConsoleApp;

public class CommandParserTests
{
    private readonly CommandParser _parser = new();

    [Theory]
    [InlineData("B3 7")]
    [InlineData("b3 7")]
    [InlineData("  B3   7  ")]
    [InlineData("B3\t7")]
    [InlineData("﻿B3 7")]
    public void CellAndNumber_ParsesToPlaceNumber(string input)
    {
        Assert.Equal(new PlaceNumberCommand(new Position(1, 2), 7), _parser.Parse(input));
    }

    [Fact]
    public void OutOfRangeInteger_StillParses_SoTheBoardCanReject()
    {
        Assert.Equal(new PlaceNumberCommand(new Position(1, 2), 12), _parser.Parse("B3 12"));
    }

    [Theory]
    [InlineData("C5 clear")]
    [InlineData("c5 CLEAR")]
    public void CellAndClear_ParsesToClearCell(string input)
    {
        Assert.Equal(new ClearCellCommand(new Position(2, 4)), _parser.Parse(input));
    }

    [Theory]
    [InlineData("hint", typeof(HintCommand))]
    [InlineData("HINT", typeof(HintCommand))]
    [InlineData("check", typeof(CheckCommand))]
    [InlineData("quit", typeof(QuitCommand))]
    [InlineData("exit", typeof(QuitCommand))]
    public void Keyword_ParsesToMatchingCommand(string input, Type expected)
    {
        Assert.IsType(expected, _parser.Parse(input));
    }

    [Fact]
    public void NonNumericValue_ReportsNumberRange()
    {
        Assert.Equal(new InvalidInputCommand(Messages.NumberOutOfRange), _parser.Parse("B3 x"));
    }

    [Fact]
    public void InvalidCell_ReportsCellError()
    {
        Assert.Equal(new InvalidInputCommand(Messages.InvalidCell("Z9")), _parser.Parse("Z9 4"));
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("B3")]
    [InlineData("B3 4 5")]
    public void UnrecognisedInput_ReportsUnknownCommand(string input)
    {
        Assert.Equal(new InvalidInputCommand(Messages.UnknownCommand(input)), _parser.Parse(input));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void BlankInput_AsksForACommand(string? input)
    {
        Assert.Equal(new InvalidInputCommand(Messages.EmptyInput), _parser.Parse(input));
    }
}
