using Android.App;
using Android.Content;
using Android.Content.PM;

namespace com.company.application;

[Activity(Exported = true, NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
[IntentFilter([Intent.ActionView],
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "Application Name")]
public class WebCallbackActivity : WebAuthenticatorCallbackActivity
{
}