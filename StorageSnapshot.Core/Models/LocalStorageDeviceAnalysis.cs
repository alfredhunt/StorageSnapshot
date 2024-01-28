using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSnapshot.Core.Models;

public class LocalStorageDeviceAnalysis
{
    public long TotalFiles
    {
        get;
        internal set;
    }
    public long TotalDirectories
    {
        get;
        internal set;
    }
    public Dictionary<string, MimeTypeDetails> MimeTypeDetailsDictionary { get; internal set; } = new Dictionary<string, MimeTypeDetails>();

    public static LocalStorageDeviceAnalysis operator +(LocalStorageDeviceAnalysis a, LocalStorageDeviceAnalysis b)
    {
        a.TotalFiles += b.TotalFiles;
        a.TotalDirectories += b.TotalDirectories;

        foreach (var bMimeTypeDetails in b.MimeTypeDetailsDictionary)
        {
            if (a.MimeTypeDetailsDictionary.ContainsKey(bMimeTypeDetails.Key))
            {

                a.MimeTypeDetailsDictionary[bMimeTypeDetails.Key] += bMimeTypeDetails.Value;
            }
            else
            {
                a.MimeTypeDetailsDictionary.Add(bMimeTypeDetails.Key, bMimeTypeDetails.Value);
            }
        }
        return a;
    }
}
