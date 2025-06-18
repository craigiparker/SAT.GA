// SAT.GA/Operators/Crossover/UniformCrossover.cs
using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Crossover;

public class UniformCrossover : ICrossoverOperator<SatSolution>
{
    private readonly Random _random;

    public UniformCrossover(Random random)
    {
        _random = random;
    }

    public IEnumerable<SatSolution> Crossover(SatSolution parent1, SatSolution parent2)
    {
        var childAssignment1 = new bool[parent1.Assignment.Length];
        var childAssignment2 = new bool[parent1.Assignment.Length];

        for (int i = 0; i < childAssignment1.Length; i++)
        {
            childAssignment1[i] = _random.NextDouble() < 0.5
                ? parent1.Assignment[i]
                : parent2.Assignment[i];

            if (_random.NextDouble() < 0.5)
            {
                childAssignment1[i] = parent1.Assignment[i];
                childAssignment2[i] = parent2.Assignment[i];
            }
            else
            {
                childAssignment2[i] = parent1.Assignment[i];
                childAssignment1[i] = parent2.Assignment[i];
            }
        }

        yield return new SatSolution(parent1.Instance, childAssignment1);
        yield return new SatSolution(parent1.Instance, childAssignment2);
    }
}

