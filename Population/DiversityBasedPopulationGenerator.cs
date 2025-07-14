using SAT.GA.Models;

namespace SAT.GA.Population;

/// <summary>
/// Generates a population with maximum diversity by maximizing Hamming distance between individuals.
/// </summary>
public class DiversityBasedPopulationGenerator(SatInstance instance, Random random) : RandomPopulationGenerator(instance, random)
{
    private const int NumberOfTrials = 10; // Number of trial candidates per individual

    /// <summary>
    /// Creates a new individual that maximizes diversity with respect to the current population.
    /// </summary>
    /// <param name="population">The current population of solutions.</param>
    /// <returns>A new SatSolution instance with maximum minimum Hamming distance.</returns>
    public override SatSolution CreateNewIndividual(List<SatSolution> population)
    {
        // For the first individual, just return a random solution
        if (population.Count == 0)
        {
            return base.CreateNewIndividual();
        }

        SatSolution bestSolution = null;
        int bestScore = -1;

        // Try T random candidates
        for (int trial = 0; trial < NumberOfTrials; trial++)
        {
            var candidate = base.CreateNewIndividual();
            int minDistance = int.MaxValue;

            // Find minimum Hamming distance to existing population
            foreach (var existingSolution in population)
            {
                int distance = HammingDistance(candidate, existingSolution);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }

            // Keep the solution with maximum minimum distance
            if (minDistance > bestScore)
            {
                bestScore = minDistance;
                bestSolution = candidate;
            }
        }

        return bestSolution ?? base.CreateNewIndividual();
    }

    /// <summary>
    /// Computes the Hamming distance between two SAT solutions.
    /// </summary>
    /// <param name="a">The first solution.</param>
    /// <param name="b">The second solution.</param>
    /// <returns>The Hamming distance between the two solutions.</returns>
    private int HammingDistance(SatSolution a, SatSolution b)
    {
        if (a.Assignment.Length != b.Assignment.Length)
            throw new ArgumentException("Solutions must have same length");

        int distance = 0;
        for (int i = 0; i < a.Assignment.Length; i++)
        {
            if (a.Assignment[i] != b.Assignment[i])
            {
                distance++;
            }
        }
        return distance;
    }
}