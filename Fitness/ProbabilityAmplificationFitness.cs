using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

/// <summary>
/// Fitness function that amplifies differences in SAT solution quality using a power function.
/// </summary>
public class ProbabilityAmplificationFitness : IFitnessFunction<SatSolution>
{
    private readonly double _amplificationFactor;

    /// <summary>
    /// Initializes a new instance of the ProbabilityAmplificationFitness class.
    /// </summary>
    /// <param name="amplificationFactor">The exponent used to amplify fitness differences.</param>
    public ProbabilityAmplificationFitness(double amplificationFactor = 2.0)
    {
        _amplificationFactor = amplificationFactor;
    }

    /// <summary>
    /// Calculates the amplified fitness score for a SAT solution.
    /// </summary>
    /// <param name="individual">The SAT solution to evaluate.</param>
    /// <returns>The amplified fitness score (between 0 and 1).</returns>
    public double Calculate(SatSolution individual)
    {
        var satisfied = individual.SatisfiedClausesCount();
        var total = individual.Instance.Clauses.Count;

        // Amplify differences in fitness
        return Math.Pow((double)satisfied / total, _amplificationFactor);
    }
}