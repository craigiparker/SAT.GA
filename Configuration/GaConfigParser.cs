namespace SAT.GA.Configuration;

public class GaConfigParser
{
    public static GaConfig Parse(string[] args)
    {
        var config = new GaConfig();

        // Skip first argument as it's reserved for file name
        for (int i = 1; i < args.Length; i++)
        {
            string flag = args[i].ToLower();

            // Check for help flag first
            if (flag is "-h" or "--help" or "?")
            {
                PrintHelp();
                Environment.Exit(0);
            }

            try
            {
                switch (flag)
                {
                    case "-p":
                    case "--population":
                        config.PopulationSize = int.Parse(args[++i]);
                        break;

                    case "-g":
                    case "--generations":
                        config.Generations = int.Parse(args[++i]);
                        break;

                    case "-m":
                    case "--mutation-rate":
                        config.MutationRate = double.Parse(args[++i]);
                        break;

                    case "-c":
                    case "--crossover-rate":
                        config.CrossoverRate = double.Parse(args[++i]);
                        break;

                    case "-e":
                    case "--elitism-rate":
                        config.ElitismRate = double.Parse(args[++i]);
                        break;

                    case "-s":
                    case "--seed":
                        config.RandomSeed = int.Parse(args[++i]);
                        break;

                    case "-t":
                    case "--tournament":
                        config.TournamentSize = int.Parse(args[++i]);
                        break;

                    case "--tabu":
                        config.TabuTenure = int.Parse(args[++i]);
                        break;

                    case "-a":
                    case "--amplification":
                        config.AmplificationFactor = double.Parse(args[++i]);
                        break;

                    case "-l":
                    case "--local-search":
                        config.UseLocalSearch = bool.Parse(args[++i]);
                        break;

                    case "--mutation-bits":
                        config.MutationBits = int.Parse(args[++i]);
                        break;

                    case "-fc":
                    case "--file-count":
                        config.FileCountLimit = int.Parse(args[++i]);
                        break;

                    case "-tc":
                    case "--thread-count":
                        config.ThreadCount = int.Parse(args[++i]);
                        break;

                    case "--local-search-method":
                        config.LocalSearchMethod = args[++i];
                        break;

                    case "--selection":
                        config.SelectionOperator = args[++i];
                        break;

                    case "--crossover":
                        config.CrossoverOperator = args[++i];
                        break;

                    case "--mutation":
                        config.MutationOperator = args[++i];
                        break;

                    case "--fitness":
                        config.FitnessFunction = args[++i];
                        break;

                    case "--generator":
                        config.PopulationGenerator = args[++i];
                        break;

                    case "--hide":
                        config.HideOutput = bool.Parse(args[++i]);
                        break;

                    default:
                        Console.WriteLine($"Unknown flag: {flag}");
                        Console.WriteLine("Use -h or --help for available options");
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Missing value for flag: {flag}");
                Environment.Exit(0);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Invalid value for flag: {flag}");
                Environment.Exit(0);
            }
        }

        return config;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("SAT.GA Configuration Options: Usage SAT.GA <fileName> [-flag] [value] pairs");
        Console.WriteLine("  -p, --population <int>      Population size (default: 100)");
        Console.WriteLine("  -g, --generations <int>     Number of generations (default: 500)");
        Console.WriteLine("  -m, --mutation-rate <double> Mutation probability (default: 0.01)");
        Console.WriteLine("  -c, --crossover-rate <double> Crossover probability (default: 0.9)");
        Console.WriteLine("  -e, --elitism-rate <double> Elitism rate (default: 0.1)");
        Console.WriteLine("  -s, --seed <int>            Random seed (default: null)");
        Console.WriteLine("  -t, --tournament <int>      Tournament size (default: 3)");
        Console.WriteLine("      --tabu <int>           Tabu tenure (default: 5)");
        Console.WriteLine("  -a, --amplification <double> Amplification factor (default: 2.0)");
        Console.WriteLine("  -l, --local-search <bool>   Enable local search (default: true)");
        Console.WriteLine("  -fc, --file-count <int>   Limit how many files are processed when folder option used (default: 1000)");
        Console.WriteLine("  -tc, --thread-count <int>   Number of threads used to process in parallel (default: 1)");
        Console.WriteLine("      --mutation-bits <int>   Number of bits to mutate (default: 1)");
        Console.WriteLine("      --local-search-method <string> Local search method (default: Tabu)");
        Console.WriteLine("      --selection <string>           Selection operator (default: Tournament)");
        Console.WriteLine("      --crossover <string>          Crossover operator (default: Uniform)");
        Console.WriteLine("      --mutation <string>           Mutation operator (default: Guided)");
        Console.WriteLine("      --fitness <string>            Fitness function (default: Amplified)");
        Console.WriteLine("      --generator <string>            Population Generator (default: Clause)");
        Console.WriteLine("  -h, --help                  Show this help message");
    }
}