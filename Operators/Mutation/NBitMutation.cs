using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Mutation;

public class NBitMutation(int numberOfBits, Random random) : IMutationOperator<SatSolution>
{
    public void Mutate(SatSolution individual, double mutationRate)
    {
        for (var i = 0; i < numberOfBits; i++)
        {
            var idx = random.Next(individual.Assignment.Length);
            individual.Assignment[idx] = !individual.Assignment[i];
        }
    }
}