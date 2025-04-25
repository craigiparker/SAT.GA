// SAT.GA/Parsers/DimacsParser.cs
using SAT.GA.Models;

namespace SAT.GA.Parsers;

public class DimacsParser
{
    public SatInstance Parse(string cnfContent)
    {
        var lines = cnfContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        int variableCount = 0;
        int clauseCount = 0;
        var clauses = new List<SatClause>();

        foreach (var l in lines)
        {
            var line = l.Trim();
            if (line.StartsWith("%")) break; // Finished Processing
            if (line == string.Empty) continue;
            if (line.StartsWith("c")) continue; // Comment
            if (line.StartsWith("p"))
            {
                // Problem line: p cnf variables clauses
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                variableCount = int.Parse(parts[2]);
                clauseCount = int.Parse(parts[3]);
                continue;
            }

            // Clause line
            var literals = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .TakeWhile(s => s != "0")
                .Select(int.Parse)
                .ToList();

            clauses.Add(new SatClause(clauses.Count, literals));
        }

        if (clauses.Count != clauseCount)
        {
            throw new FormatException($"Expected {clauseCount} clauses but found {clauses.Count}");
        }

        return new SatInstance(variableCount, clauses);
    }
}