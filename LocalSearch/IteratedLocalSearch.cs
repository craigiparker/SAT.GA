using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

public class IteratedLocalSearch : ILocalSearch<SatSolution>
{
    private readonly Random _random;
    private readonly ILocalSearch<SatSolution> _localSearch;
    private readonly double _perturbationStrength;

    public IteratedLocalSearch(Random random, ILocalSearch<SatSolution> localSearch, double perturbationStrength = 0.1)
    {
        _random = random;
        _localSearch = localSearch;
        _perturbationStrength = perturbationStrength;
    }

    public void Improve(SatSolution individual, int maxIterations)
    {
        var bestAssignment = (bool[])individual.Assignment.Clone();
        var bestFitness = individual.SatisfiedClausesCount();

        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Apply local search
            _localSearch.Improve(individual, 100);

            // Perturb the solution
            var numFlips = (int)(individual.Assignment.Length * _perturbationStrength);
            for (int i = 0; i < numFlips; i++)
            {
                var flipIndex = _random.Next(individual.Assignment.Length);
                individual.Assignment[flipIndex] = !individual.Assignment[flipIndex];
            }

            // Update best solution
            var currentFitness = individual.SatisfiedClausesCount();
            if (currentFitness > bestFitness)
            {
                bestFitness = currentFitness;
                bestAssignment = (bool[])individual.Assignment.Clone();
            }
        }

        individual.Assignment = bestAssignment;
    }
} 