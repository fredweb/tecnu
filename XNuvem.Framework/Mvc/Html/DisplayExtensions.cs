using System.Collections.Generic;
using System.Web.Mvc;
using XNuvem.UI.Messages;

namespace XNuvem.Mvc.Html
{
    public static class DisplayExtensions
    {
        public static MvcHtmlString DisplayErrors(this HtmlHelper html, IEnumerable<MessageEntry> messages)
        {
            //if (messages.Any()) {
            //    var errors = messages.Where(m => m.Type == MessageType.Error).ToList();
            //}
            return MvcHtmlString.Empty;
        }
    }
}