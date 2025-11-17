using Mtf.Maui.YouTube.OAuth2.Interfaces;
using Mtf.Maui.YouTube.OAuth2.Platforms.iOS.Services;

namespace Mtf.Maui.YouTube.OAuth2.Services;

public static partial class PlatformServices
{
    public static partial void RegisterPlatformServices(IServiceCollection services)
    {
        services.AddSingleton<IUserCredentialsHandler, UserCredentialsHandler>();
    }
}
