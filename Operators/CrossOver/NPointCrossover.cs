using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Crossover;

public class NPointCrossover(int n, Random random) : ICrossoverOperator<SatSolution>
{
    private readonly Random _random = random;
    private readonly int _n = n;

    public IEnumerable<SatSolution> Crossover(SatSolution parent1, SatSolution parent2)
    {
        var points = new List<int>();
        for (var i = 0; i < _n; i++)
        {
            points.Add(_random.Next(parent1.Instance.VariableCount));
        }
        points.Add(parent1.Instance.VariableCount);

        var childAssignment1 = new bool[parent1.Assignment.Length];
        var childAssignment2 = new bool[parent1.Assignment.Length];

        int index = 0;
        var top = parent1;
        var bottom = parent2;
        foreach (var point in points.OrderBy(p => p))
        {
            for (var i = index; i < point; i++)
            {
                childAssignment1[i] = top.Assignment[i];
                childAssignment2[i] = bottom.Assignment[i];
                index = i;
            }

            (top, bottom) = (bottom, top);
        }

        yield return new SatSolution(parent1.Instance, childAssignment1);
        yield return new SatSolution(parent1.Instance, childAssignment2);
    }
}