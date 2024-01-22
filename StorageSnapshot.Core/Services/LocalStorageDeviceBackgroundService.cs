using Microsoft.Extensions.Hosting;
using StorageSnapshot.Core.Contracts.Services;

public class LocalStorageDeviceBackgroundService : BackgroundService
{
    private readonly ILocalStorageDeviceService _localStorageDeviceService;

    public LocalStorageDeviceBackgroundService(ILocalStorageDeviceService localStorageDeviceService)
    {
        _localStorageDeviceService = localStorageDeviceService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var localStorageDevices = await _localStorageDeviceService.GetAllLocalStorageDevicesAsync();

        foreach (var localStorageDevice in localStorageDevices)
        {
            // This will cache the details for each device.
            await _localStorageDeviceService.GetLocalStorageDeviceDetailsAsync(localStorageDevice);
        }
    }
}
