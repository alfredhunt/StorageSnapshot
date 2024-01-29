using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSnapshot.Core.Contracts.Services;
public interface IAccessControlService
{
    bool IsSystemFolder(string path);
    bool IsSystemFolder(DirectoryInfo directoryInfo);


    bool CanAccessDirectory(string path);
    bool CanAccessDirectory(DirectoryInfo directoryInfo);
    bool CanAccessDirectory(DriveInfo driveInfo);

    bool HasReadAccess(DriveInfo path);

    bool HasReadAccess(DirectoryInfo directoryInfo);

    bool HasReadAccess(FileInfo fileInfo);

    void PrintAccessRules(string folderPath);





    bool CanListDirectory(DriveInfo driveInfo);

    bool CanListDirectory(DirectoryInfo directoryInfo);
}
