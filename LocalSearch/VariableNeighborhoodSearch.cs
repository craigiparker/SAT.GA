using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

public class VariableNeighborhoodSearch : ILocalSearch<SatSolution>
{
    private readonly Random _random;
    private readonly int _maxNeighborhoods;

    public VariableNeighborhoodSearch(Random random, int maxNeighborhoods = 3)
    {
        _random = random;
        _maxNeighborhoods = maxNeighborhoods;
    }

    public void Improve(SatSolution individual, int maxIterations)
    {
        var bestAssignment = (bool[])individual.Assignment.Clone();
        var bestFitness = individual.SatisfiedClausesCount();

        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Try different neighborhood structures
            for (int k = 1; k <= _maxNeighborhoods; k++)
            {
                // Generate a random solution in k-th neighborhood
                var tempAssignment = (bool[])individual.Assignment.Clone();
                for (int i = 0; i < k; i++)
                {
                    var flipIndex = _random.Next(tempAssignment.Length);
                    tempAssignment[flipIndex] = !tempAssignment[flipIndex];
                }

                // Local search in the new neighborhood
                var currentFitness = LocalSearch(tempAssignment, individual.Instance);
                
                if (currentFitness > bestFitness)
                {
                    bestFitness = currentFitness;
                    bestAssignment = (bool[])tempAssignment.Clone();
                    individual.Assignment = (bool[])tempAssignment.Clone();
                    break; // Move to next iteration
                }
            }
        }

        individual.Assignment = bestAssignment;
    }

    private int LocalSearch(bool[] assignment, SatInstance instance)
    {
        var currentFitness = CountSatisfiedClauses(assignment, instance);
        var improved = true;

        while (improved)
        {
            improved = false;
            for (int i = 0; i < assignment.Length; i++)
            {
                assignment[i] = !assignment[i];
                var newFitness = CountSatisfiedClauses(assignment, instance);
                
                if (newFitness > currentFitness)
                {
                    currentFitness = newFitness;
                    improved = true;
                }
                else
                {
                    assignment[i] = !assignment[i];
                }
            }
        }

        return currentFitness;
    }

    private int CountSatisfiedClauses(bool[] assignment, SatInstance instance)
    {
        return instance.Clauses.Count(c => c.IsSatisfied(assignment));
    }
} 