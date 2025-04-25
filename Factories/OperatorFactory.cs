// SAT.GA/Factories/OperatorFactory.cs
using SAT.GA.Configuration;
using SAT.GA.Fitness;
using SAT.GA.Interfaces;
using SAT.GA.LocalSearch;
using SAT.GA.Models;
using SAT.GA.Operators.Crossover;
using SAT.GA.Operators.Mutation;
using SAT.GA.Operators.Selection;

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
            "LocalSearch" => new LocalSearchCrossover(random, localSearch!, config.CrossoverRate),
            _ => throw new ArgumentException($"Unknown crossover type: {type}")
        };
    }

    public static IMutationOperator<SatSolution> CreateMutationOperator(
        string type,
        Random random)
    {
        return type switch
        {
            "BitFlip" => new BitFlipMutation(random),
            "Guided" => new GuidedMutation(random),
            _ => throw new ArgumentException($"Unknown mutation type: {type}")
        };
    }

    public static IFitnessFunction<SatSolution> CreateFitnessFunction(
        string type,
        SatInstance instance)
    {
        return type switch
        {
            "MaxSat" => new MaxSatFitness(),
            "Weighted" => new WeightedMaxSatFitness(
                Enumerable.Repeat(1.0, instance.Clauses.Count).ToArray()),
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
            "Tabu" => new TabuSearch(random, config.TabuTenure),
            "HillClimbing" => new HillClimbing(random),
            _ => throw new ArgumentException($"Unknown local search type: {type}")
        };
    }
}