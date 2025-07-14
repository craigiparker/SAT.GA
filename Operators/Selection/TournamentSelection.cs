using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Selection;

/// <summary>
/// Implements tournament selection for SAT solutions, selecting the best from random groups.
/// </summary>
public class TournamentSelection : ISelectionOperator<SatSolution>
{
    private readonly Random _random;
    private readonly int _tournamentSize;

    /// <summary>
    /// Initializes a new instance of the TournamentSelection class.
    /// </summary>
    /// <param name="random">Random number generator.</param>
    /// <param name="tournamentSize">Number of individuals in each tournament.</param>
    public TournamentSelection(Random random, int tournamentSize = 3)
    {
        _random = random;
        _tournamentSize = tournamentSize;
    }

    /// <summary>
    /// Selects individuals for the next generation using tournament selection.
    /// </summary>
    /// <param name="population">The current population.</param>
    /// <param name="selectionSize">The number of individuals to select.</param>
    /// <returns>A list of selected individuals.</returns>
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