﻿using SAT.GA.Models;

namespace SAT.GA.Utils;

/// <summary>
/// Parses DIMACS CNF files into SatInstance objects for use in the genetic algorithm.
/// </summary>
public class DimacsParser
{
    /// <summary>
    /// Parses the contents of a DIMACS CNF file into a SatInstance.
    /// </summary>
    /// <param name="cnfContent">The string content of the CNF file.</param>
    /// <returns>A SatInstance representing the parsed SAT problem.</returns>
    public SatInstance Parse(string cnfContent)
    {
        var lines = cnfContent.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
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
                var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                variableCount = int.Parse(parts[2]);
                clauseCount = int.Parse(parts[3]);
                continue;
            }

            // Clause line
            var literals = line.Split([' '], StringSplitOptions.RemoveEmptyEntries)
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