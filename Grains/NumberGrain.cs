using GrainInterfaces;

namespace Grains;

public class NumberGrain : Grain, INumberGrain
{
    private ulong _counter = 0;

    public Task<ulong> GetCounter()
    {
        return Task.FromResult(_counter);
    }

    public void Increase()
    {
        _counter++;
    }
}
