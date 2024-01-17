using System.ComponentModel;
using StorageSnapshot.Core.Contracts.Services;
using StorageSnapshot.Core.Models;

public class LocalStorageDeviceViewModel : INotifyPropertyChanged
{
    private ILocalStorageDeviceService _service;
    private LocalStorageDevice _device;
    private LocalStorageDeviceDetails _details;

    public LocalStorageDeviceViewModel(LocalStorageDevice device, ILocalStorageDeviceService service)
    {
        _device = device;
        _service = service;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void AnalyzeDevice()
    {
        //_details = _service.AnalyzeDevice(_device);
        // Update ViewModel properties based on _details...
    }

    // ViewModel properties, PropertyChanged event, etc...
}
