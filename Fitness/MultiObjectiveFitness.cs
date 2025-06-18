using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

public class MultiObjectiveFitness : IFitnessFunction<SatSolution>
{
    private readonly double _satisfactionWeight;
    private readonly double _simplicityWeight;
    
    public MultiObjectiveFitness(double satisfactionWeight = 0.7, double simplicityWeight = 0.3)
    {
        _satisfactionWeight = satisfactionWeight;
        _simplicityWeight = simplicityWeight;
    }
    
    public double Calculate(SatSolution individual)
    {
        var satisfactionScore = (double)individual.SatisfiedClausesCount() / individual.Instance.Clauses.Count;
        var simplicityScore = CalculateSimplicityScore(individual);
        
        return (_satisfactionWeight * satisfactionScore) + (_simplicityWeight * simplicityScore);
    }
    
    private double CalculateSimplicityScore(SatSolution individual)
    {
        // Reward solutions that use fewer variable changes
        int changes = 0;
        for (int i = 1; i < individual.Assignment.Length; i++)
        {
            if (individual.Assignment[i] != individual.Assignment[i-1])
                changes++;
        }
        
        return 1.0 - ((double)changes / individual.Assignment.Length);
    }
} 