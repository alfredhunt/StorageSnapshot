using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSnapshot.Core.Helpers;

public class MimeTypeResolver
{
    private static readonly Dictionary<string, string> MimeTypes = new()
    {
        { ".jpg", "image/jpeg" },
        { ".png", "image/png" },
        // Add other MIME types and extensions here
    };

    public static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return MimeTypes.TryGetValue(extension, out var mimeType) ? mimeType : "application/octet-stream";
    }
}
