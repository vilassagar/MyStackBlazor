namespace MyStackBlazor.Components.Chat;

public enum ChatRole { User, Assistant, System }

public record ChatAttachment(
    string Name,
    string? Url = null,
    string? ContentType = null,
    long? SizeBytes = null);
