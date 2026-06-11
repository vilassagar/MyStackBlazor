namespace MyStackBlazor.Components.Data;

public class FileManagerItem
{
    public required string Name { get; set; }
    public bool IsDirectory { get; set; }
    public long SizeBytes { get; set; }
    public DateTime? Modified { get; set; }
    public List<FileManagerItem> Children { get; set; } = [];
}
