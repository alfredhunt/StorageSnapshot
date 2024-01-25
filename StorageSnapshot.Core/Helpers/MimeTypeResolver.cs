using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageSnapshot.Core.Exceptions;

namespace StorageSnapshot.Core.Helpers;

public class MimeTypeResolver
{
    private static readonly Dictionary<string, string> MimeTypes = new()
    {
        { ".epub", "application/epub+zip" },
        { ".gz", "application/gzip" },
        { ".jar", "application/java-archive" },
        { ".json", "application/json" },
        { ".jsonld", "application/ld+json" },
        { ".doc", "application/msword" },
        { ".bin", "application/octet-stream" },
        { ".ogx", "application/ogg" },
        { ".pdf", "application/pdf" },
        { ".php", "application/php" },
        { ".rtf", "application/rtf" },
        { ".azw", "application/vnd.amazon.ebook" },
        { ".mpkg", "application/vnd.apple.installer+xml" },
        { ".xul", "application/vnd.mozilla.xul+xml" },
        { ".xls", "application/vnd.ms-excel" },
        { ".eot", "application/vnd.ms-fontobject" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".odp", "application/vnd.oasis.opendocument.presentation" },
        { ".ods", "application/vnd.oasis.opendocument.spreadsheet" },
        { ".odt", "application/vnd.oasis.opendocument.text" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".rar", "application/vnd.rar" },
        { ".vsd", "application/vnd.visio" },
        { ".7z", "application/x-7z-compressed" },
        { ".abw", "application/x-abiword" },
        { ".bz", "application/x-bzip" },
        { ".bz2", "application/x-bzip2" },
        { ".csh", "application/x-csh" },
        { ".arc", "application/x-freearc" },
        { ".sh", "application/x-sh" },
        { ".swf", "application/x-shockwave-flash" },
        { ".tar", "application/x-tar" },
        { ".xhtml", "application/xhtml+xml" },
        { ".xml", "application/xml" },
        { ".zip", "application/zip" },
        { ".3gp", "audio/3gpp" },
        { ".3g2", "audio/3gpp2" },
        { ".aac", "audio/aac" },
        { ".midi", "audio/midi" },
        { ".mp3", "audio/mpeg" },
        { ".oga", "audio/ogg" },
        { ".opus", "audio/opus" },
        { ".wav", "audio/wav" },
        { ".weba", "audio/webm" },
        { ".otf", "font/otf" },
        { ".ttf", "font/ttf" },
        { ".woff", "font/woff" },
        { ".woff2", "font/woff2" },
        { ".bmp", "image/bmp" },
        { ".gif", "image/gif" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".svg", "image/svg+xml" },
        { ".tiff", "image/tiff" },
        { ".ico", "image/vnd.microsoft.icon" },
        { ".webp", "image/webp" },
        { ".ics", "text/calendar" },
        { ".css", "text/css" },
        { ".csv", "text/csv" },
        { ".html", "text/html" },
        { ".js", "text/javascript" },
        { ".txt", "text/plain" },
        { ".mp4", "video/mp4" },
        { ".mpeg", "video/mpeg" },
        { ".ogv", "video/ogg" },
        { ".webm", "video/webm" },
        { ".avi", "video/x-msvideo" }
    };

    public static string GetMimeTypeOrDefault(FileInfo fileInfo)
    {
        return MimeTypes.TryGetValue(fileInfo.Extension, out var mimeType) ? mimeType : "application/octet-stream";
    }

    public static string GetMimeType(FileInfo fileInfo)
    {
        return MimeTypes.TryGetValue(fileInfo.Extension, out var mimeType) ? mimeType : throw new UnknownMimeTypeException(fileInfo);
    }

    public static bool TryGetMimeType(FileInfo fileInfo, out string mimeType)
    {
        return MimeTypes.TryGetValue(fileInfo.Extension, out mimeType);
    }
}
