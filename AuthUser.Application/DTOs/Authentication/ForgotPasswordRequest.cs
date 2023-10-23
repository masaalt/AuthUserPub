using System.ComponentModel.DataAnnotations;

namespace AuthUser.Application.DTOs.Authentication
{
    public class ForgotPasswordRequest
    {
        [Required]
        public string UserName { get; set; }
    }
}