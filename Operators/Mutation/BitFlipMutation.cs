using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Mutation;

/// <summary>
/// Implements bit-flip mutation by flipping each variable with a given probability.
/// </summary>
public class BitFlipMutation : IMutationOperator<SatSolution>
{
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the BitFlipMutation class.
    /// </summary>
    /// <param name="random">Random number generator.</param>
    public BitFlipMutation(Random random)
    {
        _random = random;
    }

    /// <summary>
    /// Mutates a SAT solution by flipping each variable with the given mutation rate.
    /// </summary>
    /// <param name="individual">The SAT solution to mutate.</param>
    /// <param name="mutationRate">The probability of flipping each variable.</param>
    public void Mutate(SatSolution individual, double mutationRate)
    {
        for (int i = 0; i < individual.Assignment.Length; i++)
        {
            if (_random.NextDouble() < mutationRate)
            {
                individual.Assignment[i] = !individual.Assignment[i];
            }
        }
    }
}