using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

public class HillClimbing : ILocalSearch<SatSolution>
{
    private readonly Random _random;

    public HillClimbing(Random random)
    {
        _random = random;
    }

    public void Improve(SatSolution individual, int maxIterations)
    {
        var currentFitness = individual.SatisfiedClausesCount();

        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Randomly select a variable to flip
            var flipIndex = _random.Next(individual.Assignment.Length);
            individual.Assignment[flipIndex] = !individual.Assignment[flipIndex];

            var newFitness = individual.SatisfiedClausesCount();

            // Revert if not improving
            if (newFitness <= currentFitness)
            {
                individual.Assignment[flipIndex] = !individual.Assignment[flipIndex];
            }
            else
            {
                currentFitness = newFitness;
            }
        }
    }
}