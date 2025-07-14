using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Mutation;

/// <summary>
/// Implements N-bit mutation by flipping a specified number of random variables in a SAT solution.
/// </summary>
public class NBitMutation(int numberOfBits, Random random) : IMutationOperator<SatSolution>
{
    /// <summary>
    /// Mutates a SAT solution by flipping a specified number of random variables.
    /// </summary>
    /// <param name="individual">The SAT solution to mutate.</param>
    /// <param name="mutationRate">The probability of mutation (not used in this strategy).</param>
    public void Mutate(SatSolution individual, double mutationRate)
    {
        for (var i = 0; i < numberOfBits; i++)
        {
            var idx = random.Next(individual.Assignment.Length);
            individual.Assignment[idx] = !individual.Assignment[i];
        }
    }
}