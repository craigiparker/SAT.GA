using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

/// <summary>
/// Implements the Simulated Annealing local search algorithm for improving SAT solutions.
/// </summary>
public class SimulatedAnnealing : ILocalSearch<SatSolution>
{
    private readonly double _coolingRate;
    private readonly Random _random;
    private readonly double _initialTemperature;

    /// <summary>
    /// Initializes a new instance of the SimulatedAnnealing class.
    /// </summary>
    /// <param name="random">Random number generator.</param>
    /// <param name="initialTemperature">Initial temperature for annealing.</param>
    /// <param name="coolingRate">Cooling rate for temperature decay.</param>
    public SimulatedAnnealing(Random random, double initialTemperature = 100.0, double coolingRate = 0.9)
    {
        _random = random;
        _initialTemperature = initialTemperature;
        _coolingRate = coolingRate;
    }

    /// <summary>
    /// Improves a SAT solution using simulated annealing for a given number of iterations.
    /// </summary>
    /// <param name="individual">The SAT solution to improve.</param>
    /// <param name="maxIterations">Maximum number of iterations to perform.</param>
    public void Improve(SatSolution individual, int maxIterations)
    {
        var currentFitness = individual.SatisfiedClausesCount();
        var bestAssignment = (bool[])individual.Assignment.Clone();
        var bestFitness = currentFitness;
        var temperature = _initialTemperature;

        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Randomly select a variable to flip
            var flipIndex = _random.Next(individual.Assignment.Length);
            individual.Assignment[flipIndex] = !individual.Assignment[flipIndex];
            if (individual.IsSolution) return;
            var newFitness = individual.SatisfiedClausesCount();

            // Calculate acceptance probability
            var deltaE = newFitness - currentFitness;
            var acceptanceProbability = Math.Exp(deltaE / temperature);

            // Accept move if it improves or based on probability
            if (deltaE > 0 || _random.NextDouble() < acceptanceProbability)
            {
                currentFitness = newFitness;
                if (currentFitness > bestFitness)
                {
                    bestFitness = currentFitness;
                    bestAssignment = (bool[])individual.Assignment.Clone();
                }
            }
            else
            {
                // Revert the move
                individual.Assignment[flipIndex] = !individual.Assignment[flipIndex];
            }

            // Cool down
            temperature *= _coolingRate;
        }

        // Restore best solution
        individual.Assignment = bestAssignment;
    }
} 