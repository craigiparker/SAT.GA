using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

public class AdaptiveFitness : IFitnessFunction<SatSolution>
{
    private readonly double _initialWeight;
    private readonly double _adaptationRate;
    private Dictionary<int, double> _clauseWeights;
    
    public AdaptiveFitness(double initialWeight = 1.0, double adaptationRate = 0.1)
    {
        _initialWeight = initialWeight;
        _adaptationRate = adaptationRate;
        _clauseWeights = new Dictionary<int, double>();
    }
    
    public double Calculate(SatSolution individual)
    {
        double score = 0;
        
        for (int i = 0; i < individual.Instance.Clauses.Count; i++)
        {
            var clause = individual.Instance.Clauses[i];
            var weight = _clauseWeights.GetValueOrDefault(i, _initialWeight);
            
            if (clause.IsSatisfied(individual.Assignment))
            {
                score += weight;
                // Decrease weight for satisfied clauses
                _clauseWeights[i] = weight * (1 - _adaptationRate);
            }
            else
            {
                // Increase weight for unsatisfied clauses
                _clauseWeights[i] = weight * (1 + _adaptationRate);
            }
        }
        
        return score;
    }
} 