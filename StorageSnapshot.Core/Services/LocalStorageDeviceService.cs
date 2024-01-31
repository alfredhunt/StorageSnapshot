using System;
using System.Diagnostics;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Exceptions;
using StorageSnapshot.Core.Helpers;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.Core.Services;

public class LocalStorageDeviceService : ILocalStorageDeviceService
{
    private List<LocalStorageDevice> _allStorageDeviceInfos;

    public void ReloadDrives() => _allStorageDeviceInfos = null;
    public void AddDevice(LocalStorageDevice item) => _allStorageDeviceInfos?.Add(item);
    public void RemoveDevice(LocalStorageDevice item) => _allStorageDeviceInfos?.Remove(item);

    private readonly Dictionary<LocalStorageDevice, Task<LocalStorageDeviceAnalysis>> _localStorageDeviceDetailsTasks = new();

    public IAccessControlService AccessControlService
    {
        get; private set;
    }

    public LocalStorageDeviceService(IAccessControlService accessControlService)
    {
        AccessControlService = accessControlService;
    }

    public async Task Initialize()
    {
        // Timing
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

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

        // Get the elapsed time as a TimeSpan value.
        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;

        // Format and display the TimeSpan value.
        var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

        System.Diagnostics.Debug.WriteLine($"LocalStorageDeviceService Initialized in {elapsedTime}");
    }

    public async Task<IEnumerable<LocalStorageDevice>> GetLocalStorageDevicesAsync()
    {
        _allStorageDeviceInfos ??= new List<LocalStorageDevice>(GetDriveInfo());

        await Task.CompletedTask;
        return _allStorageDeviceInfos;
    }

    private IEnumerable<LocalStorageDevice> GetDriveInfo()
    {
        var drives = DriveInfo.GetDrives()
            .Where(AccessControlService.CanListDirectory)
            .Select(x => new LocalStorageDevice(x));

        return drives;
    }

    public async Task<LocalStorageDeviceAnalysis> GetLocalStorageDeviceDetailsAsync(LocalStorageDevice localStorageDevice)
    {
        if (!_localStorageDeviceDetailsTasks.ContainsKey(localStorageDevice))
        {
            _localStorageDeviceDetailsTasks[localStorageDevice] =
                Task.Run(() =>
                {
                    LocalStorageDeviceAnalysis result = null;
                    System.Diagnostics.Debug.WriteLine($"Analyzing {localStorageDevice.RootDirectory}");
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    try
                    {
                        result = GetDirectoryAnalysis(localStorageDevice.RootDirectory);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }

                    stopwatch.Stop();
                    TimeSpan ts = stopwatch.Elapsed;

                    var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);

                    System.Diagnostics.Debug.WriteLine($"{localStorageDevice.RootDirectory} analyzed in {elapsedTime}");
                    return result;
                });
        }
        return await _localStorageDeviceDetailsTasks[localStorageDevice];
    }

    private LocalStorageDeviceAnalysis GetDirectoryAnalysis(DirectoryInfo rootDirectory)
    {
        LocalStorageDeviceAnalysis details = new LocalStorageDeviceAnalysis();

        foreach (var fileSystemInfo in rootDirectory.EnumerateFileSystemInfos())
        {
            details.TotalDirectories++;
            if (fileSystemInfo is FileInfo fileInfo)
            {
                details.TotalFiles++;

                if (!MimeTypeResolver.TryGetMimeType(fileInfo, out var mimeType))
                {
                    // Skip unknown MIME types for now
                    continue;
                }
                if (!details.MimeTypeDetailsDictionary.ContainsKey(mimeType))
                {
                    details.MimeTypeDetailsDictionary.Add(mimeType, new MimeTypeDetails(fileInfo.Extension, mimeType));
                }
                details.MimeTypeDetailsDictionary[mimeType].TotalFiles++;
                details.MimeTypeDetailsDictionary[mimeType].TotalSize += fileInfo.Length;
            }
            else if (fileSystemInfo is DirectoryInfo directoryInfo)
            {
                if (fileSystemInfo.FullName.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Windows), StringComparison.OrdinalIgnoreCase) ||
                    fileSystemInfo.FullName.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), StringComparison.OrdinalIgnoreCase) ||
                    fileSystemInfo.FullName.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (AccessControlService.IsSystemFolder(directoryInfo) || !AccessControlService.CanListDirectory(directoryInfo))
                {
                    continue;
                }

                details += GetDirectoryAnalysis(directoryInfo);
            }
        }

        return details;
    }
}
