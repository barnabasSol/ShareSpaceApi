using Microsoft.EntityFrameworkCore;
using ShareSpaceApi.Data.Context;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Repository;

public class SearchRepository(ShareSpaceDbContext shareSpaceDb) : ISearchRepository
{
    private readonly ShareSpaceDbContext shareSpaceDb = shareSpaceDb;

    public async Task<ApiResponse<IEnumerable<PostSearchDto>>> SearchPost(
        string query,
        Guid current_user
    )
    {
        try
        {
            var posts = await shareSpaceDb.Posts
                .Where(
                    w =>
                        w.User!.UserName.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                        || w.Content!.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                        || w.User.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                )
                .Include(i => i.User)
                .Include(i => i.PostImages)
                .Include(i => i.Comments)
                .ToListAsync();

            if (posts.Count == 0)
            {
                return new ApiResponse<IEnumerable<PostSearchDto>>
                {
                    IsSuccess = true,
                    Message = "No Result Found",
                    Data = []
                };
            }
            return new ApiResponse<IEnumerable<PostSearchDto>>
            {
                IsSuccess = true,
                Data = posts.Select(
                    s =>
                        new PostSearchDto
                        {
                            TextContent = s.Content,
                            PostUserProfilePicUrl = s.User?.ProfilePicUrl,
                            PostedName = s.User!.Name,
                            PostedUsername = s.User!.UserName,
                            PostedUserId = s.UserId,
                            PostId = s.Id,
                            PostPictureUrls = s.PostImages?.Select(i => i.ImageUrl),
                            LikesCount = s.Likes,
                            ViewsCount = s.Views,
                            CommentsCount = s.Comments?.Count ?? 0,
                            PostedDateTime = s.CreatedAt,
                            IsLikedByCurrentUser = shareSpaceDb.LikedPosts.Any(
                                a => a.PostId == s.Id && a.UserId == current_user
                            )
                        }
                )
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ApiResponse<IEnumerable<UserSearchDto>>> SearchUser(
        string query_string,
        Guid current_user
    )
    {
        try
        {
            var users = await shareSpaceDb.Users
                .Where(
                    w =>
                        w.UserName.Contains(query_string, StringComparison.CurrentCultureIgnoreCase)
                        || w.Name.Contains(query_string, StringComparison.CurrentCultureIgnoreCase)
                        || w.Bio!.Contains(query_string, StringComparison.CurrentCultureIgnoreCase)
                )
                .ToListAsync();
            if (users.Count == 0)
            {
                return new ApiResponse<IEnumerable<UserSearchDto>>
                {
                    IsSuccess = true,
                    Message = "User Doesn't Exist",
                    Data = []
                };
            }
            return new ApiResponse<IEnumerable<UserSearchDto>>
            {
                IsSuccess = true,
                Data = users.Select(
                    s =>
                        new UserSearchDto
                        {
                            UserId = s.UserId,
                            ProfilePic = s.ProfilePicUrl,
                            UserName = s.UserName,
                            Name = s.Name,
                            IsBeingFollowed = shareSpaceDb.Followers.Any(
                                a => a.FollowedId == s.UserId && a.FollowerId == current_user
                            )
                        }
                )
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
