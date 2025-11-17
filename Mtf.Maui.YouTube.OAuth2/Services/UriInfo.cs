using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mtf.Maui.YouTube.OAuth2.Services;

public static class UriInfo
{
    public static async Task<bool> IsOutDated(Uri requestUri)
    {
        var lastModified = await GetLastModifiedAsync(requestUri).ConfigureAwait(false);
        return lastModified.HasValue && lastModified < DateTime.UtcNow.Date;
    }

    public static async Task<DateTime?> GetLastModifiedAsync(Uri requestUri)
    {
        using var client = new HttpClient();

        using var request = new HttpRequestMessage(HttpMethod.Head, requestUri);
        using var response = await client.SendAsync(request).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        if (response.Content?.Headers?.LastModified == null)
        {
            return null;
        }

        return response.Content.Headers.LastModified.Value.UtcDateTime;
    }
}
