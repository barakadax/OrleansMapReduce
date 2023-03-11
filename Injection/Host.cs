using Ninject;

namespace Injection;

public static class Host
{
    public static StandardKernel BindInjection()
    {
        var kernel = new StandardKernel();
        kernel.Load(new InjectionBinding());
        return kernel;
    }
}
