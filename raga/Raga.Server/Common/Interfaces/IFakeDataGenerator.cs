namespace Raga.Server.Common.Interfaces;

public interface IFakeDataGenerator<T>
{
    T Generate();
}