using System.ComponentModel.DataAnnotations;

namespace FirstDatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

         [Required]
        public string Password { get; set; }
    }
}