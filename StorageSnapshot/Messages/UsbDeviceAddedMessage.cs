namespace StorageSnapshot.Messages;

public sealed class UsbDeviceAddedMessage
{
    public required string DeviceId
    {
        get; set;
    }
}
