using System;

namespace CashlessRegisterSystemCore.Helpers
{
    public enum MessageType
    {
        Service, Info, Warning, FatalError
    }

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public MessageType Type { get; set; }
    }

    public class NotifyList
    {
        public delegate void MessageEventHandler(MessageEventArgs message);

        public EventHandler dataChange;
        public MessageEventHandler messageNotice;
    }
}
