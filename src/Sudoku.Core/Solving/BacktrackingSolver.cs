using Sudoku.Core.Domain;

namespace Sudoku.Core.Solving;

public sealed class BacktrackingSolver : ISudokuSolver
{
    private const int CellCount = SudokuGrid.Size * SudokuGrid.Size;
    private const int NoCell = -1;

    private static readonly int[][] PeerIndexes = Position.All
        .Select(position => GridUnits.PeersOf(position).Select(ToIndex).ToArray())
        .ToArray();

    private readonly Random? _random;

    public BacktrackingSolver(Random? random = null)
    {
        _random = random;
    }

    public SudokuGrid? Solve(IReadOnlyGrid grid)
    {
        var cells = ToCells(grid);
        if (HasConflicts(cells))
        {
            return null;
        }

        int[]? solution = null;
        Search(cells, limit: 1, ref solution);
        return solution is null ? null : ToGrid(solution);
    }

    public int CountSolutions(IReadOnlyGrid grid, int limit)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);

        var cells = ToCells(grid);
        if (HasConflicts(cells))
        {
            return 0;
        }

        int[]? ignored = null;
        return Search(cells, limit, ref ignored);
    }

    private int Search(int[] cells, int limit, ref int[]? firstSolution)
    {
        var cell = FindMostConstrainedEmptyCell(cells, out var candidates);
        if (cell == NoCell)
        {
            firstSolution ??= (int[])cells.Clone();
            return 1;
        }

        var found = 0;
        foreach (var digit in OrderCandidates(candidates))
        {
            cells[cell] = digit;
            found += Search(cells, limit - found, ref firstSolution);
            if (found >= limit)
            {
                break;
            }
        }

        cells[cell] = SudokuGrid.Empty;
        return found;
    }

    
    private static int FindMostConstrainedEmptyCell(int[] cells, out int candidateMask)
    {
        var bestCell = NoCell;
        var bestCount = int.MaxValue;
        candidateMask = 0;

        for (var cell = 0; cell < CellCount; cell++)
        {
            if (cells[cell] != SudokuGrid.Empty)
            {
                continue;
            }

            var mask = CandidateMask(cells, cell);
            var count = System.Numerics.BitOperations.PopCount((uint)mask);
            if (count < bestCount)
            {
                (bestCell, bestCount, candidateMask) = (cell, count, mask);
                if (count <= 1)
                {
                    break; // Cannot do better: either forced or a dead end.
                }
            }
        }

        return bestCell;
    }

    /// <summary>Bit <c>d</c> is set when digit <c>d</c> can legally go in <paramref name="cell"/>.</summary>
    private static int CandidateMask(int[] cells, int cell)
    {
        const int allDigits = 0b11_1111_1110;
        var used = 0;
        foreach (var peer in PeerIndexes[cell])
        {
            used |= 1 << cells[peer];
        }

        return allDigits & ~used;
    }

    private int[] OrderCandidates(int mask)
    {
        var digits = Enumerable.Range(SudokuGrid.MinDigit, SudokuGrid.MaxDigit)
            .Where(digit => (mask & (1 << digit)) != 0)
            .ToArray();
        _random?.Shuffle(digits);
        return digits;
    }

    private static bool HasConflicts(int[] cells) =>
        Enumerable.Range(0, CellCount).Any(cell =>
            cells[cell] != SudokuGrid.Empty && PeerIndexes[cell].Any(peer => cells[peer] == cells[cell]));

    private static int ToIndex(Position position) => position.Row * SudokuGrid.Size + position.Column;

    private static int[] ToCells(IReadOnlyGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        return Position.All.Select(position => grid[position]).ToArray();
    }

    private static SudokuGrid ToGrid(int[] cells)
    {
        var grid = new SudokuGrid();
        foreach (var position in Position.All)
        {
            grid[position] = cells[ToIndex(position)];
        }

        return grid;
    }
}
