namespace SAT.GA.Models;

public class SatClause
{
    public int Index { get;  }
    public List<int> Literals { get; }

    public SatClause(int index, List<int> literals)
    {
        Literals = literals;
    }

    public bool IsSatisfied(bool[] assignment)
    {
        foreach (var literal in Literals)
        {
            var variable = Math.Abs(literal);
            var isNegated = literal < 0;

            if (assignment[variable - 1] != isNegated)
            {
                return true;
            }
        }
        return false;
    }
}