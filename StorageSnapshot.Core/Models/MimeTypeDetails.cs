using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageSnapshot.Core.Helpers;

namespace StorageSnapshot.Core.Models;
public class MimeTypeDetails
{
    public string MimeType { get; set; }
    public string Extension { get; set; }
    public long TotalFiles { get; set; }
    public long TotalSize { get; set; }

    public string TotalSizeFormatted => FormatSizeInBytes.FormatByteSize(TotalSize);


    public MimeTypeDetails(string extension, string mimeType)
    {
        Extension = extension;
        MimeType = mimeType;
    }

    public static MimeTypeDetails operator +(MimeTypeDetails a, MimeTypeDetails b)
    {
        var mimeTypeDetails = new MimeTypeDetails(a.Extension, a.MimeType);
        mimeTypeDetails.TotalFiles = a.TotalFiles + b.TotalFiles;
        mimeTypeDetails.TotalSize = a.TotalSize + b.TotalSize;
        return mimeTypeDetails;
    }

}
