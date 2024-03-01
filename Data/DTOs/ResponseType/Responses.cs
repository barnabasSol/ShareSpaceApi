namespace ShareSpaceApi.Data.DTOs.ResponseType;

public record AuthResponse
{
    public bool IsSuccess { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public record ApiResponse<X>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public X? Data { get; set; }
}
