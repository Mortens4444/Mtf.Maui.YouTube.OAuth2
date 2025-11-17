namespace Mtf.Maui.YouTube.OAuth2.Services;

public static class WebDownloader
{
    public static async Task<string> DownloadFileAsync(Uri requestUri)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(requestUri).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var tempFile = Path.GetTempFileName();
            using var fileStream = new FileStream(tempFile, FileMode.Create);
            await stream.CopyToAsync(fileStream).ConfigureAwait(false);
            return tempFile;
        }
        else
        {
            throw new InvalidOperationException($"Download failed with status code: {response.StatusCode}");
        }
    }
}