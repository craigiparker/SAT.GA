using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Mutation;

public class BitFlipMutation : IMutationOperator<SatSolution>
{
    private readonly Random _random;

    public BitFlipMutation(Random random)
    {
        _random = random;
    }

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