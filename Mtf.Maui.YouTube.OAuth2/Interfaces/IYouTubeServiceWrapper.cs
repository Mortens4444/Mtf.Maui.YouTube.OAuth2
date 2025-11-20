using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Video = Mtf.Maui.YouTube.OAuth2.Models.Video;

namespace Mtf.Maui.YouTube.OAuth2.Interfaces;

public interface IYouTubeServiceWrapper
{
    Task<CommentThread> AddCommentAsync(string videoId, string text);

    void Dispose();

    Task<IEnumerable<Video>> GetAllVideosAsync(string? searchTerm = null, CancellationToken cancellationToken = default);

    Task<HashSet<string>> GetLikedVideoIdsAsync(int maxResult, CancellationToken cancellationToken = default);

    Task<IEnumerable<Video>> GetNewestNotLikedVideosAsync(int desiredCount, int maxResult, Uri? cacheUri = null, string? searchTerm = null, CancellationToken cancellationToken = default);

    Task<string> RateVideoAsync(string videoId, VideosResource.RateRequest.RatingEnum rating);
}