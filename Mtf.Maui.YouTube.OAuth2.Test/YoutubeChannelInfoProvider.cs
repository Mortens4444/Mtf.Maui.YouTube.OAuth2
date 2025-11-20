using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using Mtf.Maui.YouTube.OAuth2.Interfaces;

namespace Mtf.Maui.YouTube.OAuth2.Test;

public class YoutubeChannelInfoProvider(IConfiguration config) : IYouTubeChannelInfo
{
    private readonly IConfiguration config = config;

    public string AppName => "Application Name";
    public string PackageName => "com.company.application";
    public string ChannelId => "YouTube Channel Id"; // You can find this on YouTube
    public string FtpHost => "ftpHost";
    public string FtpCacheFilePath => "videos.csv";
    public int DesiredCount => 10;
    public int MaxResult => 50;
    public Uri HttpsCacheFileUri => new($"https://{FtpHost}/{FtpCacheFilePath}");
    public IEnumerable<string> Scopes => [YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeForceSsl];

    // You can get ClientId and ClientSecret from https://console.cloud.google.com/ (Project / Clients)
#if ANDROID
    public string ClientId => null;
    public string? ClientSecret => null;
#endif

#if WINDOWS
    public string ClientId => config["YouTubeChannel:ClientIdWindows"] ?? String.Empty;
    public string? ClientSecret => config["YouTubeChannel:ClientSecretWindows"];
#endif

#if MACCATALYST
    public string ClientId => "";
    public string? ClientSecret => null;
#endif

#if IOS
    public string ClientId => "";
    public string? ClientSecret => null;
#endif

}
