using Sudoku.ConsoleApp.Presentation;
using Sudoku.Core.Domain;

namespace Sudoku.ConsoleApp.Commands;

/// <summary>
/// Turns a line of user input into an <see cref="ICommand"/>. Input is case-insensitive and
/// surrounding or repeated whitespace is ignored. Unparseable input becomes an <see cref="InvalidInputCommand"/>,
/// so the caller never has to handle parse failures separately.
/// </summary>
public sealed class CommandParser
{
    private const string ClearKeyword = "clear";
    private const char ByteOrderMark = '﻿';

    private static readonly Dictionary<string, Func<ICommand>> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["hint"] = () => new HintCommand(),
        ["check"] = () => new CheckCommand(),
        ["quit"] = () => new QuitCommand(),
        ["exit"] = () => new QuitCommand(),
    };

    public ICommand Parse(string? input)
    {
        // Piped input (e.g. from PowerShell) can start with a byte-order mark, which is not whitespace.
        var text = (input ?? string.Empty).Replace(ByteOrderMark, ' ');
        var tokens = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        return tokens switch
        {
            [] => new InvalidInputCommand(Messages.EmptyInput),
            [var keyword] when Keywords.TryGetValue(keyword, out var create) => create(),
            [var cell, var argument] => ParseCellCommand(cell, argument),
            _ => new InvalidInputCommand(Messages.UnknownCommand(string.Join(' ', tokens))),
        };
    }

    private static ICommand ParseCellCommand(string cell, string argument)
    {
        if (!Position.TryParse(cell, out var position))
        {
            return new InvalidInputCommand(Messages.InvalidCell(cell));
        }

        if (argument.Equals(ClearKeyword, StringComparison.OrdinalIgnoreCase))
        {
            return new ClearCellCommand(position);
        }

        // The range 1–9 is a game rule enforced by the Board; here we only require an integer.
        return int.TryParse(argument, out var number)
            ? new PlaceNumberCommand(position, number)
            : new InvalidInputCommand(Messages.NumberOutOfRange);
    }
}
