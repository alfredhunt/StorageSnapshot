using Microsoft.UI.Xaml.Controls;

using StorageSnapshot.ViewModels;

namespace StorageSnapshot.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    private void OverviewButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void GridViewButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void ListViewButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
}
