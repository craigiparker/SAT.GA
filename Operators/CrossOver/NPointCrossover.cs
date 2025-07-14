using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators.Crossover;

/// <summary>
/// Implements N-point crossover for SAT solutions, alternating segments between parents at N random points.
/// </summary>
public class NPointCrossover(int _n, Random _random) : ICrossoverOperator<SatSolution>
{
    /// <summary>
    /// Performs N-point crossover between two parent solutions to produce two offspring.
    /// </summary>
    /// <param name="parent1">The first parent solution.</param>
    /// <param name="parent2">The second parent solution.</param>
    /// <returns>An enumerable of two offspring solutions.</returns>
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