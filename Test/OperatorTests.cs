using SAT.GA.Models;
using SAT.GA.Operators.Selection;
using SAT.GA.Operators.Crossover;
using SAT.GA.Operators.Mutation;
using Xunit;

namespace SAT.GA.Test
{
    public class SelectionOperatorTests
    {
        private readonly SatInstance _instance;
        private readonly List<SatSolution> _population;

        public SelectionOperatorTests()
        {
            _instance = new SatInstance(3, new List<SatClause>());
            _population = new List<SatSolution>
            {
                new(_instance, [true, false, true]) { Fitness = 0.9 },
                new(_instance, [false, true, false]) { Fitness = 0.7 },
                new(_instance, [true, true, true]) { Fitness = 0.5 }
            };
        }

        [Fact]
        public void TournamentSelection_SelectsBestInTournament()
        {
            var selection = new TournamentSelection(new Random(42), tournamentSize: 2);
            var selected = selection.Select(_population, 2);

            Assert.Equal(2, selected.Count);
            Assert.All(selected, s => Assert.Equal(0.9, s.Fitness));
        }

        [Fact]
        public void UniformCrossover_ProducesValidChild()
        {
            var parent1 = new SatSolution(_instance, [true, true, true]);
            var parent2 = new SatSolution(_instance, [false, false, false]);
            var crossover = new UniformCrossover(new Random(42));

            var children = crossover.Crossover(parent1, parent2);

            foreach (var child in children)
            {
                Assert.Equal(3, child.Assignment.Length);
            }
        }

        [Fact]
        public void BitFlipMutation_ChangesSomeBits()
        {
            var solution = new SatSolution(_instance, [true, true, true]);
            var original = solution.Assignment.Clone();
            var mutation = new BitFlipMutation(new Random(42));

            mutation.Mutate(solution, mutationRate: 0.5);

            Assert.NotEqual(original, solution.Assignment);
        }
    }
}