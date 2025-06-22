using SAT.GA.Models;

namespace SAT.GA.Population;

public class RandomPopulationGenerator(SatInstance _instance, Random _random) : PopulationGenerator
{
    public override SatSolution CreateNewIndividual()
    {
        var assignment = new bool[_instance.VariableCount];

        for (int i = 0; i < assignment.Length; i++)
        {
            assignment[i] = _random.NextDouble() < 0.5;
        }

        return new SatSolution(_instance, assignment);
    }
}