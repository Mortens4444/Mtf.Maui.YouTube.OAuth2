using Google.Apis.Auth.OAuth2;
using Mtf.Maui.YouTube.OAuth2.Interfaces;

namespace Mtf.Maui.YouTube.OAuth2.Platforms.iOS.Services;

internal partial class UserCredentialsHandler : IUserCredentialsHandler
{
    public Task<UserCredential?> CreateUserCredentialsAsync(IYouTubeChannelInfo youTubeChannelInfo, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCredentials()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
