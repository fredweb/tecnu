namespace XNuvem.Environment
{
    public interface IServiceContext
    {
        TService Resolve<TService>();

        bool TryResolve<TService>(out TService service);
    }
}