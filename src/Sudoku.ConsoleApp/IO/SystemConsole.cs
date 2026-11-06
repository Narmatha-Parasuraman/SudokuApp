namespace Sudoku.ConsoleApp.IO;

/// <summary><see cref="IConsole"/> backed by <see cref="System.Console"/>.</summary>
public sealed class SystemConsole : IConsole
{
    public string? ReadLine() => Console.ReadLine();

    public void WriteLine(string text = "") => Console.WriteLine(text);

    public bool WaitForKey()
    {
        // ReadKey throws when input is piped from a file, so fall back to reading a line.
        if (Console.IsInputRedirected)
        {
            return Console.ReadLine() is not null;
        }

        Console.ReadKey(intercept: true);
        return true;
    }
}
