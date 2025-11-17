using Google.Apis.Auth.OAuth2;

namespace Mtf.Maui.YouTube.OAuth2.Interfaces;

public interface IUserCredentialsHandler : IDisposable
{
    Task<UserCredential?> CreateUserCredentialsAsync(IYouTubeChannelInfo youTubeChannelInfo, CancellationToken cancellationToken = default);

    Task DeleteCredentials();
}
