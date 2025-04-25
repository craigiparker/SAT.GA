// SAT.GA/Operators/Selection/RankSelection.cs
using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Selection;

public class RankSelection : ISelectionOperator<SatSolution>
{
    private readonly Random _random;

    public RankSelection(Random random)
    {
        _random = random;
    }

    public List<SatSolution> Select(List<SatSolution> population, int selectionSize)
    {
        var rankedPopulation = population.OrderByDescending(i => i.Fitness).ToList();
        var selected = new List<SatSolution>();

        for (int i = 0; i < selectionSize; i++)
        {
            // Higher rank (lower index) has higher probability
            var index = (int)(Math.Sqrt(_random.NextDouble() * population.Count * population.Count));
            selected.Add(rankedPopulation[index]);
        }

        return selected;
    }
}