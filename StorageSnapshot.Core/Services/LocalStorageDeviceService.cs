﻿using System;
using System.IO;
using System.Management;
using System.Runtime.CompilerServices;
using System.Threading;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Exceptions;
using StorageSnapshot.Core.Helpers;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.Core.Services;
public class LocalStorageDeviceService : ILocalStorageDeviceService
{
    private List<LocalStorageDevice> _allStorageDeviceInfos;

    private readonly Dictionary<LocalStorageDevice, Task<LocalStorageDeviceDetails>> _localStorageDeviceDetailsTasks = new();

    public async Task Initialize()
    {
        // Create a list to store the tasks
        var tasks = new List<Task>();

        var localStorageDevices = await GetLocalStorageDevicesAsync();

        foreach (var localStorageDevice in localStorageDevices)
        {
            // Create a task for each device to cache the details
            var task = GetLocalStorageDeviceDetailsAsync(localStorageDevice);
            tasks.Add(task);
        }

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);
        System.Diagnostics.Debug.WriteLine("LocalStorageDeviceService Initialized");
    }

    public async Task<IEnumerable<LocalStorageDevice>> GetLocalStorageDevicesAsync()
    {
        _allStorageDeviceInfos ??= new List<LocalStorageDevice>(GetDriveInfo());

        await Task.CompletedTask;
        return _allStorageDeviceInfos;
    }

    private static IEnumerable<LocalStorageDevice> GetDriveInfo()
    {
        var drives = DriveInfo.GetDrives().Select(x => new LocalStorageDevice(x));

        return drives.ToList().Where(DriveIsPermissible);
    }

    private static bool DriveIsPermissible(LocalStorageDevice drive)
    {
        try
        {
            _ = drive.VolumeLabel;
            return true;
        }
        catch { }
        return false;
    }

    public async Task<LocalStorageDeviceDetails> GetLocalStorageDeviceDetailsAsync(LocalStorageDevice localStorageDevice)
    {
        if (!_localStorageDeviceDetailsTasks.ContainsKey(localStorageDevice))
        {
            _localStorageDeviceDetailsTasks[localStorageDevice] = GetLocalStorageDeviceDetailsAsyncInternal(localStorageDevice);
        }

        return await _localStorageDeviceDetailsTasks[localStorageDevice];
    }

    private Task<LocalStorageDeviceDetails> GetLocalStorageDeviceDetailsAsyncInternal(LocalStorageDevice localStorageDevice)
    {
        return Task.Run(() =>
        {
            LocalStorageDeviceDetails localStorageDeviceDetails = new LocalStorageDeviceDetails();
            GetLocalStorageDeviceDetails(localStorageDevice.RootDirectory, localStorageDeviceDetails);
            return localStorageDeviceDetails;
        });
    }

    private void GetLocalStorageDeviceDetails(DirectoryInfo rootDirectory, LocalStorageDeviceDetails localStorageDeviceDetails)
    {
        try
        {
            foreach (FileInfo file in rootDirectory.EnumerateFiles())
            {
                localStorageDeviceDetails.TotalFiles++;

                if (!MimeTypeResolver.TryGetMimeType(file, out var mimeType))
                {
                    // Skip unknown MIME types for now
                    continue;
                }
                
                if (!localStorageDeviceDetails.MimeTypeDetailsDictionary.ContainsKey(mimeType))
                {
                    localStorageDeviceDetails.MimeTypeDetailsDictionary.Add(mimeType, new MimeTypeDetails(file.Extension, mimeType));
                }
                localStorageDeviceDetails.MimeTypeDetailsDictionary[mimeType].TotalFiles++;
                localStorageDeviceDetails.MimeTypeDetailsDictionary[mimeType].TotalSize += file.Length;
            }

            foreach (DirectoryInfo directoryInfo in rootDirectory.EnumerateDirectories())
            {
                localStorageDeviceDetails.TotalDirectories++;

                GetLocalStorageDeviceDetails(directoryInfo, localStorageDeviceDetails);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Catch and ignore this exception if we don't have access to the directory
        }
    }







}
