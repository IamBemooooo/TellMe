using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Core.Entities
{
    public class MessageReaction 
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string MessageId { get; set; } = string.Empty;
        public string Reaction { get; set; } = string.Empty;
        public string SenderPsid { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
