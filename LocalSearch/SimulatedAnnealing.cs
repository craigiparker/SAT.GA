using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

public class SimulatedAnnealing : ILocalSearch<SatSolution>
{
    private readonly Random _random;
    private readonly double _initialTemperature;
    private readonly double _coolingRate;

    public SimulatedAnnealing(Random random, double initialTemperature = 100.0, double coolingRate = 0.9)
    {
        _random = random;
        _initialTemperature = initialTemperature;
        _coolingRate = coolingRate;
    }

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