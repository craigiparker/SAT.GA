// SAT.GA/Operators/Crossover/ClauseSatisfactionCrossover.cs
using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Crossover;

public class ClauseSatisfactionCrossover : ICrossoverOperator<SatSolution>
{
    private readonly Random _random;
    private readonly double _crossoverRate;
    private readonly ILocalSearch<SatSolution>? _localSearch;

    public ClauseSatisfactionCrossover(Random random, ILocalSearch<SatSolution>? localSearch, double crossoverRate = 0.9)
    {
        _random = random;
        _localSearch = localSearch;
        _crossoverRate = crossoverRate;
    }

    public IEnumerable<SatSolution> Crossover(SatSolution parent1, SatSolution parent2)
    {
        if (_random.NextDouble() > _crossoverRate)
        {
            var nonCrossoverCandidate = _random.NextDouble() < 0.5 ? parent1 : parent2;
            yield return new SatSolution(nonCrossoverCandidate.Instance, (bool[])nonCrossoverCandidate.Assignment.Clone());
            yield break;
        }

        var instance = parent1.Instance;
        var childAssignment = new bool[parent1.Assignment.Length];

        // Identify clauses that are unsatisfied in one parent but satisfied in the other
        var unsatisfiedInParent1 = instance.Clauses
            .Select((c, i) => new { Clause = c, Index = i })
            .Where(x => !x.Clause.IsSatisfied(parent1.Assignment) &&
                        x.Clause.IsSatisfied(parent2.Assignment))
            .ToList();

        var unsatisfiedInParent2 = instance.Clauses
            .Select((c, i) => new { Clause = c, Index = i })
            .Where(x => !x.Clause.IsSatisfied(parent2.Assignment) &&
                        x.Clause.IsSatisfied(parent1.Assignment))
            .ToList();

        // Create a preference map for variables based on which parent satisfies more clauses
        var variablePreferences = new Dictionary<int, double>();
        for (int i = 0; i < childAssignment.Length; i++)
        {
            variablePreferences[i] = 0.5; // Default neutral preference
        }

        // For clauses unsatisfied in parent1 but satisfied in parent2,
        // increase preference for parent2's variable assignments
        foreach (var item in unsatisfiedInParent1)
        {
            foreach (var literal in item.Clause.Literals)
            {
                var variable = Math.Abs(literal) - 1;
                variablePreferences[variable] += 0.1;
            }
        }

        // For clauses unsatisfied in parent2 but satisfied in parent1,
        // increase preference for parent1's variable assignments
        foreach (var item in unsatisfiedInParent2)
        {
            foreach (var literal in item.Clause.Literals)
            {
                var variable = Math.Abs(literal) - 1;
                variablePreferences[variable] -= 0.1;
            }
        }

        // Create child assignment based on preferences
        for (int i = 0; i < childAssignment.Length; i++)
        {
            var preference = Math.Max(0, Math.Min(1, variablePreferences[i]));
            childAssignment[i] = _random.NextDouble() < preference
                ? parent2.Assignment[i]
                : parent1.Assignment[i];
        }

        var child =  new SatSolution(instance, childAssignment);

        // Improve with local search
        //_localSearch?.Improve(child, 1000);

        yield return child;
    }
}