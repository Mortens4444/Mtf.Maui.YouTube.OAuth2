using Google.Apis.Util.Store;
using System.Text.Json;

namespace Mtf.Maui.YouTube.OAuth2.Services;

public class SecureStorageDataStore : IDataStore
{
    public Task ClearAsync()
    {
        SecureStorage.Remove("Google.Apis.Auth.OAuth2.user");
        return Task.CompletedTask;
    }

    public Task DeleteAsync<T>(string key)
    {
        _ = SecureStorage.Remove(key);
        return Task.CompletedTask;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await SecureStorage.GetAsync(key).ConfigureAwait(false);
        if (String.IsNullOrEmpty(json))
        {
            return default;
        }

        var obj = JsonSerializer.Deserialize<T>(json);
        return obj;
    }

    public Task StoreAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        return SecureStorage.SetAsync(key, json);
    }
}
