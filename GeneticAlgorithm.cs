using SAT.GA.Interfaces;
using SAT.GA.Models;
using SAT.GA.Configuration;
using SAT.GA.Utils;

namespace SAT.GA;

/// <summary>
/// Implements a genetic algorithm for solving SAT problems using configurable operators and local search.
/// </summary>
public class GeneticAlgorithm(
    ISelectionOperator<SatSolution> selection,
    ICrossoverOperator<SatSolution> crossover,
    IMutationOperator<SatSolution> mutation,
    IFitnessFunction<SatSolution> fitness,
    IPopulationGenerator generator,
    OutputWriter writer,
    ILocalSearch<SatSolution>? localSearch = null,
    int? seed = null)
{
    private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();

    /// <summary>
    /// Solves the given SAT instance using the genetic algorithm and provided configuration.
    /// </summary>
    /// <param name="instance">The SAT instance to solve.</param>
    /// <param name="config">The genetic algorithm configuration parameters.</param>
    /// <param name="cancellationToken">Optional cancellation token to stop the algorithm early.</param>
    /// <returns>The best found SAT solution, or null if cancelled.</returns>
    public SatSolution Solve(
        SatInstance instance,
        GaConfig config,
        CancellationToken? cancellationToken = null)
    {
        // Initialize population
        var population = generator.InitializePopulation(config.PopulationSize);
        var bestSolution = population[0];
        var genLastChange = 0;
        var restartCount = 0;
        int currentGeneration = 0;
        var bestSolutionUninterrupted = bestSolution;

        for (currentGeneration = 0; currentGeneration < config.Generations; currentGeneration++)
        {
            if (cancellationToken is { IsCancellationRequested: true })
            {
                return null;
            }

            if (currentGeneration - genLastChange > config.RestartAfter)
            {
                writer.Restarts = restartCount;
                population = generator.InitializePopulation(config.PopulationSize);
                bestSolution = null;
                restartCount++;
            }

            // Evaluate fitness
            foreach (var individual in population)
            {
                individual.Fitness = fitness.Calculate(individual);
            }

            // Find best solution
            var currentBest = population.OrderByDescending(i => i.Fitness).First();
            if (bestSolution == null ||  currentBest.Fitness > bestSolution.Fitness)
            {
                bestSolution = currentBest;
                genLastChange = currentGeneration;
                writer.BestFitness = bestSolution.Fitness;

                // Early exit if solution is found
                if (bestSolution.IsSolution)
                {
                    generator.OverrideSolution(bestSolution);
                    break;
                }
            }

            // Apply elitism
            var eliteCount = (int)(config.PopulationSize * config.ElitismRate);
            var elites = population
                .OrderByDescending(i => i.Fitness)
                .Take(eliteCount)
                .ToList();
            // Selection
            var remainder = config.PopulationSize - eliteCount;
            if (remainder % 2 == 1) remainder++;
            var selected = selection.Select(population, remainder);

            // Crossover
            var offspring = new List<SatSolution>();
            for (int i = 0; i < selected.Count; i += 2)
            {
                if (i + 1 >= selected.Count) break;

                var parent1 = selected[i];
                var parent2 = selected[i + 1];
                var children = crossover.Crossover(parent1, parent2);
                offspring.AddRange(children);

                if (DoLocalSearch(children, currentGeneration, restartCount, out var solution)) return solution;
            }

            // Mutation
            foreach (var child in offspring)
            {
                mutation.Mutate(child, config.MutationRate);
            }

            // Exclude Duplicates
            population = elites;
            foreach (var child in offspring)
            {
                if (!population.Contains(child))
                {
                    population.Add(child);
                }
            }

            // Maintain population size
            while (population.Count < config.PopulationSize)
            {
                population.Add(generator.CreateNewIndividual());
            }

            while (population.Count > config.PopulationSize)
            {
                population.RemoveAt(_random.Next(population.Count));
            }

            if (currentGeneration % 300 == 0)
            {
                // Update Writer periodically
                writer.WriteLine("Generation :" + currentGeneration);
            }

            writer.Generation = currentGeneration;
        }

        if (bestSolution == null) return bestSolution ?? bestSolutionUninterrupted;

        bestSolution.Generations = currentGeneration;
        bestSolution.Restarts = restartCount;
        return bestSolution;
    }

    /// <summary>
    /// Applies local search to a population and checks for a satisfying solution.
    /// </summary>
    /// <param name="population">The population to improve with local search.</param>
    /// <param name="currentGeneration">The current generation number.</param>
    /// <param name="restartCount">The number of restarts performed so far.</param>
    /// <param name="solve">Outputs the found solution if one is found.</param>
    /// <returns>True if a satisfying solution is found, otherwise false.</returns>
    private bool DoLocalSearch(IEnumerable<SatSolution> population, int currentGeneration, int restartCount, out SatSolution solve)
    {
        SatSolution bestSolution;
        if (localSearch != null)
        {
            foreach (var individual in population)
            {
                localSearch.Improve(individual, 30);
                if (individual.IsSolution)
                {
                    bestSolution = individual;
                    bestSolution.Generations = currentGeneration;
                    bestSolution.Restarts = restartCount;
                    solve = bestSolution;
                    return true;
                }
            }
        }

        solve = null;
        return false;
    }
}