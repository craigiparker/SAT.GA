namespace SAT.GA.Configuration;

/// <summary>
/// Represents the configuration parameters for the genetic algorithm.
/// </summary>
public class GaConfig
{
    /// <summary>Population size for the genetic algorithm.</summary>
    public int PopulationSize { get; set; } = 30;
    /// <summary>Number of generations to run the genetic algorithm.</summary>
    public int Generations { get; set; } = 20000;
    /// <summary>Probability of mutation for each gene.</summary>
    public double MutationRate { get; set; } = 0.01;
    /// <summary>Probability of crossover between parents.</summary>
    public double CrossoverRate { get; set; } = 0.9;
    /// <summary>Proportion of elite individuals preserved each generation.</summary>
    public double ElitismRate { get; set; } = 0.1;
    /// <summary>Optional random seed for reproducibility.</summary>
    public int? RandomSeed { get; set; } = null;
    /// <summary>Size of tournament for tournament selection.</summary>
    public int TournamentSize { get; set; } = 3;
    /// <summary>Tabu tenure for Tabu Search local search.</summary>
    public int TabuTenure { get; set; } = 5;
    /// <summary>Amplification factor for probability amplification fitness.</summary>
    public double AmplificationFactor { get; set; } = 2.0;
    /// <summary>Whether to use local search in the genetic algorithm.</summary>
    public bool UseLocalSearch { get; set; } = false;
    /// <summary>Number of bits to mutate in N-bit mutation.</summary>
    public int MutationBits { get; set; } = 1;
    /// <summary>Maximum number of files to process when a folder is specified.</summary>
    public int FileCountLimit { get; set; } = 1000;
    /// <summary>Number of threads to use for parallel processing.</summary>
    public int ThreadCount { get; set; } = 1;
    /// <summary>Number of generations without improvement before restart.</summary>
    public int RestartAfter { get; set; } = 25;
    /// <summary>Whether to hide output in the console.</summary>
    public bool HideOutput { get; set; } = false;
    /// <summary>Local search method to use (e.g., None, Tabu, HillClimbing, SimulatedAnnealing).</summary>
    public string LocalSearchMethod { get; set; } = "None";
    /// <summary>Selection operator type (e.g., Tournament, Rank, Roulette).</summary>
    public string SelectionOperator { get; set; } = "Tournament";
    /// <summary>Crossover operator type (e.g., 1Point, 2Point, Uniform, Clause).</summary>
    public string CrossoverOperator { get; set; } = "1Point";
    /// <summary>Mutation operator type (e.g., Guided, BitFlip, NBit).</summary>
    public string MutationOperator { get; set; } = "Guided";
    /// <summary>Fitness function type (e.g., Weighted, MaxSat, Amplified).</summary>
    public string FitnessFunction { get; set; } = "Weighted";
    /// <summary>Population generator type (e.g., Clause, Random, Diversity).</summary>
    public string PopulationGenerator { get; set; } = "Clause";
}