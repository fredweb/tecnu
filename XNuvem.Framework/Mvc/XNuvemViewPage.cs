using System.Web.Mvc;
using XNuvem.Environment;

namespace XNuvem.Mvc
{
    public class XNuvemViewPage<TModel> : WebViewPage<TModel>
    {
        public IServiceContext ServiceContext { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            ServiceContext = XNuvemServices.Current;
        }

        public override void Execute()
        {
        }
    }

    public class XNuvemViewPage : ViewPage<dynamic>
    {
    }
}