// SAT.GA/LocalSearch/TabuSearch.cs
using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

public class TabuSearch : ILocalSearch<SatSolution>
{
    private readonly Random _random;
    private readonly int _tabuTenure;
    private readonly int _maxIterations;

    public TabuSearch(Random random, int tabuTenure = 5, int maxIterations = 100)
    {
        _random = random;
        _tabuTenure = tabuTenure;
        _maxIterations = maxIterations;
    }

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
        }

        // Restore best solution found
        individual.Assignment = bestAssignment;
    }
}