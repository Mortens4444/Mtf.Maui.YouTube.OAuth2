using Google.Api;
using Mtf.Maui.YouTube.OAuth2.Test.ViewModels;

namespace Mtf.Maui.YouTube.OAuth2.Test;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
