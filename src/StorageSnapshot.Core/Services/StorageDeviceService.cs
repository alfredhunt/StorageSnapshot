using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Helpers;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.Core.Services;
public class StorageDeviceService : IStorageDeviceService
{
    private List<LocalStorageDevice> _allStorageDeviceInfos;

    private static IEnumerable<LocalStorageDevice> GetDriveInfo()
    {
        return DriveInfo.GetDrives().Select(x => new LocalStorageDevice(x));
    }

    public void AnalyzeDriveContents()
    {
        // For each drive on the system, recursively enumerate all files and directories
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            if (drive.IsReady == true)
            {
                Console.WriteLine($"Analyzing drive {drive.Name}...");
                try
                {
                    EnumerateDirectories(drive.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while analyzing drive {drive.Name}: {ex.Message}");
                }
            }
        }
    }

    private void EnumerateDirectories(string path)
    {
        try
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                Console.WriteLine($"Directory: {directory}");
                EnumerateDirectories(directory); // Recursively enumerate subdirectories
            }

            foreach (string file in Directory.GetFiles(path))
            {
                Console.WriteLine($"File: {file}");

                string mimeType = MimeTypeResolver.GetMimeType(file);

                switch (mimeType)
                {
                    case "image/jpeg":
                        // Logic for JPEG files
                        break;
                    case "image/png":
                        // Logic for PNG files
                        break;
                    // Add cases for other MIME types
                    default:
                        // Default logic for unrecognized MIME types
                        break;
                }

            }
        }
        catch (UnauthorizedAccessException)
        {
            // Catch and ignore this exception if we don't have access to the directory
        }
    }

    public async Task<IEnumerable<LocalStorageDevice>> GetAllLocalStorageDevicesAsync()
    {
        
        _allStorageDeviceInfos ??= new List<LocalStorageDevice>(GetDriveInfo());

        await Task.CompletedTask;
        return _allStorageDeviceInfos;
    }
}
