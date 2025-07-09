using SAT.GA.Configuration;
using SAT.GA.Fitness;
using SAT.GA.Interfaces;
using SAT.GA.LocalSearch;
using SAT.GA.Models;
using SAT.GA.Operators.Crossover;
using SAT.GA.Operators.Mutation;
using SAT.GA.Operators.Selection;
using SAT.GA.Population;

namespace SAT.GA.Factories;

public static class OperatorFactory
{
    public static ISelectionOperator<SatSolution> CreateSelectionOperator(
        string type,
        Random random,
        GaConfig config)
    {
        return type switch
        {
            "Rank" => new RankSelection(random),
            "Roulette" => new RouletteWheelSelection(random),
            "Tournament" => new TournamentSelection(random, config.TournamentSize),
            _ => throw new ArgumentException($"Unknown selection type: {type}")
        };
    }

    public static ICrossoverOperator<SatSolution> CreateCrossoverOperator(
        string type,
        Random random,
        ILocalSearch<SatSolution>? localSearch,
        GaConfig config)
    {
        return type switch
        {
            "Uniform" => new UniformCrossover(random),
            "1Point" => new NPointCrossover(1,random),
            "2Point" => new NPointCrossover(2,random),
            "Clause" => new ClauseSatisfactionCrossover(random, localSearch),
            _ => throw new ArgumentException($"Unknown crossover type: {type}")
        };
    }

    public static IMutationOperator<SatSolution> CreateMutationOperator(
        string type,
        Random random, GaConfig config)
    {
        return type switch
        {
            "BitFlip" => new BitFlipMutation(random),
            "Guided" => new GuidedMutation(random),
            "NBit" => new NBitMutation(config.MutationBits, random),
            "1Bit" => new NBitMutation(1, random),
            "2Bit" => new NBitMutation(2, random),
            _ => throw new ArgumentException($"Unknown mutation type: {type}")
        };
    }

    public static IPopulationGenerator CreatePopulationGenerator(
        string type,
        Random random, SatInstance instance)
    {
        return type switch
        {
            "Random" => new RandomPopulationGenerator(instance, random),
            "Clause" => new ClauseProbabilityPopulationGenerator(instance, random),
            "Diversity" => new DiversityBasedPopulationGenerator(instance, random),
            _ => throw new ArgumentException($"Unknown population generator type: {type}")
        };
    }

    public static IFitnessFunction<SatSolution> CreateFitnessFunction(
        string type,
        SatInstance instance)
    {
        return type switch
        {
            "MaxSat" => new MaxSatFitness(),
            "Weighted" => new WeightedMaxSatFitness(instance),
            "Amplified" => new ProbabilityAmplificationFitness(),
            _ => throw new ArgumentException($"Unknown fitness type: {type}")
        };
    }

    public static ILocalSearch<SatSolution>? CreateLocalSearch(
        string type,
        Random random,
        GaConfig config)
    {
        if (!config.UseLocalSearch) return null;

        return type switch
        {
            "None" => null,
            "Tabu" => new TabuSearch(random, config.TabuTenure),
            "HillClimbing" => new HillClimbing(random),
            "SimulatedAnnealing" => new SimulatedAnnealing(random, initialTemperature: 100.0, coolingRate: 0.95),
            
            _ => throw new ArgumentException($"Unknown local search type: {type}")
        };
    }
}