using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace SAT.GA.Utils;

public class OutputWriter
{
    private object _locker = new();
    private string _buffer = new string(' ', 20);
    public string? FilePath { get; set; }
    public int VariableCount { get; set; }
    public int ClauseCount { get; set; }
    public int Generation { get; set; }
    public int Restarts { get; set; }
    public string Message { get; set; } = string.Empty;
    public Stopwatch? StopWatch { get; set; }
    public double? BestFitness { get; set; }

    private TimeSpan ElapsedTime => StopWatch?.Elapsed ?? TimeSpan.Zero;
    public string? Solution { get; set; }
    public bool IsSatisfied { get; set; }

    public int Instance { get; private set; }

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
        //Console.Clear();
        Console.SetCursorPosition(0, (15 + VariableCount / 30) * Instance );
        Console.WriteLine(output);
    }

    private void WriteSync()
    {
        lock (_locker)
        {
            Write();
        }
    }

    public void ResetInstance()
    {
        lock (_locker)
        {
            Write();
            Solution = null;
            Instance++;
        }
    }

    public void WriteLine(string message)
    {
        Message = message;
        Task.Run(WriteSync);
    }

    private void WriteRow(StringBuilder output, string label, string value, int leftWidth, int rightWidth)
    {
        output.AppendLine($"│ {label.PadRight(leftWidth)} │ {value.PadRight(rightWidth)} │{_buffer}");
    }

    public void WriteCompletion(string message)
    {
        lock (_locker)
        {
            Console.WriteLine(message);
        }
    }
}