#define YOUTUBE_API

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Mtf.Maui.YouTube.OAuth2.Interfaces;
using static Google.Apis.YouTube.v3.VideosResource.RateRequest;
using Video = Mtf.Maui.YouTube.OAuth2.Models.Video;

namespace Mtf.Maui.YouTube.OAuth2.Services;

public partial class YouTubeServiceWrapper : IDisposable, IYouTubeServiceWrapper
{
    private readonly string channelId;
    private readonly YouTubeService youTubeService;
    private bool disposed;

    private IEnumerable<string>? likedVideoIds;

    public YouTubeServiceWrapper(UserCredential credential, string appName, string channelId)
    {
        ArgumentNullException.ThrowIfNull(credential);
        ArgumentNullException.ThrowIfNull(channelId);

        this.channelId = channelId;
        youTubeService = new YouTubeService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = appName
        });
    }

    public async Task<IEnumerable<string>>? GetLikedVideoIdsAsync(int maxResult, CancellationToken cancellationToken = default)
    {
        var result = new HashSet<string>();
        var pageToken = String.Empty;

        try
        {
            VideoListResponse? response;
            do
            {
                var request = youTubeService.Videos.List("id");
                request.MaxResults = maxResult;
                request.MyRating = VideosResource.ListRequest.MyRatingEnum.Like;
                if (!String.IsNullOrEmpty(pageToken))
                {
                    request.PageToken = pageToken;
                }

                response = await request.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                if (response?.Items == null || response.Items.Count == 0)
                {
                    break;
                }

                foreach (var item in response.Items)
                {
                    var id = item?.Id;
                    if (!String.IsNullOrEmpty(id))
                    {
                        result.Add(id);
                    }
                }

                pageToken = response.NextPageToken;
            }
            while (!String.IsNullOrEmpty(pageToken) && !cancellationToken.IsCancellationRequested);
        }
        catch (Exception ex)
        {
            ErrorDisplayer.ShowError(ex);
        }

        return result.AsEnumerable();
    }

    public Task<IEnumerable<Video>> GetAllVideosAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        return GetVideoListAsyncWithYoutubeApi(Int32.MaxValue, Int32.MaxValue, searchTerm, cancellationToken);
    }

    private async Task<IEnumerable<Video>> GetNewestVideosAsync(int desiredCount, int maxResult, Uri cacheUri, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return await CachedYouTubeService.GetVideoListAsync(desiredCount, cacheUri, searchTerm).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ErrorDisplayer.ShowError(ex);
            return await GetVideoListAsyncWithYoutubeApi(desiredCount, maxResult, searchTerm, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<IEnumerable<Video>> GetVideoListAsyncWithYoutubeApi(int desiredCount, int maxResult, string? searchTerm, CancellationToken cancellationToken)
    {
        var results = new List<Video>();

        var pageToken = String.Empty;
        while (results.Count < desiredCount)
        {
            var request = youTubeService.Search.List("snippet");
            request.Order = SearchResource.ListRequest.OrderEnum.Date;
            request.ChannelId = channelId;
            request.MaxResults = maxResult;
            request.Type = "video";
            if (!String.IsNullOrEmpty(pageToken))
            {
                request.PageToken = pageToken;
            }

            var response = await request.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            if (response?.Items == null || response.Items.Count == 0)
            {
                break;
            }

            foreach (var item in response.Items)
            {
                var videoId = item?.Id?.VideoId;
                if (!String.IsNullOrEmpty(videoId))
                {
                    if (!likedVideoIds?.Contains(videoId) ?? true)
                    {
                        var title = item?.Snippet?.Title ?? String.Empty;
                        var description = item?.Snippet?.Description ?? String.Empty;

                        if (!String.IsNullOrEmpty(searchTerm))
                        {
                            if (!title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) &&
                                !description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                        }

                        var video = new Video
                        {
                            Title = title,
                            Description = description,
                            VideoId = item?.Id?.VideoId ?? String.Empty,
                            VideoType = item?.Id?.Kind ?? String.Empty,
                            Image = item?.Snippet?.Thumbnails?.Medium?.Url ?? String.Empty,
                            Liked = false
                        };
                        results.Add(video);
                        if (results.Count >= desiredCount)
                        {
                            break;
                        }
                    }
                }
            }

            if (String.IsNullOrEmpty(response.NextPageToken))
            {
                break;
            }

            pageToken = response.NextPageToken;
        }
        return results.AsEnumerable();
    }

    public async Task<IEnumerable<Video>> GetNewestNotLikedVideosAsync(int desiredCount, int maxResult, Uri cacheUri, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        likedVideoIds ??= await GetLikedVideoIdsAsync(maxResult, cancellationToken).ConfigureAwait(false);
        var videos = await GetNewestVideosAsync(desiredCount, maxResult, cacheUri, searchTerm, cancellationToken).ConfigureAwait(false);
        return videos.Where(video => !likedVideoIds.Contains(video.VideoId));
    }

    public Task<string> RateVideoAsync(string videoId, RatingEnum rating)
    {
        if (rating == RatingEnum.Like)
        {
            likedVideoIds = likedVideoIds?.Append(videoId);
        }
        return youTubeService.Videos.Rate(videoId, rating).ExecuteAsync();
    }

    public Task<CommentThread> AddCommentAsync(string videoId, string text)
    {
        var comment = new Comment
        {
            Snippet = new CommentSnippet
            {
                TextOriginal = text
            }
        };

        var insertRequest = youTubeService.CommentThreads.Insert(
            new CommentThread
            {
                Snippet = new CommentThreadSnippet
                {
                    VideoId = videoId,
                    TopLevelComment = comment
                }
            },
            "snippet"
        );

        return insertRequest.ExecuteAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
            youTubeService.Dispose();
        }

        disposed = true;
    }

    ~YouTubeServiceWrapper()
    {
        Dispose(false);
    }
}
