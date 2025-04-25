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

    public List<SatClause> UnSatisifedClauses()
    {
        return Instance.Clauses.Where(c => !c.IsSatisfied(Assignment)).ToList();
    }

    public override string ToString()
    {
        var output = "";
        foreach (var gene in Assignment)
        {
            output += (gene ? 1 : 0);
        }

        return $"{Fitness} [{output}]";
    }
}