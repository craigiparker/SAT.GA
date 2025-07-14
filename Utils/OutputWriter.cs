using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace SAT.GA.Utils;

/// <summary>
/// Handles formatted output and logging for the SAT genetic algorithm, including progress, results, and metrics.
/// </summary>
public class OutputWriter
{
    /// <summary>Path to the current file being processed.</summary>
    public string? FilePath { get; set; }
    /// <summary>Number of variables in the current SAT instance.</summary>
    public int VariableCount { get; set; }
    /// <summary>Number of clauses in the current SAT instance.</summary>
    public int ClauseCount { get; set; }
    /// <summary>Current generation number.</summary>
    public int Generation { get; set; }
    /// <summary>Number of restarts performed so far.</summary>
    public int Restarts { get; set; }
    /// <summary>Message to display in the output.</summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>Stopwatch for timing the current run.</summary>
    public Stopwatch? StopWatch { get; set; }
    /// <summary>Best fitness value found so far.</summary>
    public double? BestFitness { get; set; }
    /// <summary>Printable solution string for the current SAT instance.</summary>
    public string? Solution { get; set; }
    /// <summary>Indicates whether the current solution satisfies all clauses.</summary>
    public bool IsSatisfied { get; set; }
    /// <summary>Current instance index for output positioning.</summary>
    public int Instance { get; private set; }
    /// <summary>Whether to hide output in the console.</summary>
    public bool HideOutput { get; set; }

    private object _locker = new();
    private string _buffer = new string(' ', 20);

    private TimeSpan ElapsedTime => StopWatch?.Elapsed ?? TimeSpan.Zero;

    private void Write()
    {
        var output = new StringBuilder();

        // Calculate column widths
        int leftColumnWidth = Math.Max("Configuration".Length,
            new[]
            {
                "File Path".Length,
                "Variables".Length,
                "Clauses".Length,
                "Generation".Length,
                "Restarts".Length,
                "Elapsed Time".Length,
                "Best Fitness".Length,
                "Message".Length
            }.Max());

        int rightColumnWidth = Math.Max("Value".Length,
            new[]
            {
                FilePath?.Length ?? 0,
                VariableCount.ToString().Length,
                ClauseCount.ToString().Length,
                Generation.ToString().Length,
                Restarts.ToString().Length,
                ElapsedTime.ToString().Length,
                (BestFitness??0d).ToString(CultureInfo.InvariantCulture).Length,
                Message.Length
            }.Max());

        int totalWidth = leftColumnWidth + rightColumnWidth + 7; // 7 accounts for borders and padding

        // Draw top border
        output.AppendLine("┌" + new string('─', totalWidth - 2) + "┐" + _buffer);

        // Write header
        output.AppendLine($"│ {"Configuration".PadRight(leftColumnWidth)} │ {"Value".PadRight(rightColumnWidth)} │" + _buffer);

        // Draw header separator
        output.AppendLine("├" + new string('─', leftColumnWidth + 2) + "┼" + new string('─', rightColumnWidth + 2) + "┤" + _buffer);

        // Write rows
        WriteRow(output,"File Path", FilePath ?? "N/A", leftColumnWidth, rightColumnWidth);
        WriteRow(output, "Variables", VariableCount.ToString(), leftColumnWidth, rightColumnWidth);
        WriteRow(output, "Clauses", ClauseCount.ToString(), leftColumnWidth, rightColumnWidth);
        WriteRow(output, "Generation", Generation.ToString(), leftColumnWidth, rightColumnWidth);
        WriteRow(output, "Restarts", Restarts.ToString(), leftColumnWidth, rightColumnWidth);
        WriteRow(output, "Best Fitness", (BestFitness??0d).ToString(CultureInfo.InvariantCulture), leftColumnWidth, rightColumnWidth);
        WriteRow(output, "Elapsed Time", ElapsedTime.ToString(), leftColumnWidth, rightColumnWidth);
        WriteRow(output, "Message", Message, leftColumnWidth, rightColumnWidth);

        // Draw bottom border
        output.AppendLine("└" + new string('─', leftColumnWidth + 2) + "┴" + new string('─', rightColumnWidth + 2) + "┘" + _buffer);

        if (Solution != null)
        {
            output.Append(IsSatisfied ? "SATISFIABLE" : "TIMEOUT" );
            output.AppendLine(new string(' ', Console.BufferWidth - 10));
            output.Append(Solution);
            output.AppendLine(_buffer);
            output.AppendLine(new string(' ', Console.BufferWidth));
        }
        else
        {
            for(int i=0; i<=this.VariableCount; i+=25)
                output.AppendLine(new string(' ', Console.BufferWidth));
        }

        var cursorPosition = (15 + VariableCount / 30) * Instance;
        if (cursorPosition > Console.BufferHeight)
        {
            Instance = 0;
            Console.Clear();
        }
        else
        {
            Console.SetCursorPosition(0, cursorPosition);
        }

        Console.WriteLine(output);
    }

    private void WriteSync()
    {
        lock (_locker)
        {
            Write();
        }
    }

    /// <summary>
    /// Resets the output for a new instance and clears the solution.
    /// </summary>
    public void ResetInstance()
    {
        if (HideOutput)
        {
            return;
        }

        lock (_locker)
        {
            Write();
            Solution = null;
            Instance++;
        }
    }

    /// <summary>
    /// Writes a message to the output asynchronously.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public void WriteLine(string message)
    {
        if (HideOutput)
        {
            return;
        }

        Message = message;
        Task.Run(WriteSync);
    }

    private void WriteRow(StringBuilder output, string label, string value, int leftWidth, int rightWidth)
    {
        output.AppendLine($"│ {label.PadRight(leftWidth)} │ {value.PadRight(rightWidth)} │{_buffer}");
    }

    /// <summary>
    /// Writes a completion message to the output.
    /// </summary>
    /// <param name="message">The completion message to write.</param>
    public void WriteCompletion(string message)
    {
        if (HideOutput)
        {
            Console.WriteLine(message);
            return;
        }

        lock (_locker)
        {
            Console.WriteLine(message);
        }
    }
}