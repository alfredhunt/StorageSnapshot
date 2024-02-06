using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Core.Services;
using StorageSnapshot.Messages;

namespace StorageSnapshot.ViewModels;

public partial class MainViewModel : ObservableRecipient,
    IRecipient<UsbDeviceAddedMessage>, IRecipient<UsbDeviceRemovedMessage>
{

    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    public MainViewModel(ILocalStorageDeviceService localStorageDeviceService)
    {
        _localStorageDeviceService = localStorageDeviceService;

        WeakReferenceMessenger.Default.Register<UsbDeviceAddedMessage>(this);
        WeakReferenceMessenger.Default.Register<UsbDeviceRemovedMessage>(this);
    }

    public void Receive(UsbDeviceAddedMessage message)
    {
        DriveInfo driveInfo = new(message.DeviceId);
        LocalStorageDevice localStorageDevice = new(driveInfo);

        if (_localStorageDeviceService is LocalStorageDeviceService localStorageDeviceService)
        {
            localStorageDeviceService.AddDevice(localStorageDevice);
        }    
    }

    public void Receive(UsbDeviceRemovedMessage message)
    {
        DriveInfo driveInfo = new(message.DeviceId);

        if (_localStorageDeviceService is LocalStorageDeviceService localStorageDeviceService)
        {
            var device = localStorageDeviceService.AllStorageDeviceInfos.FirstOrDefault(x => x.Name == driveInfo.Name);
            if (device == null)
            {
                return;
            }

            localStorageDeviceService.RemoveDevice(device);
        }
    }
}
