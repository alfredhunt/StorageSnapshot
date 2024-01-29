using System.Data;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using StorageSnapshot.Core.Contracts.Services;

namespace StorageSnapshot.Services;

public class AccessControlService : IAccessControlService
{
    private static readonly object _lock = new();

    public bool IsSystemFolder(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        return IsSystemFolder(directoryInfo);
    }

    public bool IsSystemFolder(DirectoryInfo directoryInfo)
    {
        return (directoryInfo.Attributes & FileAttributes.System) == FileAttributes.System
            || (directoryInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
    }

    public bool CanAccessDirectory(string path)
    {
        try
        {
            // Attempt to get the list of files in the directory
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            // Access is denied
            return false;
        }
        catch (Exception)
        {
            // Other exceptions can be handled separately
            return false;
        }
    }
    public bool CanAccessDirectory(DirectoryInfo directoryInfo)
    {
        return CanAccessDirectory(directoryInfo.FullName);
    }

    public bool CanAccessDirectory(DriveInfo driveInfo)
    {
        return CanAccessDirectory(driveInfo.RootDirectory);
    }


    public bool CanListDirectory(DriveInfo driveInfo)
    {
        return CanListDirectory(driveInfo.RootDirectory);
    }

    public bool CanListDirectory(DirectoryInfo directoryInfo)
    {
        return HasFolderAccess(directoryInfo, FileSystemRights.ListDirectory);
    }


    public bool HasReadAccess(DriveInfo path)
    {
        return HasReadAccess(path.RootDirectory);
    }

    public bool HasReadAccess(DirectoryInfo directoryInfo)
    {
        return HasFolderAccess(directoryInfo, FileSystemRights.Read);
    }

    public bool HasReadAccess(FileInfo fileInfo)
    {
        try
        {
            var accessControl = fileInfo.GetAccessControl();
            return HasFileSystemRights(accessControl, FileSystemRights.Read, fileInfo.FullName);
        }
        catch (UnauthorizedAccessException)
        {
            System.Diagnostics.Debug.WriteLine("You do not have permission to view this file's access control list.");
        }
        catch (IOException)
        {
            System.Diagnostics.Debug.WriteLine("The file is in use and cannot be accessed.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
        PrintAccessRules(fileInfo.FullName);
        return false;
    }

    public static bool HasFileSystemRights(FileSystemSecurity fileSystemSecurity, FileSystemRights rights, string path)
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            foreach (FileSystemAccessRule rule in fileSystemSecurity.GetAccessRules(true, true, typeof(NTAccount)))
            {
                if ((rights & rule.FileSystemRights) != rights) continue;

                if (rule.IdentityReference.Value == identity.Name || principal.IsInRole(rule.IdentityReference.Value))
                {
                    if (rule.AccessControlType == AccessControlType.Deny)
                        return false;
                    if (rule.AccessControlType == AccessControlType.Allow)
                        return true;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // The user does not have the necessary permissions.
            System.Diagnostics.Debug.WriteLine($"User does not have rights:{rights} to {path}");
            return false;
        }

        return false; // If no explicit allow or deny, the default is to deny.
    }

    public static bool HasFolderAccess(DirectoryInfo directoryInfo, FileSystemRights rights)
    {
        try
        {
            var directorySecurity = directoryInfo.GetAccessControl();

            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            foreach (FileSystemAccessRule rule in directorySecurity.GetAccessRules(true, true, typeof(NTAccount)))
            {
                if ((rights & rule.FileSystemRights) != rights) continue;

                if (rule.IdentityReference.Value == identity.Name || principal.IsInRole(rule.IdentityReference.Value))
                {
                    if (rule.AccessControlType == AccessControlType.Deny)
                    {
                        System.Diagnostics.Debug.WriteLine($"User does not have rights:{rule.FileSystemRights} to {directoryInfo.FullName}");
                        return false;
                    }
                    if (rule.AccessControlType == AccessControlType.Allow)
                        return true;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // The user does not have the necessary permissions.
            System.Diagnostics.Debug.WriteLine($"User does not have rights:{rights} to {directoryInfo.FullName}");
            return false;
        }

        return false; // If no explicit allow or deny, the default is to deny.
    }



    public void PrintAccessRules(string folderPath)
    {
        lock (_lock)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine($"Folder: {folderPath}");
                var directoryInfo = new DirectoryInfo(folderPath);
                var directorySecurity = directoryInfo.GetAccessControl();
                foreach (FileSystemAccessRule rule in directorySecurity.GetAccessRules(true, true, typeof(NTAccount)))
                {
                    System.Diagnostics.Debug.WriteLine($"Identity: {rule.IdentityReference.Value}");
                    System.Diagnostics.Debug.WriteLine($"Access Control Type: {rule.AccessControlType}");
                    System.Diagnostics.Debug.WriteLine($"File System Rights: {rule.FileSystemRights}");
                    System.Diagnostics.Debug.WriteLine("");
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get access rules for {folderPath}. You might not have the necessary permissions.");
            }
        }
    }


}

