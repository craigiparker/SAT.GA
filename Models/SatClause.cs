namespace SAT.GA.Models;

public class SatClause
{
    public List<int> Literals { get; }

    public SatClause(List<int> literals)
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