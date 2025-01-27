namespace Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Profile { get; set; }

        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}