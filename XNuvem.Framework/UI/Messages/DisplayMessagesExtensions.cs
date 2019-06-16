namespace XNuvem.UI.Messages
{
    public static class DisplayMessagesExtensions
    {
        public static void Error(this IDisplayMessages display, string message)
        {
            display.AddMessage(MessageType.Error, message);
        }

        public static void Information(this IDisplayMessages display, string message)
        {
            display.AddMessage(MessageType.Information, message);
        }

        public static void Success(this IDisplayMessages display, string message)
        {
            display.AddMessage(MessageType.Success, message);
        }

        public static void Warning(this IDisplayMessages display, string message)
        {
            display.AddMessage(MessageType.Warning, message);
        }
    }
}