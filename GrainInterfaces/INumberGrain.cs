namespace GrainInterfaces;

public interface INumberGrain : IGrainWithStringKey
{
    Task Increment();
    Task<ulong> GetCount();
}
