using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

/// <summary>
/// Fitness function that scores SAT solutions by the number of satisfied clauses.
/// </summary>
public class MaxSatFitness : IFitnessFunction<SatSolution>
{
    /// <summary>
    /// Calculates the fitness score as the number of satisfied clauses in the solution.
    /// </summary>
    /// <param name="individual">The SAT solution to evaluate.</param>
    /// <returns>The number of satisfied clauses.</returns>
    public double Calculate(SatSolution individual)
    {
        return individual.SatisfiedClausesCount();
    }
}
