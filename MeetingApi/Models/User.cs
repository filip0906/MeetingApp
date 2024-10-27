using Microsoft.AspNetCore.Identity;

namespace MeetingApi.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

