using Domain.Enums;

namespace Application.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public UserProfile Profile { get; set; }
    }
}
