namespace SAT.GA.Interfaces;

/// <summary>
/// Interface for local search operators that improve individual solutions.
/// </summary>
public interface ILocalSearch<T> where T : class
{
    /// <summary>
    /// Improves a solution using a local search strategy for a given number of iterations.
    /// </summary>
    /// <param name="individual">The solution to improve.</param>
    /// <param name="maxIterations">Maximum number of iterations to perform.</param>
    void Improve(T individual, int maxIterations);
}