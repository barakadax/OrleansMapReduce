using Extensions;
using Extensions.Interfaces;
using Ninject.Modules;

namespace Silo;

public class NinjectBindings : NinjectModule
{
    public override void Load()
    {
        Bind<IMicrosoftTranslator>().To<MicrosoftTranslator>().InSingletonScope();
    }
}
