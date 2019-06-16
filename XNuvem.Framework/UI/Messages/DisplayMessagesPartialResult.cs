using System.Collections.Generic;
using System.Web.Mvc;

namespace XNuvem.UI.Messages
{
    public class DisplayMessagesPartialResult : PartialViewResult
    {
        public DisplayMessagesPartialResult(ControllerBase controller, IEnumerable<MessageEntry> messages)
        {
            ViewData = controller.ViewData;
            TempData = controller.TempData;
            ViewData.Model = messages;
            ViewName = "DisplayMessagesPartial";
        }
    }
}