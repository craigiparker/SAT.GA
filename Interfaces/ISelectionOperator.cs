namespace SAT.GA.Interfaces;

public interface ISelectionOperator<T> where T : class
{
    List<T> Select(List<T> population, int selectionSize);
}