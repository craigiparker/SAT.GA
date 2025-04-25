using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Mutation;

public class GuidedMutation : IMutationOperator<SatSolution>
{
    private readonly Random _random;

    public GuidedMutation(Random random)
    {
        _random = random;
    }

    public void Mutate(SatSolution individual, double mutationRate)
    {
        var instance = individual.Instance;

        // Find unsatisfied clauses
        var unsatisfiedClauses = instance.Clauses
            .Where(c => !c.IsSatisfied(individual.Assignment))
            .ToList();

        if (!unsatisfiedClauses.Any()) return;

        // Pick a random unsatisfied clause
        var clause = unsatisfiedClauses[_random.Next(unsatisfiedClauses.Count)];

        // Flip a random variable in this clause
        var literal = clause.Literals[_random.Next(clause.Literals.Count)];
        var variable = Math.Abs(literal) - 1;

        individual.Assignment[variable] = !individual.Assignment[variable];
    }
}