using GrainInterfaces;

namespace Grains;

public class NumberGrain : Grain, INumberGrain
{
    private ulong _counter = 0;

    public Task<ulong> GetCount()
    {
        return Task.FromResult(_counter);
    }

    public Task Increment()
    {
        _counter++;
        return Task.CompletedTask;
    }
}
