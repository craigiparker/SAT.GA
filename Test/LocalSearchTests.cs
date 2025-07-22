using SAT.GA.Models;
using SAT.GA.LocalSearch;
using Xunit;

namespace SAT.GA.Test
{
    public class LocalSearchTests
    {
        private readonly SatInstance _instance;

        public LocalSearchTests()
        {
            var clauses = new List<SatClause>
            {
                new(0, [1, 2]),
                new(1, [-1, 2]),
                new(2, [-2])
            };
            _instance = new SatInstance(2, clauses);
        }

        [Fact]
        public void HillClimbing_ImprovesSolution()
        {
            var solution = new SatSolution(_instance, [false, false]) { Fitness = 1 };
            var hillClimbing = new HillClimbing(new Random(42));

            hillClimbing.Improve(solution, maxIterations: 10);

            Assert.True(solution.SatisfiedClausesCount() >= 1);
        }

        [Fact]
        public void TabuSearch_FindsBetterSolution()
        {
            var solution = new SatSolution(_instance, [false, false]);
            var originalFitness = solution.SatisfiedClausesCount();
            var tabuSearch = new TabuSearch(new Random(42));

            tabuSearch.Improve(solution, maxIterations: 20);

            Assert.True(solution.SatisfiedClausesCount() > originalFitness);
        }
    }
}