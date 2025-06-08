namespace Gateway.Api.Models;

public class AddServiceCommentRequestBody {
    public int OrderId { get; set; }
    public string Content { get; set; } = null!;
}
