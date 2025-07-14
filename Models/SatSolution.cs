using System.Text;

namespace SAT.GA.Models;

/// <summary>
/// Represents a candidate solution for a SAT problem, including variable assignments and fitness.
/// </summary>
public class SatSolution
{
    /// <summary>Boolean assignment for each variable in the SAT instance.</summary>
    public bool[] Assignment { get; set; }
    /// <summary>The SAT instance this solution belongs to.</summary>
    public SatInstance Instance { get; }
    /// <summary>Fitness value of the solution (if evaluated).</summary>
    public double? Fitness { get; set; }
    /// <summary>Number of generations taken to reach this solution.</summary>
    public int Generations { get; set; }
    /// <summary>Number of restarts performed to reach this solution.</summary>
    public int Restarts { get; set; }

    /// <summary>
    /// Initializes a new instance of the SatSolution class.
    /// </summary>
    /// <param name="instance">The SAT instance this solution belongs to.</param>
    /// <param name="assignment">Boolean assignment for each variable.</param>
    public SatSolution(SatInstance instance, bool[] assignment)
    {
        Instance = instance;
        Assignment = assignment;
    }

    /// <summary>
    /// Counts the number of satisfied clauses for this solution.
    /// </summary>
    /// <returns>The number of satisfied clauses.</returns>
    public int SatisfiedClausesCount()
    {
        return Instance.Clauses.Count(c => c.IsSatisfied(Assignment));
    }

    /// <summary>
    /// Gets whether this solution satisfies all clauses.
    /// </summary>
    public bool IsSolution
    {
        get
        {
            return Instance.Clauses.All(c => c.IsSatisfied(Assignment));
        }
    }

    /// <summary>
    /// Returns a list of clauses that are not satisfied by this solution.
    /// </summary>
    /// <returns>List of unsatisfied SatClause objects.</returns>
    public List<SatClause> UnSatisifedClauses()
    {
        return Instance.Clauses.Where(c => !c.IsSatisfied(Assignment)).ToList();
    }

    /// <summary>
    /// Returns a string representation of the solution, including fitness and assignment.
    /// </summary>
    /// <returns>String representation of the solution.</returns>
    public override string ToString()
    {
        var output = "";
        foreach (var gene in Assignment)
        {
            output += (gene ? 1 : 0);
        }

        return $"{Fitness} [{output}]";
    }

    /// <summary>
    /// Returns a printable string of the variable assignments in DIMACS format.
    /// </summary>
    /// <returns>Printable string of assignments.</returns>
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

    /// <summary>
    /// Determines whether this solution is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if equal, otherwise false.</returns>
    public override bool Equals(object? obj)
    {
        return obj != null && Equals((SatSolution) obj);
    }

    /// <summary>
    /// Determines whether this solution is equal to another SatSolution.
    /// </summary>
    /// <param name="other">The other SatSolution to compare with.</param>
    /// <returns>True if equal, otherwise false.</returns>
    protected bool Equals(SatSolution other)
    {
        return Assignment.SequenceEqual(other.Assignment);
    }

    /// <summary>
    /// Returns a hash code for this solution.
    /// </summary>
    /// <returns>Hash code for the solution.</returns>
    public override int GetHashCode()
    {
        return Assignment.GetHashCode();
    }
}