using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

public class ClauseSearch : ILocalSearch<SatSolution>
{
    public void Improve(SatSolution individual, int maxIterations)
    {
        var assignment = (bool[])individual.Assignment.Clone();
        var currentCount = individual.Instance.SatisfiedCount(assignment);
        var possibleTweaks = individual.UnSatisifedClauses().SelectMany(c => c.Literals).Distinct().ToList();
        foreach (var tweak in possibleTweaks)
        {
            var idx = Math.Abs(tweak) - 1;
            var sign = tweak > 0;

            assignment[idx] = !assignment[idx];
            if (individual.Instance.SatisfiedCount(assignment) < currentCount)
            {
                individual.Assignment = assignment;

                if (individual.Instance.IsSatisfied(assignment)) 
                    return;
            }
            else
            {
                assignment[idx] = !assignment[idx];
            }
        }
    }
}