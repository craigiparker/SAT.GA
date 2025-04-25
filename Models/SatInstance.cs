namespace SAT.GA.Models;

public class SatInstance
{
    public int VariableCount { get; }
    public List<SatClause> Clauses { get; }

    public SatInstance(int variableCount, List<SatClause> clauses)
    {
        VariableCount = variableCount;
        Clauses = clauses;
    }

    public bool IsSatisfied(bool[] assignment)
    {
        return Clauses.All(c => c.IsSatisfied(assignment));
    }
}