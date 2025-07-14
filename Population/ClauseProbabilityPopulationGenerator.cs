using System.Collections;
using SAT.GA.Models;

namespace SAT.GA.Population;

/// <summary>
/// Generates a population using clause statistics to bias variable assignments for SAT solutions.
/// </summary>
public class ClauseProbabilityPopulationGenerator : PopulationGenerator
{
    private readonly List<ClauseStats> _clauseStats;
    private readonly SatInstance _instance;
    private readonly Random _random;
    private readonly Dictionary<int, bool> _fixedValues = new();

    /// <summary>
    /// Initializes a new instance of the ClauseProbabilityPopulationGenerator class.
    /// </summary>
    /// <param name="instance">The SAT instance to generate solutions for.</param>
    /// <param name="random">Random number generator.</param>
    public ClauseProbabilityPopulationGenerator(SatInstance instance, Random random)
    {
        _instance = instance;
        _random = random;

        // Generate Clause Statistics
        _clauseStats = instance.Clauses.SelectMany(c => c.Literals).GroupBy(Math.Abs)
            .Select(s => new ClauseStats { Key = s.Key, Pos = s.Count(c => c > 0), Neg = s.Count(c => c < 0) })
            .OrderBy(k => k.Key).ToList();


        foreach (var variable in _clauseStats.Where(e => e.Neg == 0 || e.Pos == 0))
        {
            _fixedValues[variable.Key - 1] = variable.Neg == 0;

            // simplify clauses where variable is only one-sided
            instance.Clauses.RemoveAll(c => c.Literals.Exists(l => Math.Abs(l) == variable.Key));
        }
    }

    /// <summary>
    /// Creates a new individual SAT solution using clause probability heuristics.
    /// </summary>
    /// <returns>A new SatSolution instance.</returns>
    public override SatSolution CreateNewIndividual()
    {
        var assignment = new bool[_instance.VariableCount];
        foreach (var variable in _clauseStats)
        {
            assignment[variable.Key - 1] = _random.Next(variable.Neg + variable.Pos) >= variable.Neg;
        }

        return new SatSolution(_instance, assignment);
    }

    /// <summary>
    /// Overrides the solution with fixed variable assignments based on clause statistics.
    /// </summary>
    /// <param name="solution">The solution to override.</param>
    public override void OverrideSolution(SatSolution solution)
    {
        foreach (var bit in _fixedValues)
        {
            solution.Assignment[bit.Key] = bit.Value;
        }
    }

    /// <summary>
    /// Stores statistics for a clause variable.
    /// </summary>
    public class ClauseStats
    {
        /// <summary>Variable index (1-based).</summary>
        public int Key { get; set; }
        /// <summary>Number of positive occurrences.</summary>
        public int Pos { get; set; }
        /// <summary>Number of negative occurrences.</summary>
        public int Neg { get; set; }
        /// <summary>Total number of occurrences.</summary>
        public int TotalCount => Pos + Neg;
    }
}