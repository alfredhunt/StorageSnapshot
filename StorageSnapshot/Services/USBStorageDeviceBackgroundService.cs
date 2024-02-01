using System.Management;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Hosting;
using StorageSnapshot.Contracts.Services;
using StorageSnapshot.Helpers;
using StorageSnapshot.Messages;
using Windows.System;

namespace StorageSnapshot.Services;

internal class USBStorageDeviceBackgroundService : BackgroundService
{
    private ManagementEventWatcher? insertWatcher;
    private ManagementEventWatcher? removeWatcher;

    private readonly Dictionary<string, string> _connectedDevices = new();

    public void StartUSBDeviceMonitoring()
    {
        // Watcher for USB connection
        insertWatcher = new ManagementEventWatcher();
        var queryInsert = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_DiskDrive'");
        insertWatcher.Query = queryInsert;
        insertWatcher.EventArrived += (sender, eventArgs) =>
        {
            var instance = eventArgs.NewEvent["TargetInstance"] as ManagementBaseObject;
            var deviceIdObject = instance?["DeviceID"];
            if (deviceIdObject == null)
            {
                return;
            }
            var deviceId = deviceIdObject.ToString();
            if (deviceId == null)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine($"USB device inserted: {instance?["Name"]}, DeviceID: {deviceId}");

            // For the device path, you might need to query associated instances
            // For example, querying Win32_DiskPartition or Win32_LogicalDisk to get more details
            var searcher = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{deviceId}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
            foreach (var partition in searcher.Get())
            {
                var logicalSearcher = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");
                foreach (var logicalDisk in logicalSearcher.Get())
                {
                    var logicalDiskIdObject = logicalDisk?["DeviceID"];
                    if (logicalDiskIdObject == null)
                    {
                        continue;
                    }
                    var logicalDiskId = logicalDiskIdObject.ToString();
                    if (logicalDiskId == null)
                    {
                        return;
                    }

                    // This gives you the drive letter, which is often used as the path for removable drives
                    System.Diagnostics.Debug.WriteLine($"Logical Disk: {logicalDiskId}");

                    _connectedDevices.Add(deviceId, logicalDiskId); // Store additional information as needed

                    // Send a message from some other module
                    var message = new UsbDeviceAddedMessage() { DeviceId = logicalDiskId };
                    WeakReferenceMessenger.Default.Send(message);
                }
            }
        };

        insertWatcher.Start();

        // Watcher for USB removal
        removeWatcher = new ManagementEventWatcher();
        var queryRemove = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_DiskDrive'");
        removeWatcher.Query = queryRemove;
        removeWatcher.EventArrived += (sender, eventArgs) =>
        {
            var instance = eventArgs.NewEvent["TargetInstance"] as ManagementBaseObject;
            var deviceIdObject = instance?["DeviceID"];
            if (deviceIdObject == null)
            {
                return;
            }
            var deviceId = deviceIdObject.ToString();
            if (deviceId == null)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine($"USB device removed: {instance?["Name"]}, DeviceID: {deviceId}");

            if (_connectedDevices.ContainsKey(deviceId))
            {
                var devicePath = _connectedDevices[deviceId.ToString()];
                System.Diagnostics.Debug.WriteLine($"USB device removed: DeviceID: {deviceId}, Path: {devicePath}");

                var message = new UsbDeviceRemovedMessage { DeviceId = devicePath };
                WeakReferenceMessenger.Default.Send(message);

                _connectedDevices.Remove(deviceId.ToString()); // Clean up
            }
        };

        removeWatcher.Start();
    }

    public void StopUSBDeviceMonitoring()
    {
        // Stop listening
        if (insertWatcher != null) insertWatcher.Stop();
        if (removeWatcher != null) removeWatcher.Stop();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        StartUSBDeviceMonitoring();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        StopUSBDeviceMonitoring();
        base.Dispose();
    }

    public void AddConnectedDevice(string deviceId, string driveLetter) => _connectedDevices.Add(deviceId, driveLetter);
}
