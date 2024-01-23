using CommunityToolkit.Mvvm.ComponentModel;

using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Core.Services;

namespace StorageSnapshot.ViewModels;

public partial class ContentGridDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    [ObservableProperty]
    private LocalStorageDeviceViewModel? item;

    public ContentGridDetailViewModel(ILocalStorageDeviceService localStorageDeviceService)
    {
        _localStorageDeviceService = localStorageDeviceService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is LocalStorageDeviceViewModel vm)
        {
            Item = vm;
            App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
            {
                vm.Details = await _localStorageDeviceService.GetLocalStorageDeviceDetailsAsync(vm.Device);
            });
            return;
        }
        //if (parameter is string name)
        //{
        //    var data = await _storageDeviceService.GetAllLocalStorageDevicesAsync();
        //    Item = data.First(i => i.Name == name);
        //}
    }

    public void OnNavigatedFrom()
    {
    }
}
