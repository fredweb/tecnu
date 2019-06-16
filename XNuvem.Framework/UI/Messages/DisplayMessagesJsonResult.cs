using System.Collections.Generic;
using System.Web.Mvc;

namespace XNuvem.UI.Messages
{
    public class DisplayMessagesJsonResult : JsonResult
    {
        public DisplayMessagesJsonResult(IEnumerable<MessageEntry> messages)
        {
            Data = new {IsError = true, Messages = messages};
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }
    }
}