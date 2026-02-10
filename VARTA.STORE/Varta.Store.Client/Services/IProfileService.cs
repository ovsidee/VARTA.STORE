using Varta.Store.Shared.DTO;

namespace Varta.Store.Client.Services;

public interface IProfileService
{
    Task<UserProfileDto?> GetProfileAsync();
}
