namespace GrainInterfaces; 
public interface IWordGrain : IGrainWithStringKey
{
    Task<ulong> WordCalculate(string word);
}
