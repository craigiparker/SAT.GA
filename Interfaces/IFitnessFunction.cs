namespace SAT.GA.Interfaces;

public interface IFitnessFunction<T> where T : class
{
    double Calculate(T individual);
}