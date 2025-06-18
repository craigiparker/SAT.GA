using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.Operators;

public class MutationOperators
{
    private readonly Random _random;

    public MutationOperators(Random random)
    {
        _random = random;
    }

    /// <summary>
    /// Standard bit-flip mutation with probability p for each bit
    /// </summary>
    public void StandardMutation(SatSolution solution, double probability)
    {
        for (int i = 0; i < solution.Assignment.Length; i++)
        {
            if (_random.NextDouble() < probability)
            {
                solution.Assignment[i] = !solution.Assignment[i];
            }
        }
    }

    /// <summary>
    /// Mutation that focuses on variables in unsatisfied clauses
    /// </summary>
    public void ClauseGuidedMutation(SatSolution solution, double probability)
    {
        var unsatisfiedClauses = solution.Instance.Clauses
            .Where(c => !c.IsSatisfied(solution.Assignment))
            .ToList();

        if (!unsatisfiedClauses.Any()) return;

        foreach (var clause in unsatisfiedClauses)
        {
            foreach (var literal in clause.Literals)
            {
                var variable = Math.Abs(literal) - 1;
                if (_random.NextDouble() < probability)
                {
                    solution.Assignment[variable] = !solution.Assignment[variable];
                }
            }
        }
    }

    /// <summary>
    /// Mutation that flips variables based on their frequency in unsatisfied clauses
    /// </summary>
    public void FrequencyBasedMutation(SatSolution solution, double baseProbability)
    {
        var variableFrequency = new Dictionary<int, int>();
        
        // Count frequency in unsatisfied clauses
        foreach (var clause in solution.Instance.Clauses)
        {
            if (!clause.IsSatisfied(solution.Assignment))
            {
                foreach (var literal in clause.Literals)
                {
                    var variable = Math.Abs(literal) - 1;
                    variableFrequency[variable] = variableFrequency.GetValueOrDefault(variable, 0) + 1;
                }
            }
        }

        // Apply mutation with probability proportional to frequency
        foreach (var (variable, frequency) in variableFrequency)
        {
            var probability = baseProbability * (1 + frequency);
            if (_random.NextDouble() < probability)
            {
                solution.Assignment[variable] = !solution.Assignment[variable];
            }
        }
    }

    /// <summary>
    /// Mutation that flips a random number of variables in a random window
    /// </summary>
    public void WindowMutation(SatSolution solution, int minWindowSize = 2, int maxWindowSize = 5)
    {
        var windowSize = _random.Next(minWindowSize, maxWindowSize + 1);
        var startPos = _random.Next(0, solution.Assignment.Length - windowSize + 1);
        
        for (int i = 0; i < windowSize; i++)
        {
            solution.Assignment[startPos + i] = !solution.Assignment[startPos + i];
        }
    }

    /// <summary>
    /// Mutation that flips variables based on their impact on clause satisfaction
    /// </summary>
    public void ImpactBasedMutation(SatSolution solution, double baseProbability)
    {
        var variableImpact = new Dictionary<int, int>();
        
        // Calculate impact of each variable
        for (int i = 0; i < solution.Assignment.Length; i++)
        {
            var originalValue = solution.Assignment[i];
            solution.Assignment[i] = !originalValue;
            
            var satisfiedAfter = solution.Instance.Clauses.Count(c => c.IsSatisfied(solution.Assignment));
            var satisfiedBefore = solution.Instance.Clauses.Count(c => c.IsSatisfied(solution.Assignment));
            
            variableImpact[i] = Math.Abs(satisfiedAfter - satisfiedBefore);
            solution.Assignment[i] = originalValue;
        }

        // Apply mutation with probability proportional to impact
        foreach (var (variable, impact) in variableImpact)
        {
            var probability = baseProbability * (1 + impact);
            if (_random.NextDouble() < probability)
            {
                solution.Assignment[variable] = !solution.Assignment[variable];
            }
        }
    }

    /// <summary>
    /// Mutation that combines multiple strategies with adaptive probabilities
    /// </summary>
    public void AdaptiveMutation(SatSolution solution, double baseProbability)
    {
        var strategies = new List<Action<SatSolution, double>>
        {
            StandardMutation,
            ClauseGuidedMutation,
            FrequencyBasedMutation,
            ImpactBasedMutation
        };

        // Choose strategy based on current solution state
        var unsatisfiedCount = solution.Instance.Clauses.Count(c => !c.IsSatisfied(solution.Assignment));
        var totalClauses = solution.Instance.Clauses.Count;
        var unsatisfiedRatio = (double)unsatisfiedCount / totalClauses;

        if (unsatisfiedRatio > 0.5)
        {
            // More unsatisfied clauses -> use more aggressive strategies
            ClauseGuidedMutation(solution, baseProbability * 1.5);
            FrequencyBasedMutation(solution, baseProbability * 1.2);
        }
        else
        {
            // Fewer unsatisfied clauses -> use more focused strategies
            ImpactBasedMutation(solution, baseProbability * 1.2);
            WindowMutation(solution);
        }
    }

    /// <summary>
    /// Mutation that uses a combination of local search and random moves
    /// </summary>
    public void HybridMutation(SatSolution solution, double probability, int localSearchIterations = 5)
    {
        // First apply standard mutation
        StandardMutation(solution, probability);

        // Then apply local search to improve the solution
        for (int i = 0; i < localSearchIterations; i++)
        {
            var bestMove = -1;
            var bestImprovement = 0;

            // Find best 1-flip move
            for (int j = 0; j < solution.Assignment.Length; j++)
            {
                solution.Assignment[j] = !solution.Assignment[j];
                var improvement = solution.Instance.Clauses.Count(c => c.IsSatisfied(solution.Assignment));
                solution.Assignment[j] = !solution.Assignment[j];

                if (improvement > bestImprovement)
                {
                    bestImprovement = improvement;
                    bestMove = j;
                }
            }

            // Apply best move if found
            if (bestMove != -1)
            {
                solution.Assignment[bestMove] = !solution.Assignment[bestMove];
            }
            else
            {
                break; // No improving moves found
            }
        }
    }
} 