using Microsoft.EntityFrameworkCore;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.Models;

namespace ShareSpaceApi.Extensions;

public static class ConvertToPostDto
{
    public static List<PostDto> ToPostDto(
        this List<Post> posts,
        DbSet<LikedPost> likedPosts,
        Guid current_user
    )
    {
        return [.. posts
            .Select(
                s =>
                    new PostDto
                    {
                        TextContent = s.Content,
                        PostUserProfilePicUrl = s.User?.ProfilePicUrl,
                        PostedName = s.User!.Name,
                        PostedUsername = s.User!.UserName,
                        PostedUserId = s.UserId,
                        PostId = s.Id,
                        PostPictureUrls =
                            s.PostImages!.Select(i => i.ImageUrl) ?? [],
                        LikesCount = s.Likes,
                        ViewsCount = s.Views,
                        CommentsCount = s.Comments?.Count ?? 0,
                        PostedDateTime = s.CreatedAt,
                        IsLikedByCurrentUser = likedPosts.Any(
                            a => a.PostId == s.Id && a.UserId == current_user
                        ),
                    }
            ).OrderByDescending(o => o.PostedDateTime)];
    }
}
