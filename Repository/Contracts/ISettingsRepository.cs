using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;

namespace ShareSpaceApi.Repository.Contracts;

public interface ISettingsRepository
{
    Task<ApiResponse<string>> UpdatePassword(UpdatePasswordDto updatePasswordDto, Guid user_id);
    Task<ApiResponse<AuthResponse>> UpdateProfile(UpdateUserProfileDto update_dto, Guid user_id);
}
