using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;

namespace ShareSpaceApi.Repository.Contracts;

public interface IPostRepository
{
    Task<ApiResponse<string>> CreatePost(CreatePostDto post);
    Task<ApiResponse<string>> EditPost(EditPostDto editPost);
    Task<ApiResponse<IEnumerable<PostDto>>> GetPosts(Guid current_user);
    Task<ApiResponse<IEnumerable<PostDto>>> GetTrendingPosts(Guid current_user);
    Task<ApiResponse<PostDetailDto>> GetPost(Guid post_id, Guid current_user);
    Task<ApiResponse<string>> DeletePost(Guid post_id);
    Task<ApiResponse<string>> UpdateLike(Guid post_id, Guid user_id);
}
