using SAT.GA.Models;

namespace SAT.GA.Interfaces;

/// <summary>
/// Interface for population generators that create and initialize SAT solution populations.
/// </summary>
public interface IPopulationGenerator
{
    /// <summary>
    /// Creates a new individual SAT solution.
    /// </summary>
    /// <returns>A new SatSolution instance.</returns>
    SatSolution CreateNewIndividual();
    /// <summary>
    /// Initializes a population of the specified size.
    /// </summary>
    /// <param name="populationSize">The number of individuals to generate.</param>
    /// <returns>A list of SatSolution instances representing the population.</returns>
    List<SatSolution> InitializePopulation(int populationSize);
    /// <summary>
    /// Optionally overrides the solution with fixed values or constraints.
    /// </summary>
    /// <param name="solution">The solution to override.</param>
    void OverrideSolution(SatSolution solution);
}