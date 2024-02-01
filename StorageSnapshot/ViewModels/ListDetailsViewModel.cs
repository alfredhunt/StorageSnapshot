using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Messages;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace StorageSnapshot.ViewModels;

public partial class ListDetailsViewModel : ObservableRecipient, INavigationAware,
    IRecipient<UsbDeviceAddedMessage>, IRecipient<UsbDeviceRemovedMessage>
{
    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    [ObservableProperty]
    private LocalStorageDeviceViewModel? selected;

    public ObservableCollection<LocalStorageDeviceViewModel> LocalStorageDevices { get; private set; } = new ObservableCollection<LocalStorageDeviceViewModel>();

    public ListDetailsViewModel(ILocalStorageDeviceService localStorageDeviceService)
    {
        _localStorageDeviceService = localStorageDeviceService;

        WeakReferenceMessenger.Default.Register<UsbDeviceAddedMessage>(this);
        WeakReferenceMessenger.Default.Register<UsbDeviceRemovedMessage>(this);
    }


    public async void OnNavigatedTo(object parameter)
    {
        LocalStorageDevices.Clear();
        var data = await _localStorageDeviceService.GetLocalStorageDevicesAsync();
        foreach (var localStorageDevice in data)
        {
            var vm = new LocalStorageDeviceViewModel(_localStorageDeviceService, localStorageDevice);
            LocalStorageDevices.Add(vm);
            _ = vm.LoadDetailsAsync();
        }
    }

    public void OnNavigatedFrom()
    {

    }

    public void EnsureItemSelected()
    {
        Selected ??= LocalStorageDevices.First();
    }

    public void Receive(UsbDeviceAddedMessage message)
    {
        DriveInfo driveInfo = new(message.DeviceId);
        LocalStorageDevice localStorageDevice = new(driveInfo);
        var vm = new LocalStorageDeviceViewModel(_localStorageDeviceService, localStorageDevice);

        LocalStorageDevices.Add(vm);
    }

    public void Receive(UsbDeviceRemovedMessage message)
    {
        DriveInfo driveInfo = new(message.DeviceId);
        LocalStorageDevice localStorageDevice = new(driveInfo);
        var vm = new LocalStorageDeviceViewModel(_localStorageDeviceService, localStorageDevice);

        LocalStorageDevices.Remove(vm);
    }
}
