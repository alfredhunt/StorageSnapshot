using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;
using StorageSnapshot.Core.Services;

namespace StorageSnapshot.ViewModels;
public partial class LocalStorageDeviceViewModel : ObservableRecipient
{
    [ObservableProperty]
    private LocalStorageDevice? device = null;

    [ObservableProperty]
    private LocalStorageDeviceDetailsViewModel? details = null;

    [ObservableProperty]
    private bool isLoading = false;

    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    public LocalStorageDeviceViewModel(ILocalStorageDeviceService localStorageDeviceService, LocalStorageDevice localStorageDevice)
    {
        _localStorageDeviceService = localStorageDeviceService;
        Device = localStorageDevice;
        LoadDetailsAsync();
    }

    public async void LoadDetailsAsync()
    {
        IsLoading = true;
        var details = await _localStorageDeviceService.GetLocalStorageDeviceDetailsAsync(Device);
        Details = new LocalStorageDeviceDetailsViewModel(details);
        IsLoading = false;
    }
}

public partial class LocalStorageDeviceDetailsViewModel : ObservableRecipient
{
    [ObservableProperty]
    private int totalFiles;

    [ObservableProperty]
    private int totalDirectories;

    [ObservableProperty]
    private ObservableCollection<MimeTypeDetails>? mimeTypeDetails = null;

    public LocalStorageDeviceDetailsViewModel(LocalStorageDeviceDetails item)
    {
        TotalFiles = item.TotalFiles;
        TotalDirectories = item.TotalDirectories;
        MimeTypeDetails = new ObservableCollection<MimeTypeDetails>(item.MimeTypeDetailsDictionary.Values);
    }
}