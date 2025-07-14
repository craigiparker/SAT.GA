namespace SAT.GA.Models;

/// <summary>
/// Represents a SAT problem instance, including variables and clauses.
/// </summary>
public class SatInstance
{
    /// <summary>Number of variables in the SAT instance.</summary>
    public int VariableCount { get; }
    /// <summary>List of clauses in the SAT instance.</summary>
    public List<SatClause> Clauses { get; set; }

    /// <summary>
    /// Initializes a new instance of the SatInstance class.
    /// </summary>
    /// <param name="variableCount">Number of variables in the SAT instance.</param>
    /// <param name="clauses">List of clauses in the SAT instance.</param>
    public SatInstance(int variableCount, List<SatClause> clauses)
    {
        VariableCount = variableCount;
        Clauses = clauses;
    }

    /// <summary>
    /// Determines whether the given assignment satisfies all clauses.
    /// </summary>
    /// <param name="assignment">Boolean assignment for each variable.</param>
    /// <returns>True if all clauses are satisfied, otherwise false.</returns>
    public bool IsSatisfied(bool[] assignment)
    {
        return Clauses.All(c => c.IsSatisfied(assignment));
    }

    /// <summary>
    /// Counts the number of satisfied clauses for a given assignment.
    /// </summary>
    /// <param name="assignment">Boolean assignment for each variable.</param>
    /// <returns>The number of satisfied clauses.</returns>
    public int SatisfiedCount(bool[] assignment)
    {
        return Clauses.Count(c => c.IsSatisfied(assignment));
    }
}