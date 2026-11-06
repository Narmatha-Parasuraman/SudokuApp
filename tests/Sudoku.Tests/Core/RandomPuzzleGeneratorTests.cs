using Sudoku.Core.Domain;
using Sudoku.Core.Generation;
using Sudoku.Core.Solving;
using Sudoku.Core.Validation;

namespace Sudoku.Tests.Core;

public class RandomPuzzleGeneratorTests
{
    private static RandomPuzzleGenerator CreateGenerator(int seed, int clueCount = RandomPuzzleGenerator.DefaultClueCount)
    {
        var random = new Random(seed);
        return new RandomPuzzleGenerator(new BacktrackingSolver(random), random, clueCount);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Generate_HasExactlyThirtyGivens(int seed)
    {
        var puzzle = CreateGenerator(seed).Generate();

        Assert.Equal(30, Position.All.Count(puzzle.IsGiven));
    }

    [Fact]
    public void Generate_SolutionIsCompleteAndValid()
    {
        var puzzle = CreateGenerator(seed: 4).Generate();

        Assert.True(puzzle.Solution.IsFull);
        Assert.Empty(new GridValidator().Validate(puzzle.Solution));
    }

    [Fact]
    public void Generate_PuzzleHasUniqueSolution()
    {
        var puzzle = CreateGenerator(seed: 5).Generate();

        Assert.Equal(1, new BacktrackingSolver().CountSolutions(puzzle.Givens, limit: 2));
    }

    [Fact]
    public void Generate_SameSeed_ProducesSamePuzzle()
    {
        var first = CreateGenerator(seed: 6).Generate();
        var second = CreateGenerator(seed: 6).Generate();

        Assert.All(Position.All, p => Assert.Equal(first.Givens[p], second.Givens[p]));
    }

    [Fact]
    public void Generate_DifferentSeeds_ProduceDifferentPuzzles()
    {
        var first = CreateGenerator(seed: 7).Generate();
        var second = CreateGenerator(seed: 8).Generate();

        Assert.Contains(Position.All, p => first.Givens[p] != second.Givens[p]);
    }

    [Fact]
    public void Generate_CustomClueCount_IsRespected()
    {
        var puzzle = CreateGenerator(seed: 9, clueCount: 40).Generate();

        Assert.Equal(40, Position.All.Count(puzzle.IsGiven));
    }

    [Theory]
    [InlineData(16)]
    [InlineData(82)]
    public void Constructor_ImpossibleClueCount_Throws(int clueCount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateGenerator(seed: 1, clueCount));
    }
}
