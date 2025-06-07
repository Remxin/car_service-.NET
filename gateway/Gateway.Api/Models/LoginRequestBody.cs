namespace Gateway.Api.Models;

public class LoginRequestBody {
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}