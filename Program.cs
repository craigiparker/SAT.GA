using System.Diagnostics;
using SAT.GA.Configuration;
using SAT.GA.Factories;
using SAT.GA.Interfaces;
using SAT.GA.Models;
using SAT.GA.Utils;

namespace SAT.GA;

/// <summary>
/// Entry point and main control logic for the SAT genetic algorithm application.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point. Parses arguments, processes files, and prints summary.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    private static void Main(string[] args)
    {
        var config = GenerateConfigFromArgs(args, out var files);

        int solvedCount = 0;
        int total = 0;
        var metrics = new List<double>();
        var solutions = new List<SatSolution>();

        var writer = new OutputWriter{HideOutput = config.HideOutput};
        var parser = new DimacsParser();

        foreach (var filePath in files.Take(config.FileCountLimit))
        {
            ProcessCnfFile(writer, filePath, parser, config, metrics, solutions, ref total, ref solvedCount);
        }

        PrintSummary(metrics, solutions, writer, solvedCount, total);
        Console.ReadKey();

    }

    /// <summary>
    /// Processes a single CNF file: parses, runs the GA, and records metrics.
    /// </summary>
    /// <param name="writer">Output writer for results and logs.</param>
    /// <param name="filePath">Path to the CNF file.</param>
    /// <param name="parser">DIMACS parser instance.</param>
    /// <param name="config">GA configuration.</param>
    /// <param name="metrics">List to store timing metrics.</param>
    /// <param name="solutions">List to store found solutions.</param>
    /// <param name="total">Reference to the total file count.</param>
    /// <param name="solvedCount">Reference to the solved file count.</param>
    private static void ProcessCnfFile(OutputWriter writer, string filePath, DimacsParser parser, GaConfig config,
        List<double> metrics, List<SatSolution> solutions, ref int total, ref int solvedCount)
    {
        total++;
        writer.FilePath = filePath;
        if (config.HideOutput)
        {
            Console.WriteLine($"Running file - {filePath}");
        }
        else
        {
            writer.WriteLine($"Running file - {filePath}");
        }

        var file = File.OpenText(filePath);

        // Parse the CNF
        var instance = parser.Parse(file.ReadToEnd());

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        writer.StopWatch = stopWatch;
        if (RunInstance(instance, writer, config, out var solution)) solvedCount++;
            
        stopWatch.Stop();

        if (config.HideOutput)
        {
            Console.WriteLine("Time taken: " + stopWatch.Elapsed.TotalMilliseconds + "ms");
        }
        else
        {
            writer.WriteLine("Time taken: " + stopWatch.Elapsed.TotalMilliseconds + "ms");
        }

        metrics.Add(stopWatch.Elapsed.TotalMilliseconds);
        solutions.Add(solution);

        if (total != config.FileCountLimit)
        {
            writer.ResetInstance();
        }
    }

    /// <summary>
    /// Generates a GaConfig object from command-line arguments and outputs the list of files to process.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <param name="files">Outputs the list of files to process.</param>
    /// <returns>The parsed GaConfig object.</returns>
    private static GaConfig GenerateConfigFromArgs(string[] args, out string[] files)
    {
        string? path = null;
        if (args.Length > 0)
        {
            path = args[0];
        }
        else
        {
            args = ["help", "-h"];
        }

        var config = GaConfigParser.Parse(args);

        files = [];
        if (Directory.Exists(path))
            files = Directory.GetFiles(path);
        else if(File.Exists(path))
            files = [path];
        else
        {
            Console.WriteLine($"File {path} does not exist.");
            Environment.Exit(0);
        }

        return config;
    }

    /// <summary>
    /// Prints a summary of the results after all files are processed.
    /// </summary>
    /// <param name="metrics">List of timing metrics.</param>
    /// <param name="solutions">List of found solutions.</param>
    /// <param name="writer">Output writer for results and logs.</param>
    /// <param name="solvedCount">Number of solved files.</param>
    /// <param name="total">Total number of files processed.</param>
    private static void PrintSummary(List<double> metrics, List<SatSolution> solutions, OutputWriter writer, int solvedCount, int total)
    {
        var averageTimeSeconds = metrics.Average() / 1000;
        var maxTimeSeconds = metrics.Max() / 1000;
        var minTimeSeconds = metrics.Min() / 1000;
        var averageGenerations = solutions.Select(s => s.Generations).Average();
        var maxGenerations = solutions.Max(s => s.Generations);
        var minGenerations = solutions.Min(s => s.Generations);
        var averageRestarts = solutions.Select(s => s.Restarts).Average();
        var maxRestarts = solutions.Max(s => s.Restarts);
        var minRestarts = solutions.Min(s => s.Restarts);
        writer.WriteCompletion($"Solved {solvedCount} of {total}.\r\nAverage Time: {averageTimeSeconds:F3}s. " +
                               $"Average Generations: {averageGenerations:F2}. Average Restarts: {averageRestarts:F2}. \r\n" +
                               $"Min Time: {minTimeSeconds:F3}s. Max Time: {maxTimeSeconds:F3}s. " +
                               $"Min Generations: {minGenerations}. Max Generations: {maxGenerations}. " +
                               $"Min Restarts: {minRestarts}. Max Restarts: {maxRestarts} \r\n" +
                               $"Press any key to exit.");
    }

    /// <summary>
    /// Runs the genetic algorithm for a single SAT instance using the provided configuration and operators.
    /// </summary>
    /// <param name="instance">The SAT instance to solve.</param>
    /// <param name="writer">Output writer for results and logs.</param>
    /// <param name="config">GA configuration.</param>
    /// <param name="solution">Outputs the found solution.</param>
    /// <returns>True if a satisfying solution is found, otherwise false.</returns>
    private static bool RunInstance(SatInstance instance, OutputWriter writer, GaConfig config, out SatSolution solution)
    {
        writer.VariableCount = instance.VariableCount;
        writer.ClauseCount = instance.Clauses.Count;
        writer.WriteLine($"There are {instance.VariableCount} variables and {instance.Clauses.Count} CNF clauses");

        var random = config.RandomSeed.HasValue
            ? new Random(config.RandomSeed.Value)
            : new Random();

        // Create operators
        var localSearch = OperatorFactory.CreateLocalSearch(config.LocalSearchMethod, random, config);
        var selection = OperatorFactory.CreateSelectionOperator(config.SelectionOperator, random, config);
        var crossover = OperatorFactory.CreateCrossoverOperator(config.CrossoverOperator, random, config);
        var mutation = OperatorFactory.CreateMutationOperator(config.MutationOperator, random, config);
        var fitness = OperatorFactory.CreateFitnessFunction(config.FitnessFunction, instance);
        var generator = OperatorFactory.CreatePopulationGenerator(config.PopulationGenerator, random, instance);


        return RunInstance(selection, crossover, mutation, fitness, localSearch, generator, config, instance, writer, out solution);
    }

    /// <summary>
    /// Runs the genetic algorithm for a single SAT instance using explicit operator instances.
    /// </summary>
    /// <param name="selection">Selection operator.</param>
    /// <param name="crossover">Crossover operator.</param>
    /// <param name="mutation">Mutation operator.</param>
    /// <param name="fitness">Fitness function.</param>
    /// <param name="localSearch">Local search operator (optional).</param>
    /// <param name="generator">Population generator.</param>
    /// <param name="config">GA configuration.</param>
    /// <param name="instance">The SAT instance to solve.</param>
    /// <param name="writer">Output writer for results and logs.</param>
    /// <param name="solution">Outputs the found solution.</param>
    /// <returns>True if a satisfying solution is found, otherwise false.</returns>
    public static bool RunInstance(ISelectionOperator<SatSolution> selection, ICrossoverOperator<SatSolution> crossover,
        IMutationOperator<SatSolution> mutation, IFitnessFunction<SatSolution> fitness, ILocalSearch<SatSolution>? localSearch, 
        IPopulationGenerator generator, GaConfig config, SatInstance instance, OutputWriter writer, out SatSolution solution)
    {
        // Find SAT Solution
        if (config.ThreadCount <= 1)
        {
            var ga = new GeneticAlgorithm(
                selection,
                crossover,
                mutation,
                fitness,
                generator,
                writer,
                localSearch,
                config.RandomSeed);

            solution = ga.Solve(
                instance,
                config);
        }
        else
        {
            if (!SolveInParallel(selection, crossover, mutation, fitness, localSearch, generator, config, instance,
                    writer, out solution))
            {
                return false;
            }
        }

        writer.Solution = solution.ToPrintable();
        if (solution.SatisfiedClausesCount() == instance.Clauses.Count)
        {
            writer.IsSatisfied = true;
            writer.WriteLine("SATISFIABLE");
            return true;
        }

        writer.IsSatisfied = false;
        return false;
    }

    /// <summary>
    /// Runs the genetic algorithm in parallel using multiple threads.
    /// </summary>
    /// <param name="selection">Selection operator.</param>
    /// <param name="crossover">Crossover operator.</param>
    /// <param name="mutation">Mutation operator.</param>
    /// <param name="fitness">Fitness function.</param>
    /// <param name="localSearch">Local search operator (optional).</param>
    /// <param name="generator">Population generator.</param>
    /// <param name="config">GA configuration.</param>
    /// <param name="instance">The SAT instance to solve.</param>
    /// <param name="writer">Output writer for results and logs.</param>
    /// <param name="solution">Outputs the found solution.</param>
    /// <returns>True if a satisfying solution is found, otherwise false.</returns>
    private static bool SolveInParallel(ISelectionOperator<SatSolution> selection, ICrossoverOperator<SatSolution> crossover,
        IMutationOperator<SatSolution> mutation, IFitnessFunction<SatSolution> fitness, ILocalSearch<SatSolution>? localSearch, IPopulationGenerator generator,
        GaConfig config, SatInstance instance, OutputWriter writer, out SatSolution? solution)
    {
        var tasks = new List<Task<SatSolution>>();
        SatSolution threadSolution = null;
        var cts = new CancellationTokenSource();
            
        for (int i = 0; i < config.ThreadCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var localGa = new GeneticAlgorithm(selection, crossover, mutation, fitness, generator, writer, localSearch); // TODO: Support seed
                var sol = localGa.Solve(instance, config, cts.Token);
                threadSolution = sol;
                return sol;
            }, cts.Token));
        }
            
        try
        {
            Task.WaitAny(tasks.ToArray());
            solution = threadSolution;
            cts.Cancel();
        }
        catch (AggregateException ex)
        {
            /* Ignore cancellations */ 
            writer.WriteLine(ex.Message);
            solution = null;
            return false;
        }

        return true;
    }
}