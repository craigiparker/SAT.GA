using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

/// <summary>
/// Implements the Tabu Search local search algorithm for improving SAT solutions.
/// </summary>
public class TabuSearch : ILocalSearch<SatSolution>
{
    private readonly int _maxIterations;
    private readonly int _tabuTenure;
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the TabuSearch class.
    /// </summary>
    /// <param name="random">Random number generator.</param>
    /// <param name="tabuTenure">Number of iterations a move remains tabu.</param>
    /// <param name="maxIterations">Maximum number of iterations for the search.</param>
    public TabuSearch(Random random, int tabuTenure = 5, int maxIterations = 10000)
    {
        _random = random;
        _tabuTenure = tabuTenure;
        _maxIterations = maxIterations;
    }

    /// <summary>
    /// Improves a SAT solution using tabu search for a given number of iterations.
    /// </summary>
    /// <param name="individual">The SAT solution to improve.</param>
    /// <param name="maxIterations">Maximum number of iterations to perform.</param>
    public void Improve(SatSolution individual, int maxIterations)
    {
        var tabuList = new Queue<int>();
        var bestAssignment = (bool[])individual.Assignment.Clone();
        var bestFitness = individual.SatisfiedClausesCount();

        for (int iter = 0; iter < Math.Min(maxIterations, _maxIterations); iter++)
        {
            var bestMove = -1;
            var bestMoveFitness = -1;

            // Evaluate all possible 1-flip moves
            for (int i = 0; i < individual.Assignment.Length; i++)
            {
                if (tabuList.Contains(i)) continue;

                individual.Assignment[i] = !individual.Assignment[i];
                var currentFitness = individual.SatisfiedClausesCount();

                if (individual.IsSolution) return;

                individual.Assignment[i] = !individual.Assignment[i];

                if (currentFitness > bestMoveFitness)
                {
                    bestMove = i;
                    bestMoveFitness = currentFitness;
                }
            }

            if (bestMove == -1) break; // No non-tabu moves

            // Apply the best move
            individual.Assignment[bestMove] = !individual.Assignment[bestMove];
            tabuList.Enqueue(bestMove);

            // Maintain tabu list size
            if (tabuList.Count > _tabuTenure)
            {
                tabuList.Dequeue();
            }

            // Update best solution found
            if (bestMoveFitness > bestFitness)
            {
                bestFitness = bestMoveFitness;
                bestAssignment = (bool[])individual.Assignment.Clone();
            }
            else
            {
                break;
            }
        }

        // Restore best solution found
        individual.Assignment = bestAssignment;
    }
}