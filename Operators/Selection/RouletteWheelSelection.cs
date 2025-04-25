using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Selection;

public class RouletteWheelSelection : ISelectionOperator<SatSolution>
{
    private readonly Random _random;

    public RouletteWheelSelection(Random random)
    {
        _random = random;
    }

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