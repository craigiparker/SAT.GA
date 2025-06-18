using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

public class PenaltyBasedFitness : IFitnessFunction<SatSolution>
{
    private readonly double _penaltyFactor;
    
    public PenaltyBasedFitness(double penaltyFactor = 0.5)
    {
        _penaltyFactor = penaltyFactor;
    }
    
    public double Calculate(SatSolution individual)
    {
        var satisfied = individual.SatisfiedClausesCount();
        var total = individual.Instance.Clauses.Count;
        var unsatisfied = total - satisfied;
        
        // Apply increasing penalty for each unsatisfied clause
        return satisfied - (_penaltyFactor * Math.Pow(unsatisfied, 2));
    }
} 