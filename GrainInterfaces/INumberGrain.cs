namespace GrainInterfaces; 

public interface INumberGrain : IGrainWithStringKey
{
    void Increase();
    Task<ulong> GetCounter();
}
