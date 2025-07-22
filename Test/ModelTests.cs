using SAT.GA.Models;
using Xunit;

namespace SAT.GA.Test
{
    public class ModelTests
    {
        [Fact]
        public void IsSatisfied_ReturnsTrue_WhenAnyLiteralSatisfied()
        {
            var clause = new SatClause(0, [1, -2, 3]);
            var assignment = new[] { true, false, true }; // x1=true, x2=false, x3=true

            Assert.True(clause.IsSatisfied(assignment));
        }

        [Fact]
        public void IsSatisfied_ReturnsFalse_WhenNoLiteralsSatisfied()
        {
            var clause = new SatClause(0, [-1, 2]);
            var assignment = new[] { true, false }; // x1=true, x2=false

            Assert.False(clause.IsSatisfied(assignment));
        }
    }

    public class SatInstanceTests
    {
        [Fact]
        public void IsSatisfied_ReturnsTrue_WhenAllClausesSatisfied()
        {
            var clauses = new List<SatClause>
            {
                new(0, [1, -2]),
                new(1, [2, 3])
            };
            var instance = new SatInstance(3, clauses);
            var assignment = new[] { true, true, true };

            Assert.True(instance.IsSatisfied(assignment));
        }
    }

    public class SatSolutionTests
    {
        [Fact]
        public void SatisfiedClausesCount_ReturnsCorrectCount()
        {
            var clauses = new List<SatClause>
            {
                new(0, [1, -2]), // satisfied
                new(1, [-1, 2])  // not satisfied
            };
            var instance = new SatInstance(2, clauses);
            var solution = new SatSolution(instance, [true, false]);

            Assert.Equal(1, solution.SatisfiedClausesCount());
        }
    }
}