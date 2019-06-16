namespace XNuvem.UI.Messages
{
    public class MessageEntry
    {
        public MessageEntry(MessageType type, string message)
        {
            Type = type;
            Message = message;
        }

        public MessageType Type { get; set; }
        public string Message { get; set; }
    }
}