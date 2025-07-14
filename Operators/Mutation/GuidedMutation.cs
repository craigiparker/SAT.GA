using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Mutation;

/// <summary>
/// Implements guided mutation by flipping a variable in a randomly chosen unsatisfied clause.
/// </summary>
public class GuidedMutation(Random random) : IMutationOperator<SatSolution>
{
    /// <summary>
    /// Mutates a SAT solution by flipping a variable in a random unsatisfied clause.
    /// </summary>
    /// <param name="individual">The SAT solution to mutate.</param>
    /// <param name="mutationRate">The probability of mutation (not used in this strategy).</param>
    public void Mutate(SatSolution individual, double mutationRate)
    {
        var instance = individual.Instance;
        
        // Find unsatisfied clauses
        var unsatisfiedClauses = instance.Clauses
            .Where(c => !c.IsSatisfied(individual.Assignment))
            .ToList();

        if (!unsatisfiedClauses.Any()) return;

        // Pick a random unsatisfied clause
        var clause = unsatisfiedClauses[random.Next(unsatisfiedClauses.Count)];

        // Flip a random variable in this clause
        var literal = clause.Literals[random.Next(clause.Literals.Count)];
        var variable = Math.Abs(literal) - 1;

        individual.Assignment[variable] = !individual.Assignment[variable];
    }
}