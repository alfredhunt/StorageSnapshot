using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSnapshot.Core.Helpers;
public class FormatSizeInBytes
{
    public static string FormatByteSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        while (bytes >= 1024 && order < sizes.Length - 1)
        {
            order++;
            bytes /= 1024;
        }

        return String.Format("{0:0.##} {1}", bytes, sizes[order]);
    }

    public static string FormatByteSize(long bytes, int precision)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double adjustedSize = bytes;

        while (adjustedSize >= 1024 && order < sizes.Length - 1)
        {
            order++;
            adjustedSize /= 1024;
        }

        // Using the precision parameter in the format string
        string format = "{0:0." + new string('#', precision) + "} {1}";

        return String.Format(format, adjustedSize, sizes[order]);
    }

}
