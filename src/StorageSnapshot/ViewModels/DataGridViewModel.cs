using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.ViewModels;

public partial class DataGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly IStorageDeviceService _storageDeviceService;

    public ObservableCollection<LocalStorageDevice> Source { get; } = new ObservableCollection<LocalStorageDevice>();

    public DataGridViewModel(IStorageDeviceService storageDeviceService)
    {
        _storageDeviceService = storageDeviceService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await _storageDeviceService.GetAllLocalStorageDevicesAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
