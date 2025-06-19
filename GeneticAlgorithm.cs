// SAT.GA/GeneticAlgorithm.cs
using SAT.GA.Interfaces;
using SAT.GA.Models;
using SAT.GA.Operators.Crossover;
using System.Collections.Generic;
using SAT.GA.Utils;

namespace SAT.GA;

public class GeneticAlgorithm
{
    private readonly Random _random;
    private readonly ISelectionOperator<SatSolution> _selection;
    private readonly ICrossoverOperator<SatSolution> _crossover;
    private readonly IMutationOperator<SatSolution> _mutation;
    private readonly IFitnessFunction<SatSolution> _fitness;
    private readonly ILocalSearch<SatSolution>? _localSearch;
    private readonly Dictionary<int, bool> _fixedValues = new();
    private List<ClauseStats> _experiment;
    private readonly OutputWriter _writer;

    public GeneticAlgorithm(
        ISelectionOperator<SatSolution> selection,
        ICrossoverOperator<SatSolution> crossover,
        IMutationOperator<SatSolution> mutation,
        IFitnessFunction<SatSolution> fitness,
        OutputWriter writer,
        ILocalSearch<SatSolution>? localSearch = null,
        int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
        _selection = selection;
        _crossover = crossover;
        _mutation = mutation;
        _fitness = fitness;
        _writer = writer;
        _localSearch = localSearch;
    }

    private HashSet<int> GetAllVariables(SatInstance instance)
    {
        HashSet<int> variables = new HashSet<int>();
        for (int i = 1; i <= instance.VariableCount; i++)
        {
            variables.Add(i);
        }
        // foreach (var clause in instance.Clauses)
        // {
        //     foreach (int lit in clause.Literals)
        //     {
        //         variables.Add(Math.Abs(lit));
        //     }
        // }
        return variables;
    }

    // private Dictionary<int, int> FindEquivalentLiterals(SatInstance instance)
    // {
    //     Dictionary<int, int> equivalents = new Dictionary<int, int>();
    //     HashSet<int> variables = GetAllVariables(instance);
    //
    //     foreach (int v in variables)
    //     {
    //         // Skip if already substituted
    //         if (equivalents.ContainsKey(v) || equivalents.ContainsKey(-v))
    //             continue;
    //
    //         // Check for (x ∨ ¬y) ∧ (¬x ∨ y) ⇒ x ≡ y
    //         foreach (int u in variables)
    //         {
    //             if (u == v || equivalents.ContainsKey(u) || equivalents.ContainsKey(-u))
    //                 continue;
    //
    //             var satisfied = instance.Clauses.Any(c => c.Literals.Contains(v) && c.Literals.Contains(-u));
    //             var satisfied2 = instance.Clauses.Any(c => c.Literals.Contains(-v) && c.Literals.Contains(u));
    //             var unSatisfied = instance.Clauses.Any(c => c.Literals.Contains(v) && c.Literals.Contains(u));
    //             var unSatisfied2 = instance.Clauses.Any(c => c.Literals.Contains(-v) && c.Literals.Contains(-u));
    //             var unSatisfied3 = instance.Clauses.Any(c => c.Literals.Any(l=>Math.Abs(l) ==v) && c.Literals.All(l => Math.Abs(l) != u));
    //             var unSatisfied4 = instance.Clauses.Any(c => c.Literals.Any(l=>Math.Abs(l) ==u) && c.Literals.All(l => Math.Abs(l) != v));
    //
    //             // bool hasXNegY = false, hasNegXY = false;
    //             // bool hasXY = false, hasNegXNegY = false;
    //             //
    //             // foreach (var clause in instance.Clauses)
    //             // {
    //             //     if (clause.Literals.Contains(v) && clause.Literals.Contains(-u))
    //             //         hasXNegY = true;
    //             //     if (clause.Literals.Contains(-v) && clause.Literals.Contains(u))
    //             //         hasNegXY = true;
    //             //     if (clause.Literals.Contains(u) && clause.Literals.Contains(v))
    //             //         hasXY = true;
    //             //     if (clause.Literals.Contains(-u) && clause.Literals.Contains(-v))
    //             //         hasNegXNegY = true;
    //             //
    //             //     // bool hasX = clause.Literals.Contains(v);
    //             //     // bool hasNegX = clause.Literals.Contains(-v);
    //             //     // bool hasY = clause.Literals.Contains(u);
    //             //     // bool hasNegY = clause.Literals.Contains(-v);
    //             //     //
    //             //     // if (hasX && !hasNegY) return false; // Clause allows x=true without y=true
    //             //     // if (hasNegX && !hasY) return false; // Clause allows x=false without y=false
    //             // }
    //
    //             // If (x ∨ ¬y) and (¬x ∨ y) exist, then x ≡ y
    //             //if ((hasXNegY && hasNegXY) && !(hasXY || hasNegXNegY))
    //             if (satisfied && satisfied2 && !unSatisfied && !unSatisfied2)// && !unSatisfied3 && !unSatisfied4)
    //             {
    //                 equivalents[u] = v; // Replace u with v
    //
    //                 // test
    //                 //if(v>11)
    //                  //   return equivalents;
    //
    //                 var check = instance.Clauses.Where(c =>
    //                     c.Literals.Any(l => Math.Abs(l) == u) || c.Literals.Any(l => Math.Abs(l) == v));
    //
    //                 break;
    //             }
    //         }
    //     }
    //
    //     return equivalents;
    // }
    //
    // private void SubstituteEquivalents(SatInstance instance, Dictionary<int, int> equivalents)
    // {
    //     foreach (var clause in instance.Clauses)
    //     {
    //         for (int i = 0; i < clause.Literals.Count; i++)
    //         {
    //             int lit = clause.Literals[i];
    //             if (equivalents.TryGetValue(Math.Abs(lit), out int substVar))
    //             {
    //                 clause.Literals[i] = (lit > 0) ? substVar : -substVar;
    //             }
    //         }
    //         // Remove duplicate literals after substitution
    //         clause.Literals = clause.Literals.Distinct().ToList();
    //     }
    //
    //     // Remove duplicate clauses and tautologies
    //     // instance.Clauses = instance.Clauses
    //     //     .Where(c => !IsTautology(c)) // Remove tautologies (e.g., x ∨ ¬x)
    //     //     .GroupBy(c => string.Join(",", c.Literals.OrderBy(l => l)))
    //     //     .Select(g => g.First())
    //     //     .ToList();
    // }
    //
    // // Checks if a clause is a tautology (e.g., x ∨ ¬x ∨ y)
    // private bool IsTautology(SatClause clause)
    // {
    //     return clause.Literals.Any(l => clause.Literals.Contains(-l));
    // }

    public SatSolution Solve(
        SatInstance instance,
        int populationSize,
        int generations,
        double mutationRate,
        double elitismRate = 0.1,
        CancellationToken? cancellationToken = null)
    {
        // Experiment
        //var equivalentLiterals = FindEquivalentLiterals(instance);
        // SubstituteEquivalents(instance, equivalentLiterals);

        // Experiment
        _experiment = instance.Clauses.SelectMany(c => c.Literals).GroupBy(Math.Abs)
            .Select(s => new ClauseStats{ Key=s.Key, Pos = s.Count(c => c > 0), Neg = s.Count(c => c < 0) }).OrderBy(k=>k.Key).ToList();

        // test 
        // if (experiment.Any(e => e.Neg == 0 || e.Pos == 0) || experiment.Count != instance.VariableCount)
        // {
        //     Console.WriteLine("Boom SHakalaka");
        // }

        foreach (var variable in _experiment.Where(e => e.Neg == 0 || e.Pos == 0))
        {
            _fixedValues[variable.Key - 1] = variable.Neg == 0;
        
            // simplify clauses
            instance.Clauses.RemoveAll(c => c.Literals.Exists(l => Math.Abs(l) == variable.Key));
            // foreach (var clause in instance.Clauses.Where(c => c.Literals.Exists(l => Math.Abs(l) == variable.Key)))
            // {
            //     clause.Literals = clause.Literals.Where(l => Math.Abs(l) != variable.Key).ToList();
            // }
        }

        //var investigate = _experiment.OrderBy(o => (double)o.Neg / (double)o.TotalCount).ToList();

        // Initialize population
        var population = InitializePopulation(instance, populationSize);
        var bestSolution = population.OrderByDescending(i => i.Fitness).First();
        var genLastChange = 0;
        var restartCount = 0;
        int gen;
        var bestSolutionUninterrupted = bestSolution;

        for (gen = 0; gen < generations; gen++)
        {
            if (cancellationToken is { IsCancellationRequested: true })
            {
                return null;
            }

            // Evaluate fitness
                foreach (var individual in population)
            {
                // Local search (if configured)
                if (_localSearch != null)//&& !(_crossover is LocalSearchCrossover))
                {
                    //foreach (var child in offspring)
                    {
                        _localSearch.Improve(individual, 10000); //TODO: look at max iter

                        if (individual.UnSatisifedClauses().Count == 0)
                        {
                            bestSolution = individual;
                            bestSolution.Generations = gen;
                            bestSolution.Restarts = restartCount;
                            return bestSolution;
                        }
                    }
                }

                individual.Fitness = _fitness.Calculate(individual);
            }

            // Find best solution
            var currentBest = population.OrderByDescending(i => i.Fitness).First();
            if (bestSolution == null ||  currentBest.Fitness > bestSolution.Fitness)
            {
                bestSolution = currentBest;
                genLastChange = gen;
                _writer.BestFitness = bestSolution.Fitness;
                //_writer.WriteLine("Best so far :" + bestSolution.Fitness);

                // Early exit if solution is found
                if (bestSolution.SatisfiedClausesCount() == instance.Clauses.Count)
                {
                    break;
                }
            }
            else if(gen - genLastChange > 100)
            {
                _writer.Restarts++;
                //_writer.WriteLine("Restarting");
                population = InitializePopulation(instance, populationSize);
                foreach (var individual in population)
                {
                    individual.Fitness = _fitness.Calculate(individual);
                }
                //population.Add(bestSolution); // keep 1 best solution

                genLastChange = gen;
                if (bestSolution.Fitness > bestSolutionUninterrupted.Fitness)
                {
                    bestSolutionUninterrupted = bestSolution;
                }

                bestSolution = null;
                restartCount++;
            }

            // Apply elitism
            var eliteCount = (int)(populationSize * elitismRate);
            var elites = population
                .OrderByDescending(i => i.Fitness)
                .Take(eliteCount)
                .ToList();

            // Selection
            var remainder = populationSize - eliteCount;
            if (remainder % 2 == 1) remainder++;
            var selected = _selection.Select(population, remainder);

            // Crossover
            var offspring = new List<SatSolution>();
            for (int i = 0; i < selected.Count; i += 2)
            {
                if (i + 1 >= selected.Count) break;

                var parent1 = selected[i];
                var parent2 = selected[i + 1];
                var child = _crossover.Crossover(parent1, parent2);
                offspring.AddRange(child);
            }

            // Mutation
            foreach (var child in offspring)
            {
                _mutation.Mutate(child, mutationRate);
            }

            // Local search (if configured)
            // if (_localSearch != null )//&& !(_crossover is LocalSearchCrossover))
            // {
            //     foreach (var child in offspring)
            //     {
            //         _localSearch.Improve(child, 10000); //TODO: look at max iter
            //     }
            // }

            // Exclude Duplicates
            population = elites;
            foreach (var child in offspring)
            {
                if (!population.Contains(child))
                {
                    population.Add(child);
                }
                else
                {
                    ;
                }
            }

            // Create new population
            //population = elites.Concat(offspring).ToList();

            // Maintain population size
            while (population.Count < populationSize)
            {
                population.Add(CreateRandomSolution(instance));
            }

            while (population.Count > populationSize)
            {
                population.RemoveAt(_random.Next(population.Count));
            }

            if (gen % 300 == 0)
            {
                _writer.WriteLine("Generation :" + gen);
            }

            _writer.Generation = gen;
        }

        if (bestSolution != null)
        {
            bestSolution.Generations = gen;
            bestSolution.Restarts = restartCount;
        }

        return bestSolution ?? bestSolutionUninterrupted;
    }

    private List<SatSolution> InitializePopulation(SatInstance instance, int populationSize)
    {
        //return InitializePopulationBasedOnClauses(instance, populationSize);

        var population = new List<SatSolution>();

        for (int i = 0; i < populationSize; i++)
        {
            population.Add(CreateRandomSolution(instance));
        }

        return population;
    }

    private List<SatSolution> InitializePopulationBasedOnClauses(SatInstance instance, int populationSize)
    {
        var population = new List<SatSolution>();

        for (int i = 0; i < populationSize; i++)
        {
            var assignment = new bool[instance.VariableCount];
            foreach (var variable in _experiment)
            {
                assignment[variable.Key - 1] = _random.Next(variable.Neg + variable.Pos) >= variable.Neg;
            }

            population.Add(new SatSolution(instance, assignment));
        }

        return population;
    }

    private List<SatSolution> InitializePopulationBasedOnClausesWithImplication(SatInstance instance, int populationSize)
    {
        var population = new List<SatSolution>();
        // var experiment = instance.Clauses.SelectMany(c => c.Literals).GroupBy(Math.Abs)
        //     .Select(s => new ClauseStats { Key = s.Key, Pos = s.Count(c => c > 0), Neg = s.Count(c => c < 0) }).OrderBy(k => k.Key).ToList();



        for (int i = 0; i < populationSize; i++)
        {
            var assignment = new bool[instance.VariableCount];
            var clauses = instance.Clauses.SelectMany(c=>c.Literals.Select(l=>new{c.Index ,lit=l})).ToList();
            var experiment = _experiment;
            foreach (var variable in experiment.OrderByDescending(e=>e.TotalCount))
            {
                var test = clauses.Where(c => Math.Abs(c.lit) == variable.Key).GroupBy(g=>Math.Abs(g.lit)).Select(s =>
                    new { neg = s.Count(a => a.lit < 0), pos = s.Count(a => a.lit > 0) }).FirstOrDefault();

                //var value = _random.Next(variable.Neg + variable.Pos) >= variable.Neg;
                var value = test ==null ?
                    _random.Next(variable.Neg + variable.Pos) >= variable.Neg
                    : _random.Next(test.neg + test.pos) >= variable.Neg;
                assignment[variable.Key - 1] = value;
                var matches = clauses.Where(c => Math.Abs(c.lit) == variable.Key && Math.Sign(c.lit) == (value ? 1 : -1)).ToList();
                var contradiction = clauses.Where(c => Math.Abs(c.lit) == variable.Key && Math.Sign(c.lit) == (value ? -1 : 1)).ToList();

                clauses.RemoveAll(c => matches.Exists(m => m.Index == c.Index));
                //clauses.RemoveAll(c => contradiction.Exists(m => m.lit == c.lit));
            }

            population.Add(new SatSolution(instance, assignment));
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

public class ClauseStats
{
    public int Key { get; set; }
    public int Pos { get; set; }
    public int Neg { get; set; }
    public int TotalCount => Pos + Neg;
}