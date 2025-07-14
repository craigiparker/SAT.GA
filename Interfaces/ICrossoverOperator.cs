namespace SAT.GA.Interfaces;

/// <summary>
/// Interface for crossover operators that combine two parent solutions to produce offspring.
/// </summary>
public interface ICrossoverOperator<T> where T : class
{
    /// <summary>
    /// Performs crossover between two parent solutions to produce offspring.
    /// </summary>
    /// <param name="parent1">The first parent solution.</param>
    /// <param name="parent2">The second parent solution.</param>
    /// <returns>An enumerable of offspring solutions.</returns>
    IEnumerable<T> Crossover(T parent1, T parent2);
}