namespace WebApi.Models;

public class EmailMessage
{
    public string MessageType { get; set; } = "verify-email";
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
