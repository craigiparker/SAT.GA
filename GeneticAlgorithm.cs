using SAT.GA.Interfaces;
using SAT.GA.Models;
using SAT.GA.Configuration;
using SAT.GA.Utils;

namespace SAT.GA;

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

    public SatSolution Solve(
        SatInstance instance,
        GaConfig config,
        CancellationToken? cancellationToken = null)
    {
        // Initialize population
        var population = generator.InitializePopulation(config.PopulationSize);
        var bestSolution = population.First();
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
            else if(currentGeneration - genLastChange > config.RestartAfter)
            {
                writer.Restarts = restartCount;
                population = generator.InitializePopulation(config.PopulationSize);
                foreach (var individual in population)
                {
                    individual.Fitness = fitness.Calculate(individual);
                }

                genLastChange = currentGeneration;
                if (bestSolution.Fitness > bestSolutionUninterrupted.Fitness)
                {
                    bestSolutionUninterrupted = bestSolution;
                }

                bestSolution = null;
                restartCount++;
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