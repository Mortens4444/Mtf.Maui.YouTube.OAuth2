using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Mtf.Maui.YouTube.OAuth2.Interfaces;
using Mtf.Maui.YouTube.OAuth2.Services;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Mtf.Maui.YouTube.OAuth2.Platforms.Windows.Services;

internal partial class UserCredentialsHandler : IUserCredentialsHandler
{
    private UserCredential? credentials;
    private GoogleAuthorizationCodeFlow? googleAuthorizationCodeFlow;

    public async Task<UserCredential?> CreateUserCredentialsAsync(IYouTubeChannelInfo youTubeChannelInfo, CancellationToken cancellationToken = default)
    {
        HttpListener? listener = null;
        try
        {
            var port = GetFreeTcpPort();
            var redirectUri = $"http://127.0.0.1:{port}/";

            googleAuthorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = youTubeChannelInfo.ClientId,
#if WINDOWS
                    ClientSecret = youTubeChannelInfo.ClientSecret
#endif
                },
                Scopes = youTubeChannelInfo.Scopes,
                DataStore = new SecureStorageDataStore()
            });

            var authUrl = googleAuthorizationCodeFlow.CreateAuthorizationCodeRequest(redirectUri).Build().AbsoluteUri;

            listener = new HttpListener();
            listener.Prefixes.Add(redirectUri);
            listener.Start();

            var psi = new ProcessStartInfo(authUrl) { UseShellExecute = true };
            Process.Start(psi);

            using (cancellationToken.Register(() => listener?.Stop()))
            {
                var context = await listener.GetContextAsync().ConfigureAwait(false);

                var qs = context.Request.QueryString;
                var code = qs["code"];
                var error = qs["error"];

                var responseString = "<html><body><h3>You can close this window now.</h3></body></html>";
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                context.Response.OutputStream.Close();

                if (!String.IsNullOrEmpty(error))
                {
                    ErrorDisplayer.ShowError($"OAuth error: {error}");
                    return null;
                }

                if (String.IsNullOrEmpty(code))
                {
                    ErrorDisplayer.ShowError("Authorization code not found in the response.");
                    return null;
                }

                var token = await googleAuthorizationCodeFlow.ExchangeCodeForTokenAsync("user", code, redirectUri, cancellationToken).ConfigureAwait(false);
                credentials = new UserCredential(googleAuthorizationCodeFlow, "user", token);
                return credentials;
            }
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
        finally
        {
            try { listener?.Stop(); listener?.Close(); } catch { }
        }
    }

    private static int GetFreeTcpPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        try
        {
            listener.Start();
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
        finally
        {
            listener.Stop();
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

