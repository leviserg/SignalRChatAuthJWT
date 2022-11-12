using System.ComponentModel.DataAnnotations;

namespace SignalRChatAuthJWT.Models
{
    public class AuthModel
    {
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 5)]
        public string Login { get; set; }
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Password { get; set; }
    }
}
