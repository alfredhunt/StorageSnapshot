using CommunityToolkit.Mvvm.Messaging;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Core.Services;
using StorageSnapshot.ViewModels;

namespace StorageSnapshot.Messages;

public sealed class UsbDeviceRemovedMessage : IMessenger
{
    public required string DeviceId
    {
        get; set;
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

