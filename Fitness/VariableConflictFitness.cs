using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Fitness;

public class VariableConflictFitness : IFitnessFunction<SatSolution>
{
    public double Calculate(SatSolution individual)
    {
        var satisfied = individual.SatisfiedClausesCount();
        var conflicts = CalculateVariableConflicts(individual);
        
        // Reward satisfied clauses but penalize variable conflicts
        return satisfied - (0.1 * conflicts);
    }
    
    private int CalculateVariableConflicts(SatSolution individual)
    {
        int conflicts = 0;
        var variableAssignments = new Dictionary<int, bool>();
        
        foreach (var clause in individual.Instance.Clauses)
        {
            foreach (var literal in clause.Literals)
            {
                var variable = Math.Abs(literal);
                var expectedValue = literal > 0;
                
                if (variableAssignments.TryGetValue(variable, out var assignment))
                {
                    if (assignment != expectedValue)
                        conflicts++;
                }
                else
                {
                    variableAssignments[variable] = expectedValue;
                }
            }
        }
        
        return conflicts;
    }
} 