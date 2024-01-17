using System;
using System.IO;
using System.Management;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Helpers;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.Core.Services;
public class LocalStorageDeviceService : ILocalStorageDeviceService
{
    private List<LocalStorageDevice> _allStorageDeviceInfos;

    private readonly Dictionary<LocalStorageDevice, Task<LocalStorageDeviceDetails>> _cache
        = new Dictionary<LocalStorageDevice, Task<LocalStorageDeviceDetails>>();

    public async Task<IEnumerable<LocalStorageDevice>> GetAllLocalStorageDevicesAsync()
    {

        _allStorageDeviceInfos ??= new List<LocalStorageDevice>(GetDriveInfo());

        await Task.CompletedTask;
        return _allStorageDeviceInfos;
    }

    private static IEnumerable<LocalStorageDevice> GetDriveInfo()
    {
        return DriveInfo.GetDrives().Select(x => new LocalStorageDevice(x));
    }

    public async Task GetLocalStorageDeviceDetailsAsync(LocalStorageDevice localStorageDevice)
    {
        await Task.Run(() =>
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            System.Diagnostics.Debug.WriteLine($"Getting details for: {localStorageDevice.Name}");
            GetLocalStorageDeviceDetails(localStorageDevice.RootDirectory, localStorageDevice.Details);
            stopwatch.Stop();
            TimeSpan duration = stopwatch.Elapsed;
            System.Diagnostics.Debug.WriteLine($"Task completed for {localStorageDevice.Name} in {duration.TotalMilliseconds} milliseconds.");
        });


    }

    private void GetLocalStorageDeviceDetails(DirectoryInfo rootDirectory, LocalStorageDeviceDetails localStorageDeviceDetails)
    {
        try
        {
            foreach (FileInfo file in rootDirectory.EnumerateFiles())
            {
                Console.WriteLine($"File: {file.FullName}");
                localStorageDeviceDetails.TotalFiles++;

                var mimeType = MimeTypeResolver.GetMimeType(file);
                if (!localStorageDeviceDetails.MimeTypeDetailsDictionary.ContainsKey(mimeType))
                {
                    localStorageDeviceDetails.MimeTypeDetailsDictionary.Add(mimeType, new MimeTypeDetails(file.Extension, mimeType));
                }
                localStorageDeviceDetails.MimeTypeDetailsDictionary[mimeType].TotalFiles++;
                localStorageDeviceDetails.MimeTypeDetailsDictionary[mimeType].TotalSize+= file.Length;

            }

            foreach (DirectoryInfo directoryInfo in rootDirectory.EnumerateDirectories())
            {
                Console.WriteLine($"Directory: {directoryInfo.FullName}");
                localStorageDeviceDetails.TotalDirectories++;

                //// Check if the directory is a protected system directory
                //if ((directoryInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                //{
                //    System.Diagnostics.Debug.WriteLine($"This directory is a protected system directory. {directoryInfo.FullName}");
                //    continue;
                //}

                GetLocalStorageDeviceDetails(directoryInfo, localStorageDeviceDetails);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            // Catch and ignore this exception if we don't have access to the directory
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }






    private void StartWin32USBControllerDeviceMonitoring()
    {
        WqlEventQuery query;
        ManagementEventWatcher watcher;

        // Bind to local machine
        ConnectionOptions opt = new ConnectionOptions
        {
            EnablePrivileges = true
        };
        ManagementScope scope = new ManagementScope("root\\CIMV2", opt);

        try
        {
            query = new WqlEventQuery("__InstanceOperationEvent", new TimeSpan(0, 0, 1), "TargetInstance isa 'Win32_USBControllerdevice'");
            watcher = new ManagementEventWatcher(scope, query);
            watcher.EventArrived += new EventArrivedEventHandler(USBChanged);
            watcher.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
            return;
        }

        Console.WriteLine("Waiting for an event...");

    }

    static void USBChanged(object sender, EventArrivedEventArgs e)
    {
        // Get the Event object and display it
        PropertyData pd = e.NewEvent.Properties["EventType"];
        ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
        foreach (var property in instance.Properties)
        {
            if (property.Name == "Dependent")
            {
                // Parse the antecedent physical device id
                string antecedent = property.Value.ToString().Split('=')[1].Replace("\"", "");

                // Query the device using the device id
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE PNPDeviceID = '" + antecedent + "'");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    switch (Convert.ToInt16(pd.Value))
                    {
                        case 2: Console.WriteLine("A USB storage device has been inserted"); break;
                        case 3: Console.WriteLine("A USB storage device has been removed"); break;
                    }
                }
            }
        }
    }

}
