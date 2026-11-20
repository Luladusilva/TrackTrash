using TrackTrash.Application.Dtos;

namespace TrackTrash.Application.Ports.Services;

public interface IXMLService
{
    byte[] CreateUsersReport(IReadOnlyList<UserDto> users);
    byte[] CreateCollectRequestsReport(IReadOnlyList<CollectRequestDto> requests);
}
