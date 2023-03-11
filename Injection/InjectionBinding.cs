using Extensions;
using Extensions.Interfaces;
using Ninject.Modules;

namespace Injection;

public class InjectionBinding : NinjectModule
{
    public override void Load()
    {
        Bind<IMicrosoftTranslator>().To<MicrosoftTranslator>().InSingletonScope();
    }
}
