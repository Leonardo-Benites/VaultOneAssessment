using Domain.Enums;

namespace Domain.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public EventType Type { get; set; }
        public string? KeyWords { get; set; } 
        public List<string> Guests { get; set; } = new List<string>();

        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}