
namespace StorageSnapshot.Core.Exceptions;

public class UnknownMimeTypeException : Exception
{
    public UnknownMimeTypeException(FileInfo fileInfo)
        : base($"The file extension '{fileInfo.Extension}' is not a known MIME type.")
    {
    }
}
