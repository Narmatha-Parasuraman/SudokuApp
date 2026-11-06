using Sudoku.ConsoleApp.Presentation;
using Sudoku.Core.Domain;
using Sudoku.Tests.TestDoubles;

namespace Sudoku.Tests.ConsoleApp;

public class GridRendererTests
{
    [Fact]
    public void Render_MatchesProblemStatementLayout()
    {
        const string expected = """
                1 2 3 4 5 6 7 8 9
              A 5 3 _ _ 7 _ _ _ _
              B 6 _ _ 1 9 5 _ _ _
              C _ 9 8 _ _ _ _ 6 _
              D 8 _ _ _ 6 _ _ _ 3
              E 4 _ _ 8 _ 3 _ _ 1
              F 7 _ _ _ 2 _ _ _ 6
              G _ 6 _ _ _ _ 2 8 _
              H _ _ _ 4 1 9 _ _ 5
              I _ _ _ _ 8 _ _ 7 9
            """;

        var rendered = GridRenderer.Render(SudokuGrid.Parse(SamplePuzzle.Givens));

        Assert.Equal(expected.ReplaceLineEndings(), rendered.ReplaceLineEndings());
    }
}
