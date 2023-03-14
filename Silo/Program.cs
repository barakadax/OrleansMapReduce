using System.Diagnostics.CodeAnalysis;

namespace Silo;

[ExcludeFromCodeCoverage]
public class Program
{
    public static async Task Main()
    {
        await Silo.RunSilo();
    }
}
