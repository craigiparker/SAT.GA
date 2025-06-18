using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Mutation;

public class NBitMutation : IMutationOperator<SatSolution>
{
    private readonly int _numberOfBits;
    private readonly Random _random;

    public NBitMutation(int numberOfBits, Random random)
    {
        _numberOfBits = numberOfBits;
        _random = random;
    }

    public void Mutate(SatSolution individual, double mutationRate)
    {
        for (var i = 0; i < _numberOfBits; i++)
        {
            var idx = _random.NextInt64(individual.Assignment.Length);
            individual.Assignment[idx] = !individual.Assignment[i];
        }
    }
}