using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Mtf.Maui.YouTube.OAuth2.Interfaces;
using Mtf.Maui.YouTube.OAuth2.Services;

namespace Mtf.Maui.YouTube.OAuth2.Platforms.Android.Services;

internal partial class UserCredentialsHandler : IUserCredentialsHandler
{
    private UserCredential? credentials;
    private GoogleAuthorizationCodeFlow? googleAuthorizationCodeFlow;

    public async Task<UserCredential?> CreateUserCredentialsAsync(IYouTubeChannelInfo youTubeChannelInfo, CancellationToken cancellationToken = default)
    {
        var redirectUri = $"{youTubeChannelInfo.PackageName}:/oauth2redirect";
        googleAuthorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets { ClientId = youTubeChannelInfo.ClientId },
            Scopes = youTubeChannelInfo.Scopes,
            DataStore = new SecureStorageDataStore()
        });

        var authUrl = googleAuthorizationCodeFlow.CreateAuthorizationCodeRequest(redirectUri).Build().AbsoluteUri;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var authResult = await WebAuthenticator.Default.AuthenticateAsync(new Uri(authUrl), new Uri(redirectUri)).ConfigureAwait(false);

            if (authResult?.Properties.TryGetValue("code", out var code) != true)
            {
                throw new InvalidOperationException("Authorization code not found in response.");
            }

            var token = await googleAuthorizationCodeFlow.ExchangeCodeForTokenAsync("user", code, redirectUri, cancellationToken).ConfigureAwait(false);
            credentials = new UserCredential(googleAuthorizationCodeFlow, "user", token);
            return credentials;
        }
        catch (OperationCanceledException)
        {
            ErrorDisplayer.ShowError("Authentication canceled.");
            return null;
        }
        catch (Exception ex)
        {
            ErrorDisplayer.ShowError(ex);
            return null;
        }
    }

    public async Task DeleteCredentials()
    {
        if (credentials != null)
        {
            await credentials.RevokeTokenAsync(CancellationToken.None).ConfigureAwait(false);
        }
        if (googleAuthorizationCodeFlow != null)
        {
            await googleAuthorizationCodeFlow.DataStore.DeleteAsync<TokenResponse>("user").ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        googleAuthorizationCodeFlow?.Dispose();
    }
}
