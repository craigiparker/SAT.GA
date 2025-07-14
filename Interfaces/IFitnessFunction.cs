namespace SAT.GA.Interfaces;

/// <summary>
/// Interface for fitness functions that evaluate the quality of SAT solutions.
/// </summary>
public interface IFitnessFunction<T> where T : class
{
    /// <summary>
    /// Calculates the fitness score for a given solution.
    /// </summary>
    /// <param name="individual">The solution to evaluate.</param>
    /// <returns>The fitness score.</returns>
    double Calculate(T individual);
}