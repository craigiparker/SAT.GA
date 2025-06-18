// Example program
using SAT.GA;
using SAT.GA.Configuration;
using SAT.GA.Factories;
using SAT.GA.Models;
using System.Diagnostics;
using SAT.GA.Interfaces;
using System.Threading.Tasks;
using SAT.GA.Utils;

public class Program
{
    static void Main(string[] args)
    {
        // Example CNF formula (x1 ∨ ¬x2) ∧ (x2 ∨ x3)
        string cnfContent = @"
            c Example CNF
            p cnf 3 2
            1 -2 0
            2 3 0
        ";

        //string path = "C:\\KCL\\SAT_DIMACS\\uf100-430";
        // string path = "C:\\KCL\\SAT_DIMACS\\uf100-430";
        // var path = "C:\\KCL\\SAT_DIMACS\\uf20-91\\uf20-0396.cnf";
        // var path = "C:\\KCL\\SAT_DIMACS\\uf20-91\\uf20-010.cnf";
        var path = "C:\\KCL\\SAT_DIMACS\\uf20-91";
        // var path = "C:\\KCL\\SAT_DIMACS\\uf50-218";
        // var path = "C:\\KCL\\SAT_DIMACS\\uf50-218\\uf50-0188.cnf";
        //var path = "C:\\KCL\\SAT_DIMACS\\uf250.1065.100\\ai\\hoos\\Shortcuts\\UF250.1065.100\\uf250-01.cnf";
        // var path = "C:\\KCL\\SAT_DIMACS\\uf250.1065.100\\ai\\hoos\\Shortcuts\\UF250.1065.100";
        //var path = "C:\\KCL\\SAT_DIMACS\\Bejing\\2bitcomp_5.cnf";
        //var path = "C:\\KCL\\SAT_DIMACS\\Bejing\\4blocks.cnf";

        var files = Directory.Exists(path) ? Directory.GetFiles(path) : [path];

        int solvedCount = 0;
        int total = 0;
        List<double> metrics = new List<double>();

        var writer = new OutputWriter();

        var fileLimit = 30;
        foreach (var filePath in files.Take(fileLimit))
        {
            total++;
            writer.FilePath = filePath;
            writer.WriteLine($"Running file - {filePath}");
            var file = File.OpenText(filePath);
            cnfContent = file.ReadToEnd();

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            writer.StopWatch = stopWatch;
            if (RunInstance(cnfContent, writer)) solvedCount++;
            
            stopWatch.Stop();
            
            writer.WriteLine("Time taken: " + stopWatch.Elapsed.TotalMilliseconds + "ms");
            metrics.Add(stopWatch.Elapsed.TotalMilliseconds);

            if (total != fileLimit)
            {
                writer.ResetInstance();
            }

            //Console.WriteLine($"Solved {solvedCount} of {total}");
        }

        // foreach (var metric in metrics)
        // {
        //     Console.WriteLine(metric);
        // }
        writer.WriteCompletion($"Solved {solvedCount} of {total}. Press any key to exit.");
        Console.ReadKey();

    }

    private static bool RunInstance(string cnfContent, OutputWriter writer)
    {
        // Parse the CNF
        var parser = new DimacsParser();
        var instance = parser.Parse(cnfContent);

        writer.VariableCount = instance.VariableCount;
        writer.ClauseCount = instance.Clauses.Count;
        writer.WriteLine($"There are {instance.VariableCount} variables and {instance.Clauses.Count} CNF clauses");

        // Configure the GA
        var config = new GaConfig
        {
            PopulationSize = 50,
            ElitismRate = 0.1,
            // Generations = 100000,
            //Generations = 100000,
            Generations = 1000,
            // Generations = 1, // test popn
            MutationRate = 0.01,
            UseLocalSearch = false,
            RandomSeed = 42,
            TabuTenure = (int)(0.05 * instance.VariableCount)
        };

        var random = config.RandomSeed.HasValue
            ? new Random(config.RandomSeed.Value)
            : new Random();

        // Create operators
        var localSearch = OperatorFactory.CreateLocalSearch("Tabu", random, config);
        var selection = OperatorFactory.CreateSelectionOperator("Tournament", random, config);
        var crossover = OperatorFactory.CreateCrossoverOperator("Uniform", random, localSearch, config);
        // var crossover = OperatorFactory.CreateCrossoverOperator("Uniform", random, localSearch, config);
        // var crossover = OperatorFactory.CreateCrossoverOperator("Clause", random, localSearch, config);
        var mutation = OperatorFactory.CreateMutationOperator("Guided", random, config);
        // var mutation = OperatorFactory.CreateMutationOperator("BitFlip", random, config);
        var fitness = OperatorFactory.CreateFitnessFunction("Amplified", instance);
        // var fitness = OperatorFactory.CreateFitnessFunction("Weighted", instance);

        return RunInstance(selection, crossover, mutation, fitness, localSearch, config, instance, writer, out var solution);
    }

    public static bool RunInstance(ISelectionOperator<SatSolution> selection, ICrossoverOperator<SatSolution> crossover,
        IMutationOperator<SatSolution> mutation,
        IFitnessFunction<SatSolution> fitness, ILocalSearch<SatSolution>? localSearch, GaConfig config,
        SatInstance instance, OutputWriter writer, out SatSolution solution)
    {
        // Create and run GA
        var ga = new GeneticAlgorithm(
            selection,
            crossover,
            mutation,
            fitness,
            writer,
            localSearch,
            config.RandomSeed);

        // dummy
        solution = new SatSolution(instance, []);

        // solution = ga.Solve(
        //     instance,
        //     config.PopulationSize,
        //     config.Generations,
        //     config.MutationRate,
        //     config.ElitismRate);

        var tasks = new List<Task<SatSolution>>();
        object lockObj = new object();
        SatSolution threadSolution = null;
        var cts = new CancellationTokenSource();
        var threadCount = 1;//Environment.ProcessorCount;

        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var localGa = new GeneticAlgorithm(selection, crossover, mutation, fitness, writer, localSearch, config.RandomSeed + 10000 * i);
                var sol = localGa.Solve(instance, config.PopulationSize, config.Generations, config.MutationRate, config.ElitismRate);
                threadSolution = sol;
                return sol;
            }, cts.Token));
        }

        try
        {
            Task.WaitAny(tasks.ToArray());
            solution = threadSolution;
        }
        catch (AggregateException ex)
        {
            /* Ignore cancellations */ 
            //Console.WriteLine(ex);
            writer.WriteLine(ex.Message);
            return false;
        }


        // Display results
        //Console.WriteLine("Best solution found:");
        //Console.WriteLine($"Satisfied clauses: {solution?.SatisfiedClausesCount()}/{instance.Clauses.Count}");

        writer.Solution = solution.ToPrintable();
        if (solution.SatisfiedClausesCount() == instance.Clauses.Count)
        {
            //Console.WriteLine("SATISFIABLE");
            // for (int i = 0; i < solution.Assignment.Length; i++)
            // {
            //     bool value = solution.Assignment[i];
            //     string sign = value ? "" : "-";
            //     Console.Write($"{sign}{i + 1} ");
            // }
            writer.IsSatisfied = true;
            //writer.Solution = solution.ToPrintable();
            writer.WriteLine("SATISFIABLE");
            //Console.WriteLine(solution.ToPrintable());
            //Console.WriteLine();
            return true;
        }

        writer.IsSatisfied = false;
        return false;
    }
}