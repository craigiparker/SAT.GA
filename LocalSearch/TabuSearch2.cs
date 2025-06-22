using SAT.GA.Interfaces;
using SAT.GA.Models;

namespace SAT.GA.LocalSearch;

public class TabuSearch2 : ILocalSearch<SatSolution>
{
    private readonly Random _random;
    private readonly int _tabuTenure;
    private readonly int _maxIterations;
    private readonly Dictionary<int, int> _clauseViolationHistory;
    private readonly int _maxViolations;
    private readonly double _diversificationStrength;

    public void Improve(SatSolution individual, int maxIterations)
    {
        int variableCount = individual.Assignment.Length;
        int clauseCount = individual.Instance.Clauses.Count;

        int[] tabuList = new int[variableCount];
        int[] flipFrequency = new int[variableCount];
        int tabuTenure = variableCount / 10 + 5;

        double?[] clauseWeights = new double?[clauseCount];
        double penaltyWeight = 0.01;
        double smoothingFactor = 0.9;

        bool[] bestAssignment = (bool[])individual.Assignment.Clone();
        double bestScore = Evaluate(individual, clauseWeights);
        double currentScore = bestScore;

        Random rand = new Random();

        for (int iter = 0; iter < maxIterations; iter++)
        {
            int bestVar = -1;
            double bestDelta = double.MinValue;

            for (int var = 0; var < variableCount; var++)
            {
                Flip(individual.Assignment, var);
                double newScore = Evaluate(individual, clauseWeights);
                double delta = newScore - currentScore;

                bool isTabu = tabuList[var] > iter;
                bool satisfiesAspiration = newScore > bestScore;

                double diversificationPenalty = penaltyWeight * flipFrequency[var];
                double effectiveDelta = delta - diversificationPenalty;

                if ((!isTabu || satisfiesAspiration) && effectiveDelta >= bestDelta)
                {
                    bestDelta = effectiveDelta;
                    bestVar = var;
                }

                Flip(individual.Assignment, var); // undo
            }

            if (bestVar == -1) break;

            Flip(individual.Assignment, bestVar);
            tabuList[bestVar] = iter + tabuTenure;
            flipFrequency[bestVar]++;
            currentScore = Evaluate(individual, clauseWeights);

            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                Array.Copy(individual.Assignment, bestAssignment, variableCount);
            }

            // ?? Dynamic clause weight update (CWLS)
            UpdateClauseWeights(individual, clauseWeights, smoothingFactor);
        }

        individual.Assignment = bestAssignment;
        individual.Fitness = bestScore;
    }


    private void Flip(bool[] assignment, int index)
    {
        assignment[index] = !assignment[index];
    }

    private double Evaluate(SatSolution solution, double?[] clauseWeights)
    {
        double score = 0;
        var clauses = solution.Instance.Clauses;

        for (int i = 0; i < clauses.Count; i++)
        {
            foreach (int literal in clauses[i].Literals)
            {
                int varIndex = Math.Abs(literal) - 1;
                bool value = solution.Assignment[varIndex];
                if ((literal > 0 && value) || (literal < 0 && !value))
                {
                    score += (clauseWeights[i] ?? 1);
                    break;
                }
            }
        }

        return score;
    }

    private void UpdateClauseWeights(SatSolution solution, double?[] clauseWeights, double smoothingFactor)
    {
        var clauses = solution.Instance.Clauses;

        for (int i = 0; i < clauses.Count; i++)
        {
            bool satisfied = false;

            foreach (int literal in clauses[i].Literals)
            {
                int varIndex = Math.Abs(literal) - 1;
                bool value = solution.Assignment[varIndex];
                if ((literal > 0 && value) || (literal < 0 && !value))
                {
                    satisfied = true;
                    break;
                }
            }

            if (!clauseWeights[i].HasValue) clauseWeights[i] = 1;

            if (!satisfied)
            {
                clauseWeights[i] += 1.0;  // Penalize unsatisfied clause
            }
            else
            {
                clauseWeights[i] *= smoothingFactor; // Gradual decay of overshot weights
            }
        }
    }



}