// SAT.GA/Fitness/MaxSatFitness.cs
using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

public class MaxSatFitness : IFitnessFunction<SatSolution>
{
    public double Calculate(SatSolution individual)
    {
        return individual.SatisfiedClausesCount();
    }
}

// SAT.GA/Fitness/WeightedMaxSatFitness.cs

// SAT.GA/Fitness/ProbabilityAmplificationFitness.cs