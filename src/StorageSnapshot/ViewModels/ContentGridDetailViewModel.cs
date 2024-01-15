using CommunityToolkit.Mvvm.ComponentModel;

using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.ViewModels;

public partial class ContentGridDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly IStorageDeviceService _storageDeviceService;

    [ObservableProperty]
    private LocalStorageDevice? item;

    public ContentGridDetailViewModel(IStorageDeviceService storageDeviceService)
    {
        _storageDeviceService = storageDeviceService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is string name)
        {
            var data = await _storageDeviceService.GetAllLocalStorageDevicesAsync();
            Item = data.First(i => i.Name == name);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
