using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.ViewModels;

public partial class ListDetailsViewModel : ObservableRecipient, INavigationAware
{
    private readonly IStorageDeviceService _storageDeviceService;

    [ObservableProperty]
    private LocalStorageDevice? selected;

    public ObservableCollection<LocalStorageDevice> LocalStorageDevices { get; private set; } = new ObservableCollection<LocalStorageDevice>();

    public ListDetailsViewModel(IStorageDeviceService storageDeviceService)
    {
        _storageDeviceService = storageDeviceService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        LocalStorageDevices.Clear();

        // TODO: Replace with real data.
        var data = await _storageDeviceService.GetAllLocalStorageDevicesAsync();

        foreach (var item in data)
        {
            LocalStorageDevices.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void EnsureItemSelected()
    {
        Selected ??= LocalStorageDevices.First();
    }
}
