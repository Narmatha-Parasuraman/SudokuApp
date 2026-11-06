# Sudoku (Command Line)

A command-line Sudoku game written in C#. Each game starts with a freshly generated puzzle that has
**30 pre-filled cells and exactly one solution**. Players can place and clear numbers, ask for hints,
check for rule violations, and quit at any time.

```
Welcome to Sudoku!

Here is your puzzle:
    1 2 3 4 5 6 7 8 9
  A 5 3 _ _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  ...
Enter command (e.g., A3 4, C5 clear, hint, check, quit):
```

## Requirements

| | |
|---|---|
| Runtime / SDK | [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later (8.0 is the target framework; any newer SDK can build it) |
| Operating system | Windows, Linux or macOS — anything the .NET 8 SDK supports. Developed and tested on Windows 11. |
| External libraries | None at runtime. Tests use xUnit (restored automatically from NuGet on first build). |

Check your SDK with `dotnet --list-sdks`.

## How to run

From the folder that contains `Sudoku.sln`:

```bash
dotnet run --project src/Sudoku.ConsoleApp
```

Optional: pass a seed to get a reproducible sequence of puzzles (useful for demos and bug reports):

```bash
dotnet run --project src/Sudoku.ConsoleApp -- --seed 42
```

To run the tests:

```bash
dotnet test
```

To build a standalone binary instead of using `dotnet run`:

```bash
dotnet publish src/Sudoku.ConsoleApp -c Release -o out
```

then run `out/Sudoku.ConsoleApp` (`out\Sudoku.ConsoleApp.exe` on Windows).

## Commands

Commands are case-insensitive and extra spaces are ignored.

| Command | Example | Effect |
|---|---|---|
| `<cell> <number>` | `B3 7` | Places 7 in row B, column 3. |
| `<cell> clear` | `C5 clear` | Empties a cell you filled. |
| `hint` | `hint` | Reveals the correct value for one cell (the grid is not changed). |
| `check` | `check` | Lists every duplicate in a row, column or 3×3 subgrid. |
| `quit` / `exit` | `quit` | Ends the game. |

Rows are `A`–`I` (top to bottom) and columns `1`–`9` (left to right).
When the grid is completely and correctly filled, the game congratulates you and starts a new puzzle after a key press.

## Design

The solution is split into a UI-independent domain library, a thin console application, and a test project.

```
src/
  Sudoku.Core/                 Game rules and algorithms. No console or I/O code.
    Domain/                    Position, SudokuGrid, Puzzle, Board, GridUnits
    Validation/                IGridValidator, GridValidator, RuleViolation
    Solving/                   ISudokuSolver, BacktrackingSolver
    Generation/                IPuzzleGenerator, RandomPuzzleGenerator
    Hints/                     IHintProvider, RandomHintProvider
  Sudoku.ConsoleApp/           Everything specific to playing in a terminal.
    Commands/                  ICommand + one class per command, CommandParser
    Game/                      SudokuGame (the game loop), GameSession
    Presentation/              GridRenderer, Messages (all user-facing text)
    IO/                        IConsole, SystemConsole
    Program.cs                 Composition root: wires concrete implementations together
tests/
  Sudoku.Tests/                Unit tests for every class plus end-to-end games driven through a fake console
```

### Key types

- **`Position`** – an immutable cell coordinate. Owns parsing and formatting of references like `B3`.
- **`SudokuGrid`** – a plain 9×9 store of digits. It does not enforce Sudoku rules, because the player
  is allowed to enter conflicting numbers and find them later with `check`.
- **`Puzzle`** – the givens plus the solution. Immutable; checks that the two are consistent.
- **`Board`** – the player's working copy of a puzzle. Enforces the *move* rules (pre-filled cells are locked and numbers must be 1–9)
  and reports the outcome as a `MoveResult` enum instead of throwing, since invalid moves are expected user behaviour.
- **`GridValidator`** – finds duplicates in rows, columns and boxes and returns them as `RuleViolation` values.
  Turning them into text is left to the presentation layer.
- **`BacktrackingSolver`** – depth-first search that always fills the cell with the fewest candidates first.
  Used to build complete grids and to count solutions (stopping at two) for the uniqueness check.
- **`RandomPuzzleGenerator`** – builds a random complete grid, then removes cells in random order,
  keeping a cell whenever removing it would allow a second solution, until 30 clues remain. If a grid cannot be reduced to 30 clues and stay unique, it starts again with a new grid.
- **Commands** – each user action is an `ICommand` that runs against a `GameSession` and returns a `CommandResult`
  (message, whether to redraw the grid, whether the game ends). `CommandParser` maps input text to commands and
  turns bad input into an `InvalidInputCommand`, so the game loop has no special cases.
- **`SudokuGame`** – the loop: generate, display, read, parse, execute, show, check for a win.

### SOLID in practice

- **Single responsibility** – parsing, rule checking, solving, generating, rendering and I/O each live in their own class.
- **Open/closed** – a new command is a new `ICommand` class plus one parser entry; the game loop does not change.
  A different generator, hint strategy or front end (GUI, web) can be added without touching `Sudoku.Core`.
- **Liskov / interface segregation** – small interfaces (`IReadOnlyGrid`, `IGridValidator`, `ISudokuSolver`,
  `IPuzzleGenerator`, `IHintProvider`, `IConsole`); callers that only read a grid receive `IReadOnlyGrid`.
- **Dependency inversion** – `SudokuGame` depends only on abstractions, which `Program.cs` supplies. This is also
  what lets the end-to-end tests run complete games with a scripted console, a fixed puzzle and a fixed hint.

### Testing

The suite (121 tests) covers:

- Unit tests for each domain class: parsing, move rules, validation, solving, uniqueness, generation (with fixed seeds so results are repeatable) and hints.
- Unit tests for the parser, each command and the grid renderer, including the exact grid layout from the problem statement.
- End-to-end tests that drive `SudokuGame` through `FakeConsole` and replay every example from the problem statement:
  accepted move, pre-filled cell, the three violation examples, hints, clearing, invalid input, quitting, and a complete winning game followed by a new round.

## Assumptions and decisions

1. **Conflicting moves are accepted.** The problem statement's examples accept `A3 3` and only report the clash on `check`, so `Board` only rejects
   moves into pre-filled cells and numbers outside 1–9.
2. **`check` reports every violation, one per line**, rows first, then columns, then subgrids. The examples are not consistent about which single violation they
   would report (for example, `B3 8` duplicates 8 in both column 3 and the subgrid), so listing all of them is the most useful and predictable choice. Each example's expected line is always included.
3. **`check` looks for rule violations only.** A number that breaks no rule but is not the solution value is not reported. Use `hint` to find mistakes.
4. **`hint` reveals but does not fill.** It shows `Hint: Cell E5 = 5` and leaves the move to the player. It picks a random empty cell; when no cells are empty
   it points to a wrongly filled cell instead. When everything is correct it says no hint is available.
5. **Winning** means every cell is filled and no rule is broken. Because generated puzzles have exactly one solution, this is the same as matching the solution.
6. **Puzzles have exactly 30 givens and a unique solution.** Difficulty is not graded.
7. **Play again:** after a win, any key starts a new puzzle. `quit`/`exit`, or closing the input stream (Ctrl+Z on Windows, Ctrl+D on Linux/macOS), ends the program.
8. **Invalid input** such as an unknown cell (`Z9 4`), a non-numeric value (`B3 x`) or an unknown word gets a short explanation and the game continues.
9. One message uses the `×` character, so `Program.cs` sets the console output encoding to UTF-8.
