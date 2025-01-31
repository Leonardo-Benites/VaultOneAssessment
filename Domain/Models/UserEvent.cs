using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserEvent
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public DateTime SubscribedDate { get; set; }

        public User? User { get; set; }
        public Event? Event { get; set; }
    }
}
