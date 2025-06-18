using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

public class GuidedLocalSearch : ILocalSearch<SatSolution>
{
    private readonly Random _random;
    private readonly double _lambda;
    private readonly Dictionary<int, int> _penalties;

    public GuidedLocalSearch(Random random, double lambda = 0.1)
    {
        _random = random;
        _lambda = lambda;
        _penalties = new Dictionary<int, int>();
    }

    public void Improve(SatSolution individual, int maxIterations)
    {
        var bestAssignment = (bool[])individual.Assignment.Clone();
        double bestFitness = individual.SatisfiedClausesCount();

        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Find most violated clauses
            var violatedClauses = individual.Instance.Clauses
                .Where(c => !c.IsSatisfied(individual.Assignment))
                .ToList();

            if (!violatedClauses.Any()) break;

            // Update penalties for variables in violated clauses
            foreach (var clause in violatedClauses)
            {
                foreach (var literal in clause.Literals)
                {
                    var variable = Math.Abs(literal);
                    _penalties[variable] = _penalties.GetValueOrDefault(variable, 0) + 1;
                }
            }

            // Try to improve solution
            var improved = false;
            for (int i = 0; i < individual.Assignment.Length; i++)
            {
                individual.Assignment[i] = !individual.Assignment[i];
                var newFitness = EvaluateSolution(individual);
                
                if (newFitness > bestFitness)
                {
                    bestFitness = newFitness;
                    bestAssignment = (bool[])individual.Assignment.Clone();
                    improved = true;
                }
                else
                {
                    individual.Assignment[i] = !individual.Assignment[i];
                }
            }

            if (!improved) break;
        }

        individual.Assignment = bestAssignment;
    }

    private double EvaluateSolution(SatSolution individual)
    {
        var satisfiedClauses = individual.SatisfiedClausesCount();
        var penalty = 0.0;

        for (int i = 0; i < individual.Assignment.Length; i++)
        {
            if (individual.Assignment[i])
            {
                penalty += _penalties.GetValueOrDefault(i + 1, 0) * _lambda;
            }
        }

        return satisfiedClauses - penalty;
    }
} 