namespace SAT.GA.Interfaces;

public interface ILocalSearch<T> where T : class
{
    void Improve(T individual, int maxIterations);
}