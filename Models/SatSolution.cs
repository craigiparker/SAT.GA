using System.Text;

namespace SAT.GA.Models;

public class SatSolution
{
    public bool[] Assignment { get; set; }
    public SatInstance Instance { get; }
    public double? Fitness { get; set; }

    public int Generations { get; set; }
    public int Restarts { get; set; }

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

    public string ToPrintable()
    {
        StringBuilder output = new StringBuilder();
        for (int i = 0; i < Assignment.Length; i++)
        {
            bool value = Assignment[i];
            string sign = value ? "" : "-";
            output.Append($"{sign}{i + 1} ");
        }

        return output.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj != null && Equals((SatSolution) obj);
    }

    protected bool Equals(SatSolution other)
    {
        return Assignment.SequenceEqual(other.Assignment);
    }

    public override int GetHashCode()
    {
        return Assignment.GetHashCode();
    }
}