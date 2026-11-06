using System.Text;
using Sudoku.ConsoleApp.IO;

namespace Sudoku.Tests.TestDoubles;

/// <summary>Feeds scripted input lines and records everything written.</summary>
public sealed class FakeConsole : IConsole
{
    private readonly Queue<string> _input;
    private readonly StringBuilder _output = new();

    public FakeConsole(params string[] inputLines)
    {
        _input = new Queue<string>(inputLines);
    }

    public FakeConsole(IEnumerable<string> inputLines)
    {
        _input = new Queue<string>(inputLines);
    }

    /// <summary>All output with line endings normalised to '\n'.</summary>
    public string Output => _output.ToString().Replace("\r\n", "\n");

    public int KeyPresses { get; private set; }

    public string? ReadLine() => _input.TryDequeue(out var line) ? line : null;

    public void WriteLine(string text = "") => _output.Append(text).Append('\n');

    public bool WaitForKey()
    {
        KeyPresses++;
        return _input.TryDequeue(out _);
    }
}
