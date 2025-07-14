using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Selection;

/// <summary>
/// Implements roulette wheel selection for SAT solutions, selecting individuals with probability proportional to fitness.
/// </summary>
public class RouletteWheelSelection : ISelectionOperator<SatSolution>
{
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the RouletteWheelSelection class.
    /// </summary>
    /// <param name="random">Random number generator.</param>
    public RouletteWheelSelection(Random random)
    {
        _random = random;
    }

    /// <summary>
    /// Selects individuals for the next generation using roulette wheel selection.
    /// </summary>
    /// <param name="population">The current population.</param>
    /// <param name="selectionSize">The number of individuals to select.</param>
    /// <returns>A list of selected individuals.</returns>
    public List<SatSolution> Select(List<SatSolution> population, int selectionSize)
    {
        var totalFitness = population.Sum(i => i.Fitness ?? 0);
        var selected = new List<SatSolution>();

        for (int i = 0; i < selectionSize; i++)
        {
            var slice = _random.NextDouble() * totalFitness;
            double cumulative = 0;

            foreach (var item in population)
            {
                cumulative += item.Fitness ?? 0;
                if (cumulative >= slice)
                {
                    selected.Add(item);
                    break;
                }
            }
        }

        return selected;
    }
}