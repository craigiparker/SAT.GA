// SAT.GA/GeneticAlgorithm.cs
using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA;

public class GeneticAlgorithm
{
    private readonly Random _random;
    private readonly ISelectionOperator<SatSolution> _selection;
    private readonly ICrossoverOperator<SatSolution> _crossover;
    private readonly IMutationOperator<SatSolution> _mutation;
    private readonly IFitnessFunction<SatSolution> _fitness;
    private readonly ILocalSearch<SatSolution>? _localSearch;

    public GeneticAlgorithm(
        ISelectionOperator<SatSolution> selection,
        ICrossoverOperator<SatSolution> crossover,
        IMutationOperator<SatSolution> mutation,
        IFitnessFunction<SatSolution> fitness,
        ILocalSearch<SatSolution>? localSearch = null,
        int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
        _selection = selection;
        _crossover = crossover;
        _mutation = mutation;
        _fitness = fitness;
        _localSearch = localSearch;
    }

    public SatSolution Solve(
        SatInstance instance,
        int populationSize,
        int generations,
        double mutationRate,
        double elitismRate = 0.1)
    {
        // Initialize population
        var population = InitializePopulation(instance, populationSize);
        var bestSolution = population.OrderByDescending(i => i.Fitness).First();

        for (int gen = 0; gen < generations; gen++)
        {
            // Evaluate fitness
            foreach (var individual in population)
            {
                individual.Fitness = _fitness.Calculate(individual);
            }

            // Find best solution
            var currentBest = population.OrderByDescending(i => i.Fitness).First();
            if (currentBest.Fitness > bestSolution.Fitness)
            {
                bestSolution = currentBest;

                // Early exit if solution is found
                if (bestSolution.SatisfiedClausesCount() == instance.Clauses.Count)
                {
                    break;
                }
            }

            // Apply elitism
            var eliteCount = (int)(populationSize * elitismRate);
            var elites = population
                .OrderByDescending(i => i.Fitness)
                .Take(eliteCount)
                .ToList();

            // Selection
            var selected = _selection.Select(population, populationSize - eliteCount);

            // Crossover
            var offspring = new List<SatSolution>();
            for (int i = 0; i < selected.Count; i += 2)
            {
                if (i + 1 >= selected.Count) break;

                var parent1 = selected[i];
                var parent2 = selected[i + 1];
                var child = _crossover.Crossover(parent1, parent2);
                offspring.Add(child);
            }

            // Mutation
            foreach (var child in offspring)
            {
                _mutation.Mutate(child, mutationRate);
            }

            // Local search (if configured)
            if (_localSearch != null)
            {
                foreach (var child in offspring)
                {
                    _localSearch.Improve(child, 10);
                }
            }

            // Create new population
            population = elites.Concat(offspring).ToList();

            // Maintain population size
            while (population.Count < populationSize)
            {
                population.Add(CreateRandomSolution(instance));
            }

            while (population.Count > populationSize)
            {
                population.RemoveAt(_random.Next(population.Count));
            }
        }

        return bestSolution;
    }

    private List<SatSolution> InitializePopulation(SatInstance instance, int populationSize)
    {
        var population = new List<SatSolution>();

        for (int i = 0; i < populationSize; i++)
        {
            population.Add(CreateRandomSolution(instance));
        }

        return population;
    }

    private SatSolution CreateRandomSolution(SatInstance instance)
    {
        var assignment = new bool[instance.VariableCount];

        for (int i = 0; i < assignment.Length; i++)
        {
            assignment[i] = _random.NextDouble() < 0.5;
        }

        return new SatSolution(instance, assignment);
    }
}