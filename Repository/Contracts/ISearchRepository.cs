using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;

namespace ShareSpaceApi.Repository.Contracts;

public interface ISearchRepository
{
    Task<ApiResponse<IEnumerable<UserSearchDto>>> SearchUser(string value, Guid current_user);
    Task<ApiResponse<IEnumerable<PostSearchDto>>> SearchPost(string value, Guid current_user);
}
