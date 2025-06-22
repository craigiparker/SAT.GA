using SAT.GA.Models;

namespace SAT.GA.Population;

public class ClauseProbabilityPopulationGenerator : PopulationGenerator
{
    private readonly SatInstance _instance;
    private readonly Random _random;
    private readonly List<ClauseStats> _clauseStats;
    private readonly Dictionary<int, bool> _fixedValues = new();

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

    public override SatSolution CreateNewIndividual()
    {
        var assignment = new bool[_instance.VariableCount];
        foreach (var variable in _clauseStats)
        {
            assignment[variable.Key - 1] = _random.Next(variable.Neg + variable.Pos) >= variable.Neg;
        }

        return new SatSolution(_instance, assignment);
    }

    public override void OverrideSolution(SatSolution solution)
    {
        foreach (var bit in _fixedValues)
        {
            solution.Assignment[bit.Key] = bit.Value;
        }
    }

    public class ClauseStats
    {
        public int Key { get; set; }
        public int Pos { get; set; }
        public int Neg { get; set; }
        public int TotalCount => Pos + Neg;
    }
}