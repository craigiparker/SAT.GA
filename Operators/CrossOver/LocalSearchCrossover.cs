using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Crossover;

public class LocalSearchCrossover : ICrossoverOperator<SatSolution>
{
    private readonly Random _random;
    private readonly ILocalSearch<SatSolution>? _localSearch;
    private readonly double _crossoverRate;

    public LocalSearchCrossover(Random random, ILocalSearch<SatSolution> localSearch, double crossoverRate = 0.9)
    {
        _random = random;
        _localSearch = localSearch;
        _crossoverRate = crossoverRate;
    }

    public IEnumerable<SatSolution> Crossover(SatSolution parent1, SatSolution parent2)
    {
        if (_random.NextDouble() > _crossoverRate)
        {
            var nonCrossoverCandidate =  _random.NextDouble() < 0.5 ? parent1 : parent2;
            yield return new SatSolution(nonCrossoverCandidate.Instance, (bool[])nonCrossoverCandidate.Assignment.Clone());
            yield break;
        }

        var childAssignment = new bool[parent1.Assignment.Length];

        // Create initial offspring
        for (int i = 0; i < childAssignment.Length; i++)
        {
            childAssignment[i] = _random.NextDouble() < 0.5
                ? parent1.Assignment[i]
                : parent2.Assignment[i];
        }

        var child = new SatSolution(parent1.Instance, childAssignment);

        // Improve with local search
        _localSearch?.Improve(child, 1000); // TODO

        yield return child;
    }
}