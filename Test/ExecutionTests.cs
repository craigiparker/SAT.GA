using System.Diagnostics;
using SAT.GA.Configuration;
using SAT.GA.Factories;
using SAT.GA.Utils;
using Xunit;
using Xunit.Abstractions;

namespace SAT.GA.Test;

/// <summary>
/// Execution Tests for steps 1 to 3 to find best GA combinations
/// </summary>
/// <param name="testOutputHelper">Logging Results</param>
public class ExecutionTests(ITestOutputHelper testOutputHelper)
{

    /// <summary>
    /// For the benchmark uf50-218, test all permutations of pure GA operations, identifying the most
    /// promising configurations for random instances
    /// </summary>
    [Fact]
    public void ExecutionPlan1()
    {
        var config = new GaConfig
        {
            PopulationSize = 100,
            ElitismRate = 0.1,
            Generations = 10000,
            MutationRate = 0.01,
            UseLocalSearch = false,
            RandomSeed = 2025,
            FileCountLimit = 50
        };

        string[] selectionTypes = ["Tournament", "Roulette", "Rank"];
        string[] crossoverTypes = ["Clause", "Uniform", "1Point", "2Point"];
        string[] mutationTypes = ["BitFlip", "Guided", "1Bit", "2Bit"];
        string[] fitnessTypes = ["MaxSat", "Weighted", "Amplified"];
        string[] generationTypes = ["Random"];
        string[] localSearchTypes = ["None"];
        ExecutePlan("uf50-218", config, selectionTypes, crossoverTypes, mutationTypes, fitnessTypes, generationTypes, localSearchTypes);
    }

    /// <summary>
    ///  For the benchmark flat50-115, test all permutations of pure GA operations, identifying the most
    /// promising configurations for graph colouring(structured) instances
    /// </summary>
    [Fact]
    public void ExecutionPlan2()
    {
        var config = new GaConfig
        {
            PopulationSize = 100,
            ElitismRate = 0.1,
            Generations = 10000,
            MutationRate = 0.01,
            UseLocalSearch = false,
            RandomSeed = 2025,
            FileCountLimit = 50
        };

        string[] selectionTypes = ["Tournament", "Roulette", "Rank"];
        string[] crossoverTypes = ["Clause", "Uniform", "1Point", "2Point"];
        string[] mutationTypes = ["BitFlip", "Guided", "1Bit", "2Bit"];
        string[] fitnessTypes = ["MaxSat", "Weighted", "Amplified"];
        string[] generationTypes = ["Random"];
        string[] localSearchTypes = ["None"];
        ExecutePlan("flat50-115", config, selectionTypes, crossoverTypes, mutationTypes, fitnessTypes, generationTypes, localSearchTypes);
    }

    /// <summary>
    /// Compare different population initialisation methods on random instances with 100 variables, using
    /// the best options found in previous steps
    /// </summary>
    [Fact]
    public void ExecutionPlan3()
    {
        var config = new GaConfig
        {
            PopulationSize = 100,
            ElitismRate = 0.1,
            Generations = 10000,
            MutationRate = 0.01,
            UseLocalSearch = false,
            RandomSeed = 2025,
            FileCountLimit = 100
        };

        string[] selectionTypes = ["Tournament"];
        string[] crossoverTypes = ["Clause", "Uniform", "1Point", "2Point"];
        string[] mutationTypes = ["Guided"];
        string[] fitnessTypes = ["MaxSat", "Weighted", "Amplified"];
        string[] generationTypes = ["Random", "Clause", "Diversity"];
        string[] localSearchTypes = ["None"];
        ExecutePlan("uf100-430", config, selectionTypes, crossoverTypes, mutationTypes, fitnessTypes, generationTypes, localSearchTypes);
    }
    
    private void ExecutePlan(string path, GaConfig config, string[] selectionTypes, 
                             string[] crossoverTypes, string[] mutationTypes, string[] fitnessTypes, 
                             string[] generationTypes, string[] localSearchTypes)
    {
        var files = Directory.Exists(path) ? Directory.GetFiles(path) : [path];

        foreach (var filePath in files.OrderBy(f=>f).Take(config.FileCountLimit))
        {
            var file = File.OpenText(filePath);
            var fileName = Path.GetFileName(filePath);
            var cnfContent = file.ReadToEnd();

            // Parse the CNF
            var parser = new DimacsParser();
            var instance = parser.Parse(cnfContent);

            foreach (var selection in selectionTypes)
            foreach (var crossover in crossoverTypes)
            foreach (var mutation in mutationTypes)
            foreach (var fitness in fitnessTypes)
            foreach (var generationType in generationTypes)
            foreach(var search in localSearchTypes)

            {
                var random = config.RandomSeed.HasValue
                    ? new Random(config.RandomSeed.Value)
                    : new Random();

                bool solved = false;

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                if (Program.RunInstance(
                        OperatorFactory.CreateSelectionOperator(selection, random, config),
                        OperatorFactory.CreateCrossoverOperator(crossover, random,  config),
                        OperatorFactory.CreateMutationOperator(mutation, random, config),
                        OperatorFactory.CreateFitnessFunction(fitness, instance),
                        OperatorFactory.CreateLocalSearch(search, random,config),
                        OperatorFactory.CreatePopulationGenerator(generationType, random, instance), 
                        config, 
                        instance,
                        new OutputWriter{HideOutput = true},
                        out var solution))
                {
                    solved = true;
                }

                stopWatch.Stop();
                // Log Results for combination
                testOutputHelper.WriteLine($"{fileName},{selection},{crossover},{mutation},{fitness},{solved},{solution.Generations},{solution.Restarts},{stopWatch.Elapsed.TotalMilliseconds:0}");
            }
        }
    }
}