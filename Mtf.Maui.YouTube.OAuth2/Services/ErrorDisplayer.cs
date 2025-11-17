using System.Diagnostics;

namespace Mtf.Maui.YouTube.OAuth2.Services;

public static class ErrorDisplayer
{
    public static void ShowError(string message)
    {
        Debug.WriteLine(message);
        var app = Application.Current;
        var page = app?.Windows?.Count > 0 ? app.Windows[0].Page : null;
        if (page != null)
        {
            MainThread.BeginInvokeOnMainThread(() => page.DisplayAlert("⚠️ Error", $"Something went wrong. Details: {message}", "OK"));
        }
    }

    public static void ShowError(Exception ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        Debug.WriteLine(ex);
        ShowError(ex.Message);
    }
}
