namespace Mtf.Maui.YouTube.OAuth2.Interfaces;

public interface IYouTubeChannelInfo : IHavePackageName, IClientSecretProvider
{
    IEnumerable<string> Scopes { get; }
}
