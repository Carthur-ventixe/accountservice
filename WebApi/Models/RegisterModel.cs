using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class RegisterModel
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
