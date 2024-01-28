using System.Data;
using System.Security.AccessControl;
using System.Security.Principal;

namespace StorageSnapshot.Core.Helpers;

public class PermissionChecker
{
    public static bool HasFolderAccess(string folderPath, FileSystemRights rights)
    {
        try
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            var directorySecurity = directoryInfo.GetAccessControl();

            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            foreach (FileSystemAccessRule rule in directorySecurity.GetAccessRules(true, true, typeof(NTAccount)))
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
            return false;
        }

        return false; // If no explicit allow or deny, the default is to deny.
    }

    private static readonly object _lock = new();

    public static void PrintAccessRules(string folderPath)
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
                foreach (FileSystemAccessRule rule in directorySecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
                {
                    System.Diagnostics.Debug.WriteLine($"Identity: {rule.IdentityReference.Value}");
                    System.Diagnostics.Debug.WriteLine($"Access Control Type: {rule.AccessControlType}");
                    System.Diagnostics.Debug.WriteLine($"File System Rights: {rule.FileSystemRights}");
                    System.Diagnostics.Debug.WriteLine("");
                }
            }
            catch (UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get access rules for {folderPath}. You might not have the necessary permissions.");
            }
        }
    }


}

