namespace ShareSpaceApi.Data.DTOs;

public record CommentDto
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public required string UserName { get; set; }
    public required string Name { get; set; }
    public string? UserProfilePicUrl { get; set; }
    public required string Content { get; set; }
    public DateTime CommentedAt { get; set; }
}

public record CommentAddDto
{
    public Guid PostId { get; set; }
    public required string Content { get; set; }
}
