using System.Collections.ObjectModel;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using StorageSnapshot.Contracts.Services;
using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Core.Services;

namespace StorageSnapshot.ViewModels;

public partial class ContentGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    public ObservableCollection<LocalStorageDeviceViewModel> LocalStorageDevices { get; } = new ObservableCollection<LocalStorageDeviceViewModel>();

    public ContentGridViewModel(INavigationService navigationService, ILocalStorageDeviceService localStorageDeviceService)
    {
        _navigationService = navigationService;
        _localStorageDeviceService = localStorageDeviceService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        var tasks = new List<Task>();
        LocalStorageDevices.Clear();

        var data = await _localStorageDeviceService.GetAllLocalStorageDevicesAsync();

        foreach (var localStorageDevice in data)
        {
            var vm = new LocalStorageDeviceViewModel(localStorageDevice);
            LocalStorageDevices.Add(vm);

            var task = Task.Run(() => {
                App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
                {
                    vm.Details = await _localStorageDeviceService.GetLocalStorageDeviceDetailsAsync(localStorageDevice);
                });
                return Task.CompletedTask;
            });
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
        System.Diagnostics.Debug.WriteLine("ContentGridViewModel::OnNavigatedTo Finished");
    }

    public void OnNavigatedFrom()
    {
    }

    [RelayCommand]
    private void OnItemClick(LocalStorageDeviceViewModel? clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(ContentGridDetailViewModel).FullName!, clickedItem);
        }
    }
}
