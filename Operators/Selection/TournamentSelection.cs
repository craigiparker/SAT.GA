using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Selection;

public class TournamentSelection : ISelectionOperator<SatSolution>
{
    private readonly Random _random;
    private readonly int _tournamentSize;

    public TournamentSelection(Random random, int tournamentSize = 3)
    {
        _random = random;
        _tournamentSize = tournamentSize;
    }

    public List<SatSolution> Select(List<SatSolution> population, int selectionSize)
    {
        var selected = new List<SatSolution>();

        for (int i = 0; i < selectionSize; i++)
        {
            var tournament = new List<SatSolution>();
            for (int j = 0; j < _tournamentSize; j++)
            {
                tournament.Add(population[_random.Next(population.Count)]);
            }
            selected.Add(tournament.OrderByDescending(i => i.Fitness).First());
        }

        return selected;
    }
}