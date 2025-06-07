namespace Gateway.Api.Models;

public class AddRoleRequestBody {
    public string Token { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string RoleId { get; set; } = null!;
}