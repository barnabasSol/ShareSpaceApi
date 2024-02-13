using ShareSpaceApi.Data.Context;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Data.Models;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Repository;

public class CommentRepository(ShareSpaceDbContext shareSpaceDb) : ICommentRepository
{
    private readonly ShareSpaceDbContext shareSpaceDb = shareSpaceDb;

    public async Task<ApiResponse<Guid>> AddComment(CommentAddDto comment, Guid user_id)
    {
        try
        {
            var new_comment = new Comment
            {
                Content = comment.Content,
                PostId = comment.PostId,
                UserId = user_id
            };

            await shareSpaceDb.Comments.AddAsync(new_comment);
            await shareSpaceDb.SaveChangesAsync();

            return new ApiResponse<Guid> { IsSuccess = true, Data = new_comment.Id };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ApiResponse<string>> DeleteComment(Guid comment_id)
    {
        try
        {
            var comment = await shareSpaceDb.Comments.FindAsync(comment_id);
            if (comment is null)
            {
                return new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "comment doesnt exist"
                };
            }
            shareSpaceDb.Comments.Remove(comment);
            await shareSpaceDb.SaveChangesAsync();
            return new ApiResponse<string> { IsSuccess = true };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
