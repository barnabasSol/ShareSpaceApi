using Microsoft.EntityFrameworkCore;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.Models;

namespace ShareSpaceApi.Extensions;

public static class UserInfoCheck
{
    public static async Task<bool> EmaiOrUsernameExists(
        this CreateUserDTO requesting_user,
        DbSet<User> users
    )
    {
        return await users.AnyAsync(
            _ => _.UserName == requesting_user.UserName || _.Email == requesting_user.Email
        );
    }
}
