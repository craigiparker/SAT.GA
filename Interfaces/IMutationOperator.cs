namespace SAT.GA.Interfaces;

/// <summary>
/// Interface for mutation operators that modify individual solutions.
/// </summary>
public interface IMutationOperator<T> where T : class
{
    /// <summary>
    /// Mutates a solution with a given mutation rate.
    /// </summary>
    /// <param name="individual">The solution to mutate.</param>
    /// <param name="mutationRate">The probability of mutation for each gene.</param>
    void Mutate(T individual, double mutationRate);
}