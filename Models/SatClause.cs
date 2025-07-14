namespace SAT.GA.Models;

/// <summary>
/// Represents a single clause in a SAT problem, consisting of a list of literals.
/// </summary>
public class SatClause
{
    /// <summary>Index of the clause in the SAT instance.</summary>
    public int Index { get;  }
    /// <summary>List of integer literals in the clause.</summary>
    public List<int> Literals { get; set; }

    /// <summary>
    /// Initializes a new instance of the SatClause class.
    /// </summary>
    /// <param name="index">Index of the clause.</param>
    /// <param name="literals">List of integer literals in the clause.</param>
    public SatClause(int index, List<int> literals)
    {
        Index = index;
        Literals = literals;
    }

    /// <summary>
    /// Determines whether the clause is satisfied by the given assignment.
    /// </summary>
    /// <param name="assignment">Boolean assignment for each variable.</param>
    /// <returns>True if the clause is satisfied, otherwise false.</returns>
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