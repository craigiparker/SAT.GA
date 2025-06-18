// SAT.GA/Configuration/GaConfig.cs
namespace SAT.GA.Configuration;

public class GaConfig
{
    public int PopulationSize { get; set; } = 100;
    public int Generations { get; set; } = 500;
    public double MutationRate { get; set; } = 0.01;
    public double CrossoverRate { get; set; } = 0.9;
    public double ElitismRate { get; set; } = 0.1;
    public int? RandomSeed { get; set; } = null;
    public int TournamentSize { get; set; } = 3;
    public int TabuTenure { get; set; } = 5;
    public double AmplificationFactor { get; set; } = 2.0;
    public bool UseLocalSearch { get; set; } = true;
    public int MutationBits { get; set; } = 1;
}