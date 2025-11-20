using Mtf.Maui.YouTube.OAuth2.Interfaces;
using Mtf.Maui.YouTube.OAuth2.Services;
using System.Windows.Input;

namespace Mtf.Maui.YouTube.OAuth2.Test.ViewModels;

public class MainViewModel(
    IUserCredentialsHandler userCredentialsHandler,
    YoutubeChannelInfoProvider youtubeChannelInfoProvider)
{
    private readonly IUserCredentialsHandler userCredentialsHandler = userCredentialsHandler;
    private readonly YoutubeChannelInfoProvider youtubeChannelInfoProvider = youtubeChannelInfoProvider;

    public ICommand GetVideosCommand => new Command(v => _ = Task.Run(async () => await GetNewestNotLikedVideosAsync()));

    public async Task GetNewestNotLikedVideosAsync(CancellationToken cancellationToken = default)
    {
        var credentials = await userCredentialsHandler.CreateUserCredentialsAsync(youtubeChannelInfoProvider, cancellationToken).ConfigureAwait(false);
        if (credentials == null)
        {
            return;
        }

        var youTubeServiceWrapper = new YouTubeServiceWrapper(credentials, youtubeChannelInfoProvider.AppName, youtubeChannelInfoProvider.ChannelId);

        var videos = await youTubeServiceWrapper.GetNewestNotLikedVideosAsync(youtubeChannelInfoProvider.DesiredCount, youtubeChannelInfoProvider.MaxResult, youtubeChannelInfoProvider.HttpsCacheFileUri, cancellationToken: cancellationToken).ConfigureAwait(false);
        //var videos = await youTubeServiceWrapper.GetAllVideosAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

    }
}
