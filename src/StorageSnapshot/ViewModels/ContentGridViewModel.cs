using System.Collections.ObjectModel;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using StorageSnapshot.Contracts.Services;
using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.ViewModels;

public partial class ContentGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly ILocalStorageDeviceService _storageDeviceService;

    public ObservableCollection<LocalStorageDevice> Source { get; } = new ObservableCollection<LocalStorageDevice>();

    public ContentGridViewModel(INavigationService navigationService, ILocalStorageDeviceService storageDeviceService)
    {
        _navigationService = navigationService;
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

    [RelayCommand]
    private void OnItemClick(LocalStorageDevice? clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(ContentGridDetailViewModel).FullName!, clickedItem.Name);
        }
    }
}
