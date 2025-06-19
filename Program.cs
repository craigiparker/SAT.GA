using System.Diagnostics;
using SAT.GA.Configuration;
using SAT.GA.Factories;
using SAT.GA.Interfaces;
using SAT.GA.Models;
using SAT.GA.Utils;

namespace SAT.GA;

public class Program
{
    private static void Main(string[] args)
    {
        var config = GenerateConfigFromArgs(args, out var files);

        int solvedCount = 0;
        int total = 0;
        var metrics = new List<double>();
        var solutions = new List<SatSolution>();

        var writer = new OutputWriter();
        var parser = new DimacsParser();

        foreach (var filePath in files.Take(config.FileCountLimit))
        {
            ProcessCnfFile(writer, filePath, parser, config, metrics, solutions, ref total, ref solvedCount);
        }

        PrintSummary(metrics, solutions, writer, solvedCount, total);
        Console.ReadKey();

    }

    private static void ProcessCnfFile(OutputWriter writer, string filePath, DimacsParser parser, GaConfig config,
        List<double> metrics, List<SatSolution> solutions, ref int total, ref int solvedCount)
    {
        total++;
        writer.FilePath = filePath;
        writer.WriteLine($"Running file - {filePath}");
        var file = File.OpenText(filePath);

        // Parse the CNF
        var instance = parser.Parse(file.ReadToEnd());

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        writer.StopWatch = stopWatch;
        if (RunInstance(instance, writer, config, out var solution)) solvedCount++;
            
        stopWatch.Stop();
            
        writer.WriteLine("Time taken: " + stopWatch.Elapsed.TotalMilliseconds + "ms");
        metrics.Add(stopWatch.Elapsed.TotalMilliseconds);
        solutions.Add(solution);

        if (total != config.FileCountLimit)
        {
            writer.ResetInstance();
        }
    }

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
        var crossover = OperatorFactory.CreateCrossoverOperator(config.CrossoverOperator, random, localSearch, config);
        var mutation = OperatorFactory.CreateMutationOperator(config.MutationOperator, random, config);
        var fitness = OperatorFactory.CreateFitnessFunction(config.FitnessFunction, instance);


        return RunInstance(selection, crossover, mutation, fitness, localSearch, config, instance, writer, out solution);
    }

    public static bool RunInstance(ISelectionOperator<SatSolution> selection, ICrossoverOperator<SatSolution> crossover,
        IMutationOperator<SatSolution> mutation,
        IFitnessFunction<SatSolution> fitness, ILocalSearch<SatSolution>? localSearch, GaConfig config,
        SatInstance instance, OutputWriter writer, out SatSolution solution)
    {
        // Create and run GA
        if (config.ThreadCount <= 1)
        {
            var ga = new GeneticAlgorithm(
                selection,
                crossover,
                mutation,
                fitness,
                writer,
                localSearch,
                config.RandomSeed);

            solution = ga.Solve(
                instance,
                config.PopulationSize,
                config.Generations,
                config.MutationRate,
                config.ElitismRate);
        }
        else
        {
            var tasks = new List<Task<SatSolution>>();
            SatSolution threadSolution = null;
            var cts = new CancellationTokenSource();
            
            for (int i = 0; i < config.ThreadCount; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var localGa = new GeneticAlgorithm(selection, crossover, mutation, fitness, writer, localSearch); // TODO: Support seed
                    var sol = localGa.Solve(instance, config.PopulationSize, config.Generations, config.MutationRate, config.ElitismRate, cts.Token);
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
}