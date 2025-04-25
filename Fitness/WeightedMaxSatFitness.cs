using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

public class WeightedMaxSatFitness : IFitnessFunction<SatSolution>
{
    private readonly double[] _clauseWeights;

    public WeightedMaxSatFitness(double[] clauseWeights)
    {
        _clauseWeights = clauseWeights;
    }

    public double Calculate(SatSolution individual)
    {
        double score = 0;
        var clauses = individual.Instance.Clauses;

        for (int i = 0; i < clauses.Count; i++)
        {
            if (clauses[i].IsSatisfied(individual.Assignment))
            {
                score += _clauseWeights[i];
            }
        }

        return score;
    }
}