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

/// <summary>
/// Provides factory methods for creating genetic algorithm operators (selection, crossover, mutation, population, fitness, and local search).
/// </summary>
public static class OperatorFactory
{
    /// <summary>
    /// Creates a selection operator based on the specified type.
    /// </summary>
    /// <param name="type">The type of selection operator (e.g., Rank, Roulette, Tournament).</param>
    /// <param name="random">Random number generator.</param>
    /// <param name="config">Genetic algorithm configuration.</param>
    /// <returns>An ISelectionOperator instance.</returns>
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

    /// <summary>
    /// Creates a crossover operator based on the specified type.
    /// </summary>
    /// <param name="type">The type of crossover operator (e.g., Uniform, 1Point, 2Point, Clause).</param>
    /// <param name="random">Random number generator.</param>
    /// <param name="config">Genetic algorithm configuration.</param>
    /// <returns>An ICrossoverOperator instance.</returns>
    public static ICrossoverOperator<SatSolution> CreateCrossoverOperator(
        string type,
        Random random,
        GaConfig config)
    {
        return type switch
        {
            "Uniform" => new UniformCrossover(random),
            "1Point" => new NPointCrossover(1,random),
            "2Point" => new NPointCrossover(2,random),
            "Clause" => new ClauseSatisfactionCrossover(random),
            _ => throw new ArgumentException($"Unknown crossover type: {type}")
        };
    }

    /// <summary>
    /// Creates a mutation operator based on the specified type.
    /// </summary>
    /// <param name="type">The type of mutation operator (e.g., BitFlip, Guided, NBit, 1Bit, 2Bit).</param>
    /// <param name="random">Random number generator.</param>
    /// <param name="config">Genetic algorithm configuration.</param>
    /// <returns>An IMutationOperator instance.</returns>
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

    /// <summary>
    /// Creates a population generator based on the specified type.
    /// </summary>
    /// <param name="type">The type of population generator (e.g., Random, Clause, Diversity).</param>
    /// <param name="random">Random number generator.</param>
    /// <param name="instance">SAT instance for population generation.</param>
    /// <returns>An IPopulationGenerator instance.</returns>
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

    /// <summary>
    /// Creates a fitness function based on the specified type.
    /// </summary>
    /// <param name="type">The type of fitness function (e.g., MaxSat, Weighted, Amplified).</param>
    /// <param name="instance">SAT instance for fitness evaluation.</param>
    /// <returns>An IFitnessFunction instance.</returns>
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

    /// <summary>
    /// Creates a local search operator based on the specified type.
    /// </summary>
    /// <param name="type">The type of local search operator (e.g., None, Tabu, HillClimbing, SimulatedAnnealing).</param>
    /// <param name="random">Random number generator.</param>
    /// <param name="config">Genetic algorithm configuration.</param>
    /// <returns>An ILocalSearch instance or null if not used.</returns>
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