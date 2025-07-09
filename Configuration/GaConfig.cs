namespace SAT.GA.Configuration;

public class GaConfig
{
    public int PopulationSize { get; set; } = 30;
    public int Generations { get; set; } = 20000;
    public double MutationRate { get; set; } = 0.01;
    public double CrossoverRate { get; set; } = 0.9;
    public double ElitismRate { get; set; } = 0.1;
    public int? RandomSeed { get; set; } = null;
    public int TournamentSize { get; set; } = 3;
    public int TabuTenure { get; set; } = 5;
    public double AmplificationFactor { get; set; } = 2.0;
    public bool UseLocalSearch { get; set; } = false;
    public int MutationBits { get; set; } = 1;
    public int FileCountLimit { get; set; } = 1000;
    public int ThreadCount { get; set; } = 1;
    public int RestartAfter { get; set; } = 25;

    public bool HideOutput { get; set; } = false;
    public string LocalSearchMethod { get; set; } = "None";
    public string SelectionOperator { get; set; } = "Tournament";
    public string CrossoverOperator { get; set; } = "1Point";
    public string MutationOperator { get; set; } = "Guided";
    public string FitnessFunction { get; set; } = "Weighted";
    public string PopulationGenerator { get; set; } = "Clause";
}