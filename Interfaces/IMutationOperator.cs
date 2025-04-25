namespace SAT.GA.Interfaces;

public interface IMutationOperator<T> where T : class
{
    void Mutate(T individual, double mutationRate);
}