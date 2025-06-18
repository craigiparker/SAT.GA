using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

public class HillClimbing : ILocalSearch<SatSolution>
{
    private readonly Random _random;

    public HillClimbing(Random random)
    {
        _random = random;
    }

    public void Improve(SatSolution individual, int maxIterations)
    {
        var currentFitness = individual.SatisfiedClausesCount();

        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Randomly select a variable to flip
            var flipIndex = _random.Next(individual.Assignment.Length);
            individual.Assignment[flipIndex] = !individual.Assignment[flipIndex];

            var newFitness = individual.SatisfiedClausesCount();

            // Revert if not improving
            if (newFitness <= currentFitness)
            {
                individual.Assignment[flipIndex] = !individual.Assignment[flipIndex];
            }
            else
            {
                currentFitness = newFitness;
            }
        }
    }
}

public class ClauseSearch : ILocalSearch<SatSolution>
{
    public void Improve(SatSolution individual, int maxIterations)
    {
        var assignment = (bool[])individual.Assignment.Clone();
        var currentCount = individual.Instance.SatisfiedCount(assignment);
        if (currentCount > 400)
        {
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
}