# Mtf.Maui.YouTube.OAuth2 documentation

Cross-platform OAuth2 authentication for .NET MAUI (Android & Windows), using Google OAuth2 + YouTube API, with secure, unified platform services.

## Features

* Cross-platform OAuth2 login (Android + Windows).
* Google OAuth2 Authorization Code flow.
* YouTube API scopes (`Youtube`, `YoutubeForceSsl`).
* Platform-specific credential handling:

  * Android: redirects to `<package>:/oauth2redirect`.
  * Windows: built-in local HTTP listener.
* Secure token storage via MAUI `SecureStorage`.
* Automatic token refresh & logout (with revoke).
* Dependency-injected service registration (per platform).
* Clean partial-class architecture for MAUI.

---

## Installation

Add the project reference:

```xml
<ProjectReference Include="..\Mtf.Maui.YouTube.OAuth2\Mtf.Maui.YouTube.OAuth2.csproj" />
```

Register the service inside your **MauiProgram.cs**:

```csharp
using Mtf.Maui.YouTube.OAuth2.Services;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    PlatformServices.RegisterPlatformServices(builder.Services);

    return builder.Build();
}
```

---

## Architecture Overview

### Platform-independent interface

```csharp
public interface IUserCredentialsHandler : IDisposable
{
    Task<UserCredential?> CreateUserCredentialsAsync(
        IHavePackageName packageNameProvider,
        IClientSecretProvider clientSecretProvider,
        CancellationToken cancellationToken = default);
}
```

### Platform-specific implementations

Each platform has its own folder:

```
Platforms/
 +¦¦ Android/Services/UserCredentialsHandler.cs
 L¦¦ Windows/Services/UserCredentialsHandler.cs
```

Registered via:

```csharp
public static partial class PlatformServices
{
    public static partial void RegisterPlatformServices(IServiceCollection services);
}
```

Platform implementations:

```csharp
// Android
public static partial void RegisterPlatformServices(IServiceCollection services)
{
    services.AddSingleton<IUserCredentialsHandler, UserCredentialsHandler>();
}
```

```csharp
// Windows
public static partial void RegisterPlatformServices(IServiceCollection services)
{
    services.AddSingleton<IUserCredentialsHandler, UserCredentialsHandler>();
}
```

---

## Secure Token Storage

Tokens are stored using a custom `SecureStorageDataStore` that implements Google’s `IDataStore`:

```csharp
public class SecureStorageDataStore : IDataStore
{
    public Task StoreAsync<T>(string key, T value) =>
        SecureStorage.SetAsync(key, JsonSerializer.Serialize(value));

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await SecureStorage.GetAsync(key);
        return String.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
    }

    public Task DeleteAsync<T>(string key)
    {
        SecureStorage.Remove(key);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        SecureStorage.Remove("Google.Apis.Auth.OAuth2.user");
        return Task.CompletedTask;
    }
}
```

---

## Android OAuth2 Flow

### Redirect URI

`<PACKAGE>:/oauth2redirect`

### Workflow

1. Build auth URL.
2. Launch external browser via `WebAuthenticator`.
3. Receive authorization code.
4. Exchange code for access token.
5. Store token securely.

```csharp
var authResult = await WebAuthenticator.Default.AuthenticateAsync(
    new Uri(authUrl),
    new Uri(redirectUri));
```

---

## Windows OAuth2 Flow

### Redirect URI

`http://127.0.0.1:{RANDOM_PORT}/`

### Workflow

1. Start a local `HttpListener`.
2. Open browser with auth URL.
3. Capture Google OAuth redirect.
4. Extract code & exchange token.
5. Store securely.

```csharp
var context = await listener.GetContextAsync();
var code = context.Request.QueryString["code"];
```

---

## Error Handling

A platform-safe display helper:

```csharp
public static class ErrorDisplayer
{
    public static void ShowError(string message)
    {
        var page = Application.Current?.Windows?[0]?.Page;
        if (page != null)
            MainThread.BeginInvokeOnMainThread(() =>
                page.DisplayAlert("?? Error", $"Something went wrong. Details: {message}", "OK"));
    }
}
```

---

## Usage Example

```csharp
var handler = Services.GetRequiredService<IUserCredentialsHandler>();

var credentials = await handler.CreateUserCredentialsAsync(
    new MyPackageNameProvider(),
    new MyClientSecretProvider(),
    cancellationToken);

if (credentials != null)
{
    var service = new YouTubeService(new BaseClientService.Initializer
    {
        HttpClientInitializer = credentials
    });
}
```

---

## Token Revocation (Logout)

```csharp
public void Dispose()
{
    _ = DeleteCredentials();
    googleAuthorizationCodeFlow?.Dispose();
}
```

`Dispose()` revokes the token + clears secure storage.

---

## Requirements

* .NET 8/9 MAUI
* Google API Client Library
* Registered Google OAuth2 credentials

  * Android uses `ClientId`
  * Windows requires `ClientSecret` too

---

## License

MIT — free for commercial and personal use.