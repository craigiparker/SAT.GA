using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

public class DynamicWeightFitness : IFitnessFunction<SatSolution>
{
    private readonly Dictionary<int, double> _clauseWeights;
    private readonly int _historySize;
    private readonly Queue<SatSolution> _solutionHistory;
    
    public DynamicWeightFitness(int historySize = 10)
    {
        _historySize = historySize;
        _solutionHistory = new Queue<SatSolution>();
        _clauseWeights = new Dictionary<int, double>();
    }
    
    public double Calculate(SatSolution individual)
    {
        UpdateWeights(individual);
        double score = 0;
        
        for (int i = 0; i < individual.Instance.Clauses.Count; i++)
        {
            if (individual.Instance.Clauses[i].IsSatisfied(individual.Assignment))
            {
                score += _clauseWeights.GetValueOrDefault(i, 1.0);
            }
        }
        
        _solutionHistory.Enqueue(individual);
        if (_solutionHistory.Count > _historySize)
            _solutionHistory.Dequeue();
            
        return score;
    }
    
    private void UpdateWeights(SatSolution individual)
    {
        // Increase weights for clauses that are consistently unsatisfied
        for (int i = 0; i < individual.Instance.Clauses.Count; i++)
        {
            if (!individual.Instance.Clauses[i].IsSatisfied(individual.Assignment))
            {
                _clauseWeights[i] = _clauseWeights.GetValueOrDefault(i, 1.0) * 1.1;
            }
        }
    }
} 