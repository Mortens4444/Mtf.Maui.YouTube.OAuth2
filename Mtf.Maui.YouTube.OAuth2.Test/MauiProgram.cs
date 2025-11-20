using Microsoft.Extensions.Logging;
using Mtf.Maui.YouTube.OAuth2.Services;
using Mtf.Maui.YouTube.OAuth2.Test.ViewModels;

namespace Mtf.Maui.YouTube.OAuth2.Test;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if WINDOWS
        //builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
#endif
        builder.Services.AddSingleton<YoutubeChannelInfoProvider>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<MainPage>();
        PlatformServices.RegisterPlatformServices(builder.Services);

        return builder.Build();
    }
}
