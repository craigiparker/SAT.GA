using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Population;

public abstract class PopulationGenerator : IPopulationGenerator
{
    public abstract SatSolution CreateNewIndividual();

    public virtual void OverrideSolution(SatSolution solution)
    {
    }

    public List<SatSolution> InitializePopulation(int populationSize)
    {
        var population = new List<SatSolution>();

        for (int i = 0; i < populationSize; i++)
        {
            population.Add(CreateNewIndividual());
        }

        return population;
    }

    
}