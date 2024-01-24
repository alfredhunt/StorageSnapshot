using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageSnapshot.Core.Models;

namespace StorageSnapshot.Core.Contracts.Services;
public interface ILocalStorageDeviceService
{
    Task Initialize();
    Task<IEnumerable<LocalStorageDevice>> GetLocalStorageDevicesAsync();
    Task<LocalStorageDeviceDetails> GetLocalStorageDeviceDetailsAsync(LocalStorageDevice localStorageDevice1);

}
