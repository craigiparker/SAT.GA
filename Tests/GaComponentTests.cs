// using SAT.GA.Factories;
// using SAT.GA.Interfaces;
// using SAT.GA.Models;
// using SAT.GA.Operators;
// using SAT.GA.Fitness;
// using SAT.GA.LocalSearch;
// using Xunit;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using SAT.GA.Configuration;
// using System.Diagnostics;
// using SAT.GA.Parsers;
// using Xunit.Abstractions;
//
// namespace SAT.GA.Tests;
//
// public class GaComponentTests
// {
//     private readonly ITestOutputHelper _testOutputHelper;
//
//     public GaComponentTests(ITestOutputHelper testOutputHelper)
//     {
//         _testOutputHelper = testOutputHelper;
//         // _operatorFactory = new OperatorFactory(_random);
//         // _fitnessFactory = new FitnessFactory();
//         //
//         // // Create a small test instance
//         // _testInstance = new SatInstance
//         // {
//         //     Variables = 10,
//         //     Clauses = new List<Clause>
//         //     {
//         //         new Clause { Literals = new List<int> { 1, 2, -3 } },
//         //         new Clause { Literals = new List<int> { -1, 2, 4 } },
//         //         new Clause { Literals = new List<int> { 3, -4, 5 } },
//         //         new Clause { Literals = new List<int> { -2, 3, -5 } },
//         //         new Clause { Literals = new List<int> { 1, -3, 4 } }
//         //     }
//         // };
//     }
//
//
//
//     [Fact]
//     public void TestAllComponentCombinations()
//     {
//         var config = new GaConfig
//         {
//             PopulationSize = 100,
//             ElitismRate = 0.1,
//             Generations = 100000,
//             MutationRate = 0.01,
//             UseLocalSearch = false,
//             RandomSeed = 2025,
//             TabuTenure = 5
//         };
//
//         var random = config.RandomSeed.HasValue
//             ? new Random(config.RandomSeed.Value)
//             : new Random();
//
//         var path = "C:\\KCL\\SAT_DIMACS\\uf50-218\\uf50-0188.cnf";
//
//         var files = Directory.Exists(path) ? Directory.GetFiles(path) : [path];
//
//         int solvedCount = 0;
//         int total = 0;
//         List<TimeSpan> metrics = new List<TimeSpan>();
//
//
//         foreach (var filePath in files.Take(100))
//         {
//             _testOutputHelper.WriteLine($"Running file - {filePath}");
//             var file = File.OpenText(filePath);
//             var cnfContent = file.ReadToEnd();
//
//             // Parse the CNF
//             var parser = new DimacsParser();
//             var instance = parser.Parse(cnfContent);
//
//             _testOutputHelper.WriteLine($"There are {instance.VariableCount} variables and {instance.Clauses.Count} CNF clauses");
//
//             var selectionTypes = new[] { "Tournament", "Roulette", "Rank" };
//             var crossoverTypes = new[] { "Clause", "Uniform" };
//             var mutationTypes = new[] { "BitFlip", "Guided", "NBit" };
//             var fitnessTypes = new[] { "MaxSat", "Weighted", "Amplified" };
//             //var localSearchTypes = new[] {  "Tabu", "HillClimbing", "SimulatedAnnealing", "VariableNeighborhood" };
//
//             foreach (var selection in selectionTypes)
//                 foreach (var crossover in crossoverTypes)
//                     foreach (var mutation in mutationTypes)
//                         foreach (var fitness in fitnessTypes)
//                         {
//                             var stopWatch = new Stopwatch();
//                             stopWatch.Start();
//
//                             if (Program.RunInstance(
//                                     OperatorFactory.CreateSelectionOperator(selection, random, config),
//                                     OperatorFactory.CreateCrossoverOperator(crossover, random, null, config),
//                                     OperatorFactory.CreateMutationOperator(mutation, random, config),
//                                     OperatorFactory.CreateFitnessFunction(fitness, instance), null, config, instance))
//                             {
//                                 solvedCount++;
//                             }
//
//                             stopWatch.Stop();
//                             metrics.Add(stopWatch.Elapsed);
//                             _testOutputHelper.WriteLine("Time taken:" + stopWatch.Elapsed);
//                         }
//             total++;
//         }
//
//         foreach (var metric in metrics)
//         {
//             _testOutputHelper.WriteLine(metric.ToString());
//         }
//
//
//
//     }
// }