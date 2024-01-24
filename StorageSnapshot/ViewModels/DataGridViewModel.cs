using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;

using StorageSnapshot.Contracts.ViewModels;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Core.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StorageSnapshot.ViewModels;

public partial class DataGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly ILocalStorageDeviceService _localStorageDeviceService;
    private readonly object lockObject = new object();

    public ObservableCollection<LocalStorageDeviceViewModel> LocalStorageDevices { get; } = new ObservableCollection<LocalStorageDeviceViewModel>();
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
    public ObservableCollection<MimeTypeDetails> MimeTypeDetails { get; } = new ObservableCollection<MimeTypeDetails>();

    [ObservableProperty]
    private bool isLoading = false;

    private Dictionary<string, MimeTypeDetails> _mimeTypeDetailsDictionary = new Dictionary<string, MimeTypeDetails>();


    public DataGridViewModel(ILocalStorageDeviceService localStorageDeviceService)
    {
        _localStorageDeviceService = localStorageDeviceService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        List<Task> tasks = new List<Task>();
        LocalStorageDevices.Clear();
        var data = await _localStorageDeviceService.GetLocalStorageDevicesAsync();
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

    private void CombineDeviceDetails(Task task)
    {
        var devices = LocalStorageDevices.Select(selector: x => x.Device);
        TotalSize = devices.Sum(selector: x => x.TotalSize);
        TotalFreeSpace = devices.Sum(selector: x => x.TotalFreeSpace);
        AvailableFreeSpace = devices.Sum(selector: x => x.AvailableFreeSpace);

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
}
