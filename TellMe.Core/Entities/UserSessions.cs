using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Core.Entities
{
    public class UserSessions : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiredAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}
