using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageSnapshot.Core.Helpers;

namespace StorageSnapshot.Core.Models;
public class LocalStorageDevice
{
    private DriveInfo _driveInfo = null;
    public LocalStorageDeviceDetails Details { get; private set; } = new LocalStorageDeviceDetails();
    public string Name => _driveInfo.Name;
    public DriveType DriveType => _driveInfo.DriveType;
    public bool IsReady => _driveInfo.IsReady;
    public string DriveFormat => _driveInfo.DriveFormat;
    public string VolumeLabel => _driveInfo.VolumeLabel;
    public long TotalSize => _driveInfo.TotalSize;
    public long TotalFreeSpace => _driveInfo.TotalFreeSpace;
    public long AvailableFreeSpace => _driveInfo.AvailableFreeSpace;
    public double PercentageInUse => ((TotalSize - TotalFreeSpace) / (double)TotalSize) * 100;
    public DirectoryInfo RootDirectory => _driveInfo.RootDirectory;
    public string VolumeLabelAndName => $"{VolumeLabel} ({Name})";
    public string TotalSizeFormatted => FormatSizeInBytes.FormatByteSize(TotalSize);
    public string TotalFreeSpaceFormatted => FormatSizeInBytes.FormatByteSize(TotalFreeSpace);
    public string AvailableFreeSpaceFormatted => FormatSizeInBytes.FormatByteSize(AvailableFreeSpace);
    public string TotalFreeSpaceOfTotalSize => $"{TotalFreeSpaceFormatted} free of {TotalSizeFormatted}";

    public int SymbolCode
    {
        get; set;
    }
    public string SymbolName
    {
        get; set;
    }
    public char Symbol => (char)SymbolCode;

    public LocalStorageDevice(DriveInfo driveInfo)
    {
        _driveInfo = driveInfo;
        // https://learn.microsoft.com/en-us/windows/apps/design/style/segoe-ui-symbol-font
        SymbolCode = 60834;
        SymbolName = "HardDisk";
    }

    public void ConsolePrint()
    {
        Console.WriteLine($"Drive {_driveInfo.Name}");
        Console.WriteLine($"  Drive type: {_driveInfo.DriveType}");
        if (_driveInfo.IsReady == true)
        {
            Console.WriteLine($"File system: {_driveInfo.DriveFormat}");
            Console.WriteLine($"Volume label: {_driveInfo.VolumeLabel}");
            Console.WriteLine($"Total size: {_driveInfo.TotalSize} bytes");
            Console.WriteLine($"Total free space: {_driveInfo.TotalFreeSpace} bytes");
            Console.WriteLine($"Available free space: {_driveInfo.AvailableFreeSpace} bytes");
            Console.WriteLine($"Root directory: {_driveInfo.RootDirectory}");
        }
    }
}
