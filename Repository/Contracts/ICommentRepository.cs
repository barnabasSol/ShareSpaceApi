using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;

namespace ShareSpaceApi.Repository.Contracts;

public interface ICommentRepository
{
    Task<ApiResponse<string>> DeleteComment(Guid comment_id);
    Task<ApiResponse<Guid>> AddComment(CommentAddDto comment, Guid user_id);
}
