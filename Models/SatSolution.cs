namespace SAT.GA.Models;

public class SatSolution
{
    public bool[] Assignment { get; set; }
    public SatInstance Instance { get; }
    public double? Fitness { get; set; }

    public SatSolution(SatInstance instance, bool[] assignment)
    {
        Instance = instance;
        Assignment = assignment;
    }

    public int SatisfiedClausesCount()
    {
        return Instance.Clauses.Count(c => c.IsSatisfied(Assignment));
    }
}