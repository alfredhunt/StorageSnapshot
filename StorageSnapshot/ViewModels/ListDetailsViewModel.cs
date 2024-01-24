using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using StorageSnapshot.Contracts.Services;
using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Services;

namespace StorageSnapshot.ViewModels;

public partial class ListDetailsViewModel : ObservableRecipient, INavigationAware
{
    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    [ObservableProperty]
    private LocalStorageDeviceViewModel? selected;

    public ObservableCollection<LocalStorageDeviceViewModel> LocalStorageDevices { get; private set; } = new ObservableCollection<LocalStorageDeviceViewModel>();

    public ListDetailsViewModel(ILocalStorageDeviceService localStorageDeviceService)
    {
        _localStorageDeviceService = localStorageDeviceService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        LocalStorageDevices.Clear();
        var data = await _localStorageDeviceService.GetAllLocalStorageDevicesAsync();
        foreach (var localStorageDevice in data)
        {
            var vm = new LocalStorageDeviceViewModel(_localStorageDeviceService, localStorageDevice);
            LocalStorageDevices.Add(vm);
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
