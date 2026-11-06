namespace Sudoku.ConsoleApp.IO;

/// <summary>Abstracts terminal input and output so the game can be driven by tests.</summary>
public interface IConsole
{
    /// <summary>Reads a line of input, or returns <c>null</c> when input has ended.</summary>
    string? ReadLine();

    void WriteLine(string text = "");

    /// <summary>Blocks until the user presses a key. Returns <c>false</c> when input has ended.</summary>
    bool WaitForKey();
}
