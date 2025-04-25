// Example program
using SAT.GA;
using SAT.GA.Configuration;
using SAT.GA.Factories;
using SAT.GA.Models;
using SAT.GA.Parsers;
using System.Diagnostics;

class Program
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
        string path = "C:\\KCL\\SAT_DIMACS\\uf100-430\\uf100-01.cnf";
        // var path = "C:\\KCL\\SAT_DIMACS\\uf20-91\\uf20-0396.cnf";
        // var path = "C:\\KCL\\SAT_DIMACS\\uf20-91";
        // var path = "C:\\KCL\\SAT_DIMACS\\uf250.1065.100\\ai\\hoos\\Shortcuts\\UF250.1065.100\\uf250-01.cnf";
        //var path = "C:\\KCL\\SAT_DIMACS\\Bejing\\2bitcomp_5.cnf";

        var files = Directory.Exists(path) ? Directory.GetFiles(path) : [path];

        int solvedCount = 0;
        int total = 0;

        foreach (var filePath in files.Take(10))
        {
            Console.WriteLine($"Running file - {filePath}");
            var file = File.OpenText(filePath);
            cnfContent = file.ReadToEnd();

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            if (RunInstance(cnfContent)) solvedCount++;
            stopWatch.Stop();
            Console.WriteLine("Time taken:" + stopWatch.Elapsed);
            total++;
            Console.WriteLine($"Solved {solvedCount} of {total}");
        }

        
    }

    private static bool RunInstance(string cnfContent)
    {
        // Parse the CNF
        var parser = new DimacsParser();
        var instance = parser.Parse(cnfContent);

        Console.WriteLine($"There are {instance.VariableCount} variables and {instance.Clauses.Count} CNF clauses");

        // Configure the GA
        var config = new GaConfig
        {
            PopulationSize = 50,
            ElitismRate = 0.1,
            Generations = 10000,
            MutationRate = 0.01,
            UseLocalSearch = true,
            RandomSeed = 42,
            TabuTenure = (int)(0.1 * instance.VariableCount)
        };

        var random = config.RandomSeed.HasValue
            ? new Random(config.RandomSeed.Value)
            : new Random();

        // Create operators
        var localSearch = OperatorFactory.CreateLocalSearch("Tabu", random, config);
        var selection = OperatorFactory.CreateSelectionOperator("Tournament", random, config);
        var crossover = OperatorFactory.CreateCrossoverOperator("LocalSearch", random, localSearch, config);
        // var crossover = OperatorFactory.CreateCrossoverOperator("Uniform", random, localSearch, config);
        // var crossover = OperatorFactory.CreateCrossoverOperator("Clause", random, localSearch, config);
        var mutation = OperatorFactory.CreateMutationOperator("Guided", random);
        var fitness = OperatorFactory.CreateFitnessFunction("MaxSat", instance);

        // Create and run GA
        var ga = new GeneticAlgorithm(
            selection,
            crossover,
            mutation,
            fitness,
            localSearch,
            config.RandomSeed);

        var solution = ga.Solve(
            instance,
            config.PopulationSize,
            config.Generations,
            config.MutationRate,
            config.ElitismRate);

        // Display results
        Console.WriteLine("Best solution found:");
        Console.WriteLine($"Satisfied clauses: {solution.SatisfiedClausesCount()}/{instance.Clauses.Count}");
        // Console.WriteLine("Assignment:");
        // for (int i = 0; i < solution.Assignment.Length; i++)
        // {
        //     Console.WriteLine($"x{i + 1} = {solution.Assignment[i]}");
        // }

        if (solution.SatisfiedClausesCount() == instance.Clauses.Count)
        {
            Console.WriteLine("SATISFIABLE");
            for (int i = 0; i < solution.Assignment.Length; i++)
            {
                bool value = solution.Assignment[i];
                string sign = value ? "" : "-";
                Console.Write($"{sign}{i + 1} ");
            }
            Console.WriteLine();
            return true;
        }

        return false;
    }
}