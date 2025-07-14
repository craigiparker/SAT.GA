using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

/// <summary>
/// Fitness function that weights clauses based on difficulty and variable frequency for SAT solutions.
/// </summary>
public class WeightedMaxSatFitness : IFitnessFunction<SatSolution>
{
    /// <summary>
    /// Initializes a new instance of the WeightedMaxSatFitness class for a given SAT instance.
    /// </summary>
    /// <param name="instance">The SAT instance to use for clause weighting.</param>
    public WeightedMaxSatFitness(SatInstance instance)
    {
        _clauseWeights = CalculateClauseWeights(instance);
    }

    private readonly double[] _clauseWeights;

    /// <summary>
    /// Calculates normalized weights for each clause based on length, variable frequency, and literal distribution.
    /// </summary>
    /// <param name="instance">The SAT instance to analyze.</param>
    /// <returns>Array of normalized clause weights.</returns>
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

    /// <summary>
    /// Calculates the frequency of each variable in the SAT instance.
    /// </summary>
    /// <param name="instance">The SAT instance to analyze.</param>
    /// <returns>Dictionary mapping variable index to frequency.</returns>
    private Dictionary<int, int> CalculateVariableFrequency(SatInstance instance)
    {
        var frequency = new Dictionary<int, int>();
        
        foreach (var clause in instance.Clauses)
        {
            foreach (var literal in clause.Literals)
            {
                var variable = Math.Abs(literal);
                frequency.TryAdd(variable, 0);
                frequency[variable]++;
            }
        }

        return frequency;
    }

    /// <summary>
    /// Calculates the weighted fitness score for a SAT solution.
    /// </summary>
    /// <param name="individual">The SAT solution to evaluate.</param>
    /// <returns>The weighted fitness score.</returns>
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