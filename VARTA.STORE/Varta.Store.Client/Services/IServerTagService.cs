using Varta.Store.Shared;

namespace Varta.Store.Client.Services;

public interface IServerTagService
{
    Task<List<ServerTag>> GetServerTags();
}
