namespace MyStackBlazor.Components.Form;

/// <summary>A file picked in <see cref="StackFileSelect"/> (Telerik FileSelectFileInfo).</summary>
public class FileSelectFileInfo
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public required string Name { get; init; }
    public string Extension => Path.GetExtension(Name);
    public long Size { get; init; }
    public string ContentType { get; init; } = "";
    public bool InvalidExtension { get; init; }
    public bool InvalidMinFileSize { get; init; }
    public bool InvalidMaxFileSize { get; init; }
    public bool IsValid => !InvalidExtension && !InvalidMinFileSize && !InvalidMaxFileSize;

    internal Func<Stream>? OpenStream { get; init; }

    /// <summary>Reads the file content. Valid until the next selection; dispose it when done.</summary>
    public Stream Stream => OpenStream?.Invoke() ?? Stream.Null;
}

/// <summary>Arguments for <c>StackFileSelect.OnSelect</c> / <c>OnRemove</c>. Set <see cref="IsCancelled"/> to reject the files.</summary>
public class FileSelectEventArgs
{
    public required List<FileSelectFileInfo> Files { get; init; }
    public bool IsCancelled { get; set; }
}

/// <summary>Upload state of a file in <see cref="StackUpload"/>.</summary>
public enum UploadFileStatus
{
    Selected,
    Uploading,
    Uploaded,
    Failed,
    Cancelled,
}

/// <summary>Which request a success/error event belongs to.</summary>
public enum UploadOperationType
{
    Upload,
    Remove,
}

/// <summary>A file in <see cref="StackUpload"/> (Telerik UploadFileInfo).</summary>
public class UploadFileInfo
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public required string Name { get; init; }
    public string Extension => Path.GetExtension(Name);
    public long Size { get; init; }
    public string ContentType { get; init; } = "";
    public bool InvalidExtension { get; init; }
    public bool InvalidMinFileSize { get; init; }
    public bool InvalidMaxFileSize { get; init; }
    public bool IsValid => !InvalidExtension && !InvalidMinFileSize && !InvalidMaxFileSize;
    public int Progress { get; set; }
    public UploadFileStatus Status { get; set; } = UploadFileStatus.Selected;
}

/// <summary>The server's answer to an upload or remove request.</summary>
public class UploadHttpRequest
{
    public int Status { get; init; }
    public string StatusText { get; init; } = "";
    public string ResponseText { get; init; } = "";
}

/// <summary>Arguments for <c>StackUpload.OnSelect</c>, <c>OnRemove</c> and <c>OnCancel</c>.</summary>
public class UploadSelectEventArgs
{
    public required List<UploadFileInfo> Files { get; init; }
    public bool IsCancelled { get; set; }
}

/// <summary>
/// Arguments for <c>StackUpload.OnUpload</c> (and the remove request): add headers or form fields, or cancel.
/// </summary>
public class UploadEventArgs
{
    public required List<UploadFileInfo> Files { get; init; }
    public Dictionary<string, string> RequestHeaders { get; } = [];
    public Dictionary<string, string> RequestData { get; } = [];
    public bool IsCancelled { get; set; }
}

public class UploadProgressEventArgs
{
    public required List<UploadFileInfo> Files { get; init; }
    public int Progress { get; init; }
}

public class UploadSuccessEventArgs
{
    public required List<UploadFileInfo> Files { get; init; }
    public UploadOperationType Operation { get; init; }
    public required UploadHttpRequest Request { get; init; }
}

public class UploadErrorEventArgs
{
    public required List<UploadFileInfo> Files { get; init; }
    public UploadOperationType Operation { get; init; }
    public required UploadHttpRequest Request { get; init; }
}

/// <summary>Shared validation for FileSelect and Upload.</summary>
internal static class FileRules
{
    public static (bool Extension, bool Min, bool Max) Check(string name, long size, IEnumerable<string>? allowedExtensions, long? minSize, long? maxSize)
    {
        var extension = Path.GetExtension(name);
        var badExtension = allowedExtensions is not null && allowedExtensions.Any()
                           && !allowedExtensions.Any(e => string.Equals(Normalize(e), extension, StringComparison.OrdinalIgnoreCase));
        return (badExtension, minSize is long min && size < min, maxSize is long max && size > max);
    }

    private static string Normalize(string extension) => extension.StartsWith('.') ? extension : "." + extension;

    public static string Message(bool extension, bool min, bool max) =>
        extension ? "File type not allowed." : min ? "File is too small." : max ? "File is too large." : "";

    public static string FormatSize(long bytes) => bytes switch
    {
        < 1024        => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _             => $"{bytes / (1024.0 * 1024):F1} MB",
    };
}
