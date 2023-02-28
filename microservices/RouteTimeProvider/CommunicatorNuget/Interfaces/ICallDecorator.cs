namespace CommunicatorNuget.Interfaces;

public interface ICallDecorator<T>
{
    T AuthorizeAndForward();
}