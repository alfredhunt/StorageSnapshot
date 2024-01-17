using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.Core.Contracts.Services;
public interface ILocalStorageDeviceService
{
    Task<IEnumerable<LocalStorageDevice>> GetAllLocalStorageDevicesAsync();
    Task GetLocalStorageDeviceDetailsAsync(LocalStorageDevice localStorageDevice);

}
