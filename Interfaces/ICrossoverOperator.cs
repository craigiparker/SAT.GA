namespace SAT.GA.Interfaces;

public interface ICrossoverOperator<T> where T : class
{
    IEnumerable<T> Crossover(T parent1, T parent2);
}