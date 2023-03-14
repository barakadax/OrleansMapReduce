using Extensions;
using Extensions.Interfaces;

namespace Silo;

public readonly record struct Binding
{
    public required Type Interface { get; init; }
    public required Type Class { get; init; }
}

public static class DIBinding
{
    public readonly static List<Binding> Bindings = new()
    {
        new Binding() { Interface = typeof(IMicrosoftTranslator),  Class = typeof(MicrosoftTranslator) }
    };
}
