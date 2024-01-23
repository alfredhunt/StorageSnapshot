using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.ViewModels;
public partial class LocalStorageDeviceViewModel : ObservableRecipient
{
    [ObservableProperty]
    private LocalStorageDevice? device;

    [ObservableProperty]
    private LocalStorageDeviceDetails? details;

    public LocalStorageDeviceViewModel(LocalStorageDevice item)
    {
        Device = item;
    }
}
