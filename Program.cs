// Example program
using SAT.GA;
using SAT.GA.Configuration;
using SAT.GA.Factories;
using SAT.GA.Models;
using SAT.GA.Parsers;

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

        var file = File.OpenText("C:\\KCL\\SAT_DIMACS\\uf100-430\\uf100-01.cnf");
        cnfContent = file.ReadToEnd();

        // Parse the CNF
        var parser = new DimacsParser();
        var instance = parser.Parse(cnfContent);

        // Configure the GA
        var config = new GaConfig
        {
            PopulationSize = 50,
            Generations = 100,
            MutationRate = 0.02,
            UseLocalSearch = true,
            RandomSeed = 42
        };

        var random = config.RandomSeed.HasValue
            ? new Random(config.RandomSeed.Value)
            : new Random();

        // Create operators
        var localSearch = OperatorFactory.CreateLocalSearch("Tabu", random, config);
        var selection = OperatorFactory.CreateSelectionOperator("Tournament", random, config);
        var crossover = OperatorFactory.CreateCrossoverOperator("LocalSearch", random, localSearch, config);
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
        Console.WriteLine("Assignment:");
        for (int i = 0; i < solution.Assignment.Length; i++)
        {
            Console.WriteLine($"x{i + 1} = {solution.Assignment[i]}");
        }
    }
}