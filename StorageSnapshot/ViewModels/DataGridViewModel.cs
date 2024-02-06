using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Helpers;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Messages;

namespace StorageSnapshot.ViewModels;

public partial class DataGridViewModel : ObservableRecipient, INavigationAware,
    IRecipient<UsbDeviceAddedMessage>, IRecipient<UsbDeviceRemovedMessage>
{
    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    private readonly Dictionary<string, MimeTypeDetails> _mimeTypeDetailsDictionary = new();
    public ObservableCollection<MimeTypeDetails> MimeTypeDetails { get; } = new ObservableCollection<MimeTypeDetails>();

    public ObservableCollection<LocalStorageDeviceViewModel> LocalStorageDevices { get; } = new ObservableCollection<LocalStorageDeviceViewModel>();

    private readonly DispatcherQueue dispatcherQueue;

    [ObservableProperty]
    private long totalSize;
    [ObservableProperty]
    private long totalFreeSpace;
    [ObservableProperty]
    private long availableFreeSpace;
    [ObservableProperty]
    private long totalFiles;
    [ObservableProperty]
    private long totalDirectories;
    
    [ObservableProperty]
    private bool isLoading = false;
    [ObservableProperty]
    private double percentageInUse = 50;
    [ObservableProperty]
    private string? totalSizeFormatted;
    [ObservableProperty]
    private string? totalFreeSpaceFormatted;
    [ObservableProperty]
    private string? availableFreeSpaceFormatted;
    [ObservableProperty]
    private string? totalFreeSpaceOfTotalSize;

    public DataGridViewModel(ILocalStorageDeviceService localStorageDeviceService)
    {
        _localStorageDeviceService = localStorageDeviceService;

        dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        WeakReferenceMessenger.Default.Register<UsbDeviceAddedMessage>(this);
        WeakReferenceMessenger.Default.Register<UsbDeviceRemovedMessage>(this);
    }

    public async void OnNavigatedTo(object parameter)
    {
        List<Task> tasks = new List<Task>();
        LocalStorageDevices.Clear();
        var data = await _localStorageDeviceService.GetLocalStorageDevicesAsync();
        CreateSummaryInfo(data);
        foreach (var localStorageDevice in data)
        {
            var vm = new LocalStorageDeviceViewModel(_localStorageDeviceService, localStorageDevice);
            LocalStorageDevices.Add(vm);
            tasks.Add(vm.LoadDetailsAsync());
        }
        IsLoading = true;
        await Task.WhenAll(tasks).ContinueWith(CombineDeviceDetails, TaskScheduler.Current);
        IsLoading = false;
    }

    private void CreateSummaryInfo(IEnumerable<LocalStorageDevice> devices)
    {
        TotalSize = devices.Sum(selector: x => x.TotalSize);
        TotalFreeSpace = devices.Sum(selector: x => x.TotalFreeSpace);
        AvailableFreeSpace = devices.Sum(selector: x => x.AvailableFreeSpace);
        PercentageInUse = ((TotalSize - TotalFreeSpace) / (double)TotalSize) * 100;

        TotalSizeFormatted = FormatSizeInBytes.FormatByteSize(TotalSize, 2);
        TotalFreeSpaceFormatted = FormatSizeInBytes.FormatByteSize(TotalFreeSpace, 2);
        AvailableFreeSpaceFormatted = FormatSizeInBytes.FormatByteSize(AvailableFreeSpace, 2);

        TotalFreeSpaceOfTotalSize = $"{TotalFreeSpaceFormatted} free of {TotalSizeFormatted}";
    }

    private void CombineDeviceDetails(Task task)
    {
        

        _mimeTypeDetailsDictionary.Clear();
        var details = LocalStorageDevices.Select(selector: x => x.Details);
        foreach (var detail in details)
        {
            if (detail == null) continue;

            TotalFiles += detail.TotalFiles;
            TotalDirectories += detail.TotalDirectories;

            foreach (var mimeType in detail.MimeTypeDetails)
            {
                if (!_mimeTypeDetailsDictionary.ContainsKey(mimeType.MimeType))
                {
                    _mimeTypeDetailsDictionary.Add(mimeType.MimeType, mimeType);
                    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        MimeTypeDetails.Add(mimeType);
                    });
                }
                else
                {
                    _mimeTypeDetailsDictionary[mimeType.MimeType] += mimeType;
                }
            }
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void Receive(UsbDeviceAddedMessage message)
    {
        dispatcherQueue.TryEnqueue(async () => {
            DriveInfo driveInfo = new(message.DeviceId);
            LocalStorageDevice localStorageDevice = new(driveInfo);
            var vm = new LocalStorageDeviceViewModel(_localStorageDeviceService, localStorageDevice);

            LocalStorageDevices.Add(vm);

            await vm.LoadDetailsAsync();
        });
    }

    public void Receive(UsbDeviceRemovedMessage message)
    {
        dispatcherQueue.TryEnqueue(() => {
            DriveInfo driveInfo = new(message.DeviceId);

            var vm = LocalStorageDevices.FirstOrDefault(x => x.Device.Name == driveInfo.Name);

            if (vm != null)
                LocalStorageDevices.Remove(vm);
        });
    }
}
