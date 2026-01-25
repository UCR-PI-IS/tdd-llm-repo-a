using System;

namespace Domain.Entities
{
    public class CPDLCMessage
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime Timestamp { get; set; }

        public CPDLCMessage()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }
    }
}
