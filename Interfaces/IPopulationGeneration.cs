using SAT.GA.Models;

namespace SAT.GA.Interfaces;

public interface IPopulationGenerator
{
    SatSolution CreateNewIndividual();
    List<SatSolution> InitializePopulation(int populationSize);
    void OverrideSolution(SatSolution solution);
}