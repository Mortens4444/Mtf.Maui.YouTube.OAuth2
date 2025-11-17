using Mtf.Maui.YouTube.OAuth2.Interfaces;
using Mtf.Maui.YouTube.OAuth2.Platforms.Tizen.Services;

namespace Mtf.Maui.YouTube.OAuth2.Services;

public static partial void RegisterPlatformServices(IServiceCollection services)
{
    services.AddSingleton<IUserCredentialsHandler, UserCredentialsHandler>();
}
