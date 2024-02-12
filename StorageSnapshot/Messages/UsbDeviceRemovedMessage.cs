namespace StorageSnapshot.Messages;

public sealed class UsbDeviceRemovedMessage
{
    public required string DeviceId
    {
        get; set;
    }
}

