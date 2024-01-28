using System;
using System.Collections.Generic;
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
    private LocalStorageDevice device;

    [ObservableProperty]
    private LocalStorageDeviceDetailsViewModel? details = null;

    [ObservableProperty]
    private bool isLoading = false;

    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    public LocalStorageDeviceViewModel(ILocalStorageDeviceService localStorageDeviceService, LocalStorageDevice localStorageDevice)
    {
        _localStorageDeviceService = localStorageDeviceService;
        Device = localStorageDevice;
    }

    public async Task<LocalStorageDeviceAnalysis> LoadDetailsAsync()
    {
        IsLoading = true;
        var details = await _localStorageDeviceService.GetLocalStorageDeviceDetailsAsync(Device);
        Details = new LocalStorageDeviceDetailsViewModel(details);
        IsLoading = false;
        return details;
    }
}
