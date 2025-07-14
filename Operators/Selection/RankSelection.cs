using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Selection;

/// <summary>
/// Implements rank-based selection for SAT solutions, assigning selection probability based on rank.
/// </summary>
public class RankSelection : ISelectionOperator<SatSolution>
{
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the RankSelection class.
    /// </summary>
    /// <param name="random">Random number generator.</param>
    public RankSelection(Random random)
    {
        _random = random;
    }

    /// <summary>
    /// Selects individuals for the next generation using rank-based selection.
    /// </summary>
    /// <param name="population">The current population.</param>
    /// <param name="selectionSize">The number of individuals to select.</param>
    /// <returns>A list of selected individuals.</returns>
    public List<SatSolution> Select(List<SatSolution> population, int selectionSize)
    {
        // Step 1: Sort population by fitness (ascending or descending depending on problem)
        var sortedPopulation = population.OrderBy(ind => ind.Fitness).ToList();

        int n = sortedPopulation.Count;

        // Step 2: Assign ranks and calculate cumulative probability
        // Assign selection probabilities based on rank (linear rank selection)
        double[] selectionProbabilities = new double[n];
        double totalRank = n * (n + 1) / 2.0;

        for (int i = 0; i < n; i++)
        {
            // Higher rank gets higher selection probability
            selectionProbabilities[i] = (i + 1) / totalRank;
        }

        // Convert to cumulative probabilities
        double[] cumulativeProbabilities = new double[n];
        cumulativeProbabilities[0] = selectionProbabilities[0];
        for (int i = 1; i < n; i++)
        {
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + selectionProbabilities[i];
        }

        // Step 3: Select individuals based on cumulative probability (roulette wheel style)
        var selected = new List<SatSolution>();
        var rand = new Random();

        for (int s = 0; s < selectionSize; s++)
        {
            double r = rand.NextDouble();

            for (int i = 0; i < n; i++)
            {
                if (r <= cumulativeProbabilities[i])
                {
                    selected.Add(sortedPopulation[i]);
                    break;
                }
            }
        }

        return selected;
    }
}