namespace Gateway.Api.Models;

public class RemoveRoleRequestBody {
    public string UserId { get; set; } = null!;
    public string RoleId { get; set; } = null!;
}