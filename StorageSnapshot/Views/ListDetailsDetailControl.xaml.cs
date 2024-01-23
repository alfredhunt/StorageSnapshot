using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using StorageSnapshot.Core.Models;
using StorageSnapshot.ViewModels;

namespace StorageSnapshot.Views;

public sealed partial class ListDetailsDetailControl : UserControl
{
    public LocalStorageDeviceViewModel? ListDetailsMenuItem
    {
        get => GetValue(ListDetailsMenuItemProperty) as LocalStorageDeviceViewModel;
        set => SetValue(ListDetailsMenuItemProperty, value);
    }

    public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(LocalStorageDeviceViewModel), typeof(ListDetailsDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

    public ListDetailsDetailControl()
    {
        InitializeComponent();
    }

    private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListDetailsDetailControl control)
        {
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
