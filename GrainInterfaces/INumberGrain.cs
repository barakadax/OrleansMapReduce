namespace GrainInterfaces;

public interface INumberGrain : IGrainWithStringKey
{
    Task Increase();
    Task<ulong> GetCounter();
}
