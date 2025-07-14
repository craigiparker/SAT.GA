namespace SAT.GA.Interfaces;

/// <summary>
/// Interface for selection operators that select individuals from a population for reproduction.
/// </summary>
public interface ISelectionOperator<T> where T : class
{
    /// <summary>
    /// Selects a subset of individuals from the population for the next generation.
    /// </summary>
    /// <param name="population">The current population.</param>
    /// <param name="selectionSize">The number of individuals to select.</param>
    /// <returns>A list of selected individuals.</returns>
    List<T> Select(List<T> population, int selectionSize);
}