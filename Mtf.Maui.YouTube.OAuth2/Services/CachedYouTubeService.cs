using Mtf.Maui.YouTube.OAuth2.Models;

namespace Mtf.Maui.YouTube.OAuth2.Services;

public static class CachedYouTubeService
{
    public static async Task<IEnumerable<Video>> GetVideoListAsync(Uri cacheUri, string? searchTerm = null)
    {
        var tempFileName = await WebDownloader.DownloadFileAsync(cacheUri).ConfigureAwait(false);
        var result = await VideoCsvService.ReadVideosFromCsvAsync(tempFileName).ConfigureAwait(false);
        try
        {
            File.Delete(tempFileName);
        }
        catch (Exception ex)
        {
            ErrorDisplayer.ShowError(ex);
        }

        if (String.IsNullOrEmpty(searchTerm))
        {
            return result;
        }
        return result.Where(video => video.Title.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) || video.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
    }
}
