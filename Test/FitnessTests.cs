using SAT.GA.Models;
using SAT.GA.Fitness;
using Xunit;

namespace SAT.GA.Test
{
    public class FitnessFunctionTests
    {
        private readonly SatInstance _instance;

        public FitnessFunctionTests()
        {
            var clauses = new List<SatClause>
            {
                new(0, [1, -2]), // weight 1.0
                new(1, [-1, 2])  // weight 2.0
            };
            _instance = new SatInstance(2, clauses);
        }

        [Fact]
        public void MaxSatFitness_ReturnsCorrectCount()
        {
            var solution = new SatSolution(_instance, [true, false]);
            var fitness = new MaxSatFitness();

            Assert.Equal(1, fitness.Calculate(solution));
        }

        [Fact]
        public void WeightedMaxSatFitness_CalculatesWeightedSum()
        {
            var solution = new SatSolution(_instance, [true, false]);
            var fitness = new WeightedMaxSatFitness(_instance);

            Assert.Equal(1.0, fitness.Calculate(solution));
        }

        [Fact]
        public void ProbabilityAmplification_AmplifiesDifferences()
        {
            var solution1 = new SatSolution(_instance, [true, false]); // 1/2 clauses
            var solution2 = new SatSolution(_instance, [true, true]);  // 2/2 clauses
            var fitness = new ProbabilityAmplificationFitness(2.0);

            var fit1 = fitness.Calculate(solution1);
            var fit2 = fitness.Calculate(solution2);

            Assert.True(fit2 > fit1);
            Assert.Equal(0.25, fit1);
            Assert.Equal(1.0, fit2);
        }
    }
}