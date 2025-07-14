using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Population;

/// <summary>
/// Abstract base class for population generators in the genetic algorithm.
/// </summary>
public abstract class PopulationGenerator : IPopulationGenerator
{
    /// <summary>
    /// Creates a new individual SAT solution.
    /// </summary>
    /// <returns>A new SatSolution instance.</returns>
    public abstract SatSolution CreateNewIndividual();

    /// <summary>
    /// Creates a new individual SAT solution, possibly considering the current population.
    /// </summary>
    /// <param name="population">The current population of solutions.</param>
    /// <returns>A new SatSolution instance.</returns>
    public virtual SatSolution CreateNewIndividual(List<SatSolution> population)
    {
        return CreateNewIndividual();
    }

    /// <summary>
    /// Optionally overrides the solution with fixed values or constraints.
    /// </summary>
    /// <param name="solution">The solution to override.</param>
    public virtual void OverrideSolution(SatSolution solution)
    {
    }

    /// <summary>
    /// Initializes a population of the specified size.
    /// </summary>
    /// <param name="populationSize">The number of individuals to generate.</param>
    /// <returns>A list of SatSolution instances representing the population.</returns>
    public List<SatSolution> InitializePopulation(int populationSize)
    {
        var population = new List<SatSolution>();

        for (int i = 0; i < populationSize; i++)
        {
            population.Add(CreateNewIndividual(population));
        }

        return population;
    }

    
}