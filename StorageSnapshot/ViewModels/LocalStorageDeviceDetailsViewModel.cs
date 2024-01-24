using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.ViewModels;

public partial class LocalStorageDeviceDetailsViewModel : ObservableRecipient
{
    private LocalStorageDeviceDetails _details;

    [ObservableProperty]
    private long totalFiles;

    [ObservableProperty]
    private long totalDirectories;
    
    public IEnumerable<MimeTypeDetails> MimeTypeDetails => _details.MimeTypeDetailsDictionary.Values;

    public LocalStorageDeviceDetailsViewModel(LocalStorageDeviceDetails details)
    {
        _details = details;
        TotalFiles = details.TotalFiles;
        TotalDirectories = details.TotalDirectories;
    }
}