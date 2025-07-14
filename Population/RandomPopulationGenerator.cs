using SAT.GA.Models;

namespace SAT.GA.Population;

/// <summary>
/// Generates a population of SAT solutions with random variable assignments.
/// </summary>
public class RandomPopulationGenerator(SatInstance _instance, Random _random) : PopulationGenerator
{
    /// <summary>
    /// Creates a new individual SAT solution with random assignments.
    /// </summary>
    /// <returns>A new SatSolution instance with random variable values.</returns>
    public override SatSolution CreateNewIndividual()
    {
        var assignment = new bool[_instance.VariableCount];

        for (int i = 0; i < assignment.Length; i++)
        {
            assignment[i] = _random.NextDouble() < 0.5;
        }

        return new SatSolution(_instance, assignment);
    }
}