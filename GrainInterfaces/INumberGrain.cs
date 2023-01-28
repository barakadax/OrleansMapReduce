namespace GrainInterfaces; 

public interface INumberGrain : IGrainWithIntegerKey
{
    void Increase();
    Task<ulong> GetCounter();
}
