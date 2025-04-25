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

    public SatSolution Crossover(SatSolution parent1, SatSolution parent2)
    {
        var childAssignment = new bool[parent1.Assignment.Length];

        for (int i = 0; i < childAssignment.Length; i++)
        {
            childAssignment[i] = _random.NextDouble() < 0.5
                ? parent1.Assignment[i]
                : parent2.Assignment[i];
        }

        return new SatSolution(parent1.Instance, childAssignment);
    }
}

