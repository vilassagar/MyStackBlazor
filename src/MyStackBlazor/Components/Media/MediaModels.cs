namespace MyStackBlazor.Components.Media;

/// <summary>A playable source. <see cref="Type"/> is the MIME type, e.g. "video/mp4" or "application/vnd.apple.mpegurl" for HLS.</summary>
public record VideoSource(
    string Src,
    string? Type = null,
    string? Label = null);

/// <summary>A text track (captions/subtitles/chapters) attached to a &lt;track&gt; element.</summary>
public record VideoTrack(
    string Src,
    string SrcLang,
    string Label,
    string Kind = "subtitles",
    bool Default = false);
