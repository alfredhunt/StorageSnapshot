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







}
