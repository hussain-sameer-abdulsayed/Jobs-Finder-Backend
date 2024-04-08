namespace MB_Project.Models.DTOS.UserDto
{
    public class ViewUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; } = false; // Email check to active
        public string? ProfileImage { get; set; }
        public string Bio { get; set; }
        public List<Post>? Posts { get; set; } // one to many
        public List<Review>? Reviews { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
