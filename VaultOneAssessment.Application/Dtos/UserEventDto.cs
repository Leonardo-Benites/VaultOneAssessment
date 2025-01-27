namespace Application.Dtos
{
    public class UserEventDto
    {
        public int UserId { get; set; }
        public UserDto? User { get; set; }

        public int EventId { get; set; }
        public EventDto? Event { get; set; }

        public DateTime SubscribedDate { get; set; }
    }
}
