namespace ShareSpaceApi.Data.DTOs;

public record CreateUserDTO
{
    public required string Name { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public record UpdatePasswordDto(string OldPassword, string NewPassword);

public record UserLoginDTO(string UserName, string Password);

public record ExtraUserInfoDto
{
    public string? ProfilePicUrl { get; set; }
    public string? Bio { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public DateTime JoinedDate { get; set; }
    public IEnumerable<InterestsDto>? Interests { get; set; }
}

public record FollowerUserDto
{
    public Guid UserId { get; set; }
    public string? ProfilePicUrl { get; set; }
    public required string Name { get; set; }
    public required string UserName { get; set; }
    public bool IsBeingFollowed { get; set; }
}

public record SuggestedUserDto
{
    public required string Name { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

public record UpdateUserProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public FileDto? ProfilePic { get; set; }
    public string? OldProfilePicUrl { get; set; }
}

public record ProfileDto
{
    public ExtraUserInfoDto? ExtraUserInfoDto { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<PostDto>? Posts { get; set; }
    public bool IsBeingFollowed { get; set; }
}
