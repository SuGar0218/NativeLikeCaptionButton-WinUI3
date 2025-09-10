using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using Microsoft.UI.Windowing;

using SuGarToolkit.WinUI3.Samples.NativeLikeCaptionButton.ViewModels;

namespace SuGarToolkit.WinUI3.Samples.NativeLikeCaptionButton.Views;

internal sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
    }

    public class NavigationParameter
    {
        public required Window OwnerWindow { get; set; }

        public required TitleBarElementViewModel ViewModel { get; set; }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        NavigationParameter parameter = (NavigationParameter) e.Parameter;
        ViewModel = parameter.ViewModel;
        OwnerWindow = parameter.OwnerWindow;

        base.OnNavigatedTo(e);
    }

    private TitleBarElementViewModel ViewModel { get; set; }

    private Window OwnerWindow { get; set; }

    private void TitleBarAreaPaneToggleButtonClick(Controls.NativeLikeCaptionButton.NativeLikeTitleBar sender, System.EventArgs args)
    {
        RootNavigationView.IsPaneOpen = !RootNavigationView.IsPaneOpen;
    }

    private void FullScreenButtonClick(object sender, RoutedEventArgs e)
    {
        if (OwnerWindow.AppWindow.Presenter.Kind is AppWindowPresenterKind.Overlapped)
        {
            OwnerWindow.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
        else
        {
            OwnerWindow.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
        }
    }
}
