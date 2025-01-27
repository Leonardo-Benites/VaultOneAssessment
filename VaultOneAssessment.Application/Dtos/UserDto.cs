namespace Application.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Profile { get; set; }

        public List<UserEventDto> UserEvents { get; set; } = new List<UserEventDto>();
    }
}
