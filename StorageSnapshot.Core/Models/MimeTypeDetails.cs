using System;
using System.Collections.Generic;
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

}
