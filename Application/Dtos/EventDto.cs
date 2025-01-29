namespace Application.Dtos
{
    public class EventDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; }
        public string? KeyWords { get; set; } 
        public List<string> Guests { get; set; } = new List<string>();
    }
}
