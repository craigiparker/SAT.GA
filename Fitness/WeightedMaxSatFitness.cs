using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

public class WeightedMaxSatFitness : IFitnessFunction<SatSolution>
{
    private readonly double[] _clauseWeights;
    private readonly Dictionary<int, int> _variableFrequency;

    public WeightedMaxSatFitness(SatInstance instance)
    {
        _clauseWeights = CalculateClauseWeights(instance);
        _variableFrequency = CalculateVariableFrequency(instance);
    }

    private double[] CalculateClauseWeights(SatInstance instance)
    {
        var weights = new double[instance.Clauses.Count];
        var variableFrequency = CalculateVariableFrequency(instance);

        for (int i = 0; i < instance.Clauses.Count; i++)
        {
            var clause = instance.Clauses[i];
            
            // Base weight on clause length (more literals = harder to satisfy)
            double weight = 1.0 + (1.0 / clause.Literals.Count);
            
            // Adjust weight based on variable frequency
            foreach (var literal in clause.Literals)
            {
                var variable = Math.Abs(literal);
                // Variables that appear less frequently are harder to satisfy
                weight += 1.0 / (variableFrequency[variable] + 1);
            }

            // Adjust weight based on literal distribution
            int positiveCount = clause.Literals.Count(l => l > 0);
            int negativeCount = clause.Literals.Count - positiveCount;
            // Clauses with imbalanced positive/negative literals are harder
            weight += Math.Abs(positiveCount - negativeCount) * 0.5;

            weights[i] = weight;
        }

        // Normalize weights to sum to 1
        double sum = weights.Sum();
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] /= sum;
        }

        return weights;
    }

    private Dictionary<int, int> CalculateVariableFrequency(SatInstance instance)
    {
        var frequency = new Dictionary<int, int>();
        
        foreach (var clause in instance.Clauses)
        {
            foreach (var literal in clause.Literals)
            {
                var variable = Math.Abs(literal);
                if (!frequency.ContainsKey(variable))
                {
                    frequency[variable] = 0;
                }
                frequency[variable]++;
            }
        }

        return frequency;
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