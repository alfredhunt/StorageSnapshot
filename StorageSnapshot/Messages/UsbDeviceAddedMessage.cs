using CommunityToolkit.Mvvm.Messaging;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Core.Services;
using StorageSnapshot.ViewModels;

namespace StorageSnapshot.Messages;

public class UsbDeviceAddedMessage : IMessenger
{
    public required string DeviceId
    {
        get; set;
    }

    public UsbDeviceAddedMessage(string deviceId)
    {
        var driveInfo = DriveInfo.GetDrives().FirstOrDefault(x => x.RootDirectory.Name == deviceId) ?? throw new Exception();
        var localStorageDevice = new LocalStorageDevice(driveInfo);

        var ListVM = App.GetService<ListDetailsViewModel>();
        if (ListVM.LocalStorageDeviceService is LocalStorageDeviceService service)
        {
            service.AddDevice(localStorageDevice);
        }
    }

    public void Cleanup() => throw new NotImplementedException();

    public bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken> => throw new NotImplementedException();

    public void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler)
        where TRecipient : class
        where TMessage : class
        where TToken : IEquatable<TToken> => throw new NotImplementedException();

    public void Reset() => throw new NotImplementedException();

    public TMessage Send<TMessage, TToken>(TMessage message, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken> => throw new NotImplementedException();

    public void Unregister<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken> => throw new NotImplementedException();

    public void UnregisterAll(object recipient) => throw new NotImplementedException();

    public void UnregisterAll<TToken>(object recipient, TToken token) where TToken : IEquatable<TToken> => throw new NotImplementedException();
}
