using Extensions.Interfaces;
using Translators;
using Translators.Interfaces;

namespace Silo;

public static class DIBinding
{
    public readonly static List<Binding> Bindings = new()
    {
        new Binding() { Interface = typeof(IMicrosoftTranslator),  Class = typeof(MicrosoftTranslator) }
    };
}
