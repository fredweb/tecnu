using XNuvem.Environment.Extensions;

namespace XNuvem.Environment
{
    public static class PreApplicationStarter
    {
        public static void Start()
        {
            ExtensionLoader.Load();
        }
    }
}