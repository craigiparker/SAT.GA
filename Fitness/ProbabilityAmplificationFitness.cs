using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

public class ProbabilityAmplificationFitness : IFitnessFunction<SatSolution>
{
    private readonly double _amplificationFactor;

    public ProbabilityAmplificationFitness(double amplificationFactor = 2.0)
    {
        _amplificationFactor = amplificationFactor;
    }

    public double Calculate(SatSolution individual)
    {
        var satisfied = individual.SatisfiedClausesCount();
        var total = individual.Instance.Clauses.Count;

        // Amplify differences in fitness
        return Math.Pow((double)satisfied / total, _amplificationFactor);
    }
}