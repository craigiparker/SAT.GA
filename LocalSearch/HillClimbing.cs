using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

/// <summary>
/// Implements the Hill Climbing local search algorithm for improving SAT solutions.
/// </summary>
public class HillClimbing : ILocalSearch<SatSolution>
{
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the HillClimbing class.
    /// </summary>
    /// <param name="random">Random number generator.</param>
    public HillClimbing(Random random)
    {
        _random = random;
    }

    /// <summary>
    /// Improves a SAT solution using hill climbing for a given number of iterations.
    /// </summary>
    /// <param name="individual">The SAT solution to improve.</param>
    /// <param name="maxIterations">Maximum number of iterations to perform.</param>
    public void Improve(SatSolution individual, int maxIterations)
    {
        var currentFitness = individual.SatisfiedClausesCount();

        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Randomly select a variable to flip
            var flipIndex = _random.Next(individual.Assignment.Length);
            individual.Assignment[flipIndex] = !individual.Assignment[flipIndex];

            if (individual.IsSolution) return;

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