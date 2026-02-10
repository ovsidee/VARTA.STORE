using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Services.Interfaces;

public interface IAdminService
{
    Task<List<UserProfileDto>> GetAllUsersAsync(CancellationToken ct = default);
    Task<UserProfileDto?> GetUserByIdAsync(int id, CancellationToken ct = default);
}
