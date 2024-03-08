namespace ShareSpaceApi.Data.DTOs;

public record PostBase
{
    public Guid PostId { get; set; }
    public Guid PostedUserId { get; set; }
    public string PostedUsername { get; set; } = string.Empty;
}

public record PostDto : PostBase
{
    public string? PostUserProfilePicUrl { get; set; }
    public required string PostedName { get; set; }
    public string? TextContent { get; set; }
    public IEnumerable<string>? PostPictureUrls { get; set; }
    public int LikesCount { get; set; }
    public int ViewsCount { get; set; }
    public int CommentsCount { get; set; }
    public DateTime PostedDateTime { get; set; }
    public DateTime LikedTimeStamp { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
}

public record PostDetailDto : PostDto
{
    public List<CommentDto>? Comments { get; set; }
}

public record CreatePostDto
{
    public string? TextContent { get; set; }
    public IEnumerable<FileDto>? PostFiles { get; set; }
    public Guid PostedUserId { get; set; }
}

public record FileDto
{
    public byte[]? ImageBytes { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public long Size { get; set; }
}

public record LikedPostDto
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}

public record EditPostDto(Guid PostId, string TextContent);
