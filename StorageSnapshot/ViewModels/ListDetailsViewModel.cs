using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Messages;

namespace StorageSnapshot.ViewModels;

public partial class ListDetailsViewModel : ObservableRecipient, INavigationAware,
    IRecipient<UsbDeviceAddedMessage>, IRecipient<UsbDeviceRemovedMessage>
{
    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    [ObservableProperty]
    private LocalStorageDeviceViewModel? selected;

    public ObservableCollection<LocalStorageDeviceViewModel> LocalStorageDevices { get; private set; } = new ObservableCollection<LocalStorageDeviceViewModel>();

    private readonly DispatcherQueue dispatcherQueue;

    public ListDetailsViewModel(ILocalStorageDeviceService localStorageDeviceService)
    {
        _localStorageDeviceService = localStorageDeviceService;

        dispatcherQueue = DispatcherQueue.GetForCurrentThread();
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
        dispatcherQueue.TryEnqueue(async () => {
            DriveInfo driveInfo = new(message.DeviceId);
            LocalStorageDevice localStorageDevice = new(driveInfo);
            var vm = new LocalStorageDeviceViewModel(_localStorageDeviceService, localStorageDevice);

            LocalStorageDevices.Add(vm);

            await vm.LoadDetailsAsync();
        });
    }

    public void Receive(UsbDeviceRemovedMessage message)
    {
        dispatcherQueue.TryEnqueue(() => {
            DriveInfo driveInfo = new(message.DeviceId);

            var vm = LocalStorageDevices.FirstOrDefault(x => x.Device.Name == driveInfo.Name);

            if (vm != null)
                LocalStorageDevices.Remove(vm);
        });
    }
}
