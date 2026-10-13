namespace MyStackBlazor.Components.Form.EditorTools;

/// <summary>A tool on the <see cref="StackEditor"/> toolbar (Telerik IEditorTool).</summary>
public interface IEditorTool { }

/// <summary>A toolbar button that runs a browser editing command.</summary>
public abstract class EditorCommandTool : IEditorTool
{
    /// <summary>document.execCommand name.</summary>
    internal abstract string Command { get; }
    internal virtual string? Argument => null;
    internal abstract string Title { get; }
    /// <summary>SVG path(s) for a 24×24 stroke icon, or null to show <see cref="Text"/>.</summary>
    internal virtual string? Icon => null;
    internal virtual string Text => Title;
    /// <summary>Key in the selection state that marks the button as pressed.</summary>
    internal virtual string? StateKey => null;
}

public sealed class Bold : EditorCommandTool
{
    internal override string Command => "bold";
    internal override string Title => "Bold";
    internal override string Text => "B";
    internal override string? StateKey => "bold";
}

public sealed class Italic : EditorCommandTool
{
    internal override string Command => "italic";
    internal override string Title => "Italic";
    internal override string Text => "I";
    internal override string? StateKey => "italic";
}

public sealed class Underline : EditorCommandTool
{
    internal override string Command => "underline";
    internal override string Title => "Underline";
    internal override string Text => "U";
    internal override string? StateKey => "underline";
}

public sealed class Strikethrough : EditorCommandTool
{
    internal override string Command => "strikeThrough";
    internal override string Title => "Strikethrough";
    internal override string Text => "S";
    internal override string? StateKey => "strikeThrough";
}

public sealed class Subscript : EditorCommandTool
{
    internal override string Command => "subscript";
    internal override string Title => "Subscript";
    internal override string Text => "x₂";
    internal override string? StateKey => "subscript";
}

public sealed class Superscript : EditorCommandTool
{
    internal override string Command => "superscript";
    internal override string Title => "Superscript";
    internal override string Text => "x²";
    internal override string? StateKey => "superscript";
}

public sealed class AlignLeft : EditorCommandTool
{
    internal override string Command => "justifyLeft";
    internal override string Title => "Align left";
    internal override string? Icon => "M4 6h16M4 12h10M4 18h14";
    internal override string? StateKey => "justifyLeft";
}

public sealed class AlignCenter : EditorCommandTool
{
    internal override string Command => "justifyCenter";
    internal override string Title => "Align center";
    internal override string? Icon => "M4 6h16M7 12h10M5 18h14";
    internal override string? StateKey => "justifyCenter";
}

public sealed class AlignRight : EditorCommandTool
{
    internal override string Command => "justifyRight";
    internal override string Title => "Align right";
    internal override string? Icon => "M4 6h16M10 12h10M6 18h14";
    internal override string? StateKey => "justifyRight";
}

public sealed class AlignJustify : EditorCommandTool
{
    internal override string Command => "justifyFull";
    internal override string Title => "Justify";
    internal override string? Icon => "M4 6h16M4 12h16M4 18h16";
    internal override string? StateKey => "justifyFull";
}

public sealed class BulletedList : EditorCommandTool
{
    internal override string Command => "insertUnorderedList";
    internal override string Title => "Bulleted list";
    internal override string? Icon => "M9 6h11M9 12h11M9 18h11M4.5 6h.01M4.5 12h.01M4.5 18h.01";
    internal override string? StateKey => "insertUnorderedList";
}

public sealed class NumberedList : EditorCommandTool
{
    internal override string Command => "insertOrderedList";
    internal override string Title => "Numbered list";
    internal override string? Icon => "M10 6h10M10 12h10M10 18h10M4 4.5h1.5v4M4 8.5h3M4 14.5c.5-.7 1.2-1 1.9-.8.8.2 1.1 1 .6 1.7L4 18.5h3";
    internal override string? StateKey => "insertOrderedList";
}

public sealed class Indent : EditorCommandTool
{
    internal override string Command => "indent";
    internal override string Title => "Indent";
    internal override string? Icon => "M10 6h10M10 12h10M10 18h10M3 9l3 3-3 3";
}

public sealed class Outdent : EditorCommandTool
{
    internal override string Command => "outdent";
    internal override string Title => "Outdent";
    internal override string? Icon => "M10 6h10M10 12h10M10 18h10M6 9l-3 3 3 3";
}

public sealed class Unlink : EditorCommandTool
{
    internal override string Command => "unlink";
    internal override string Title => "Remove link";
    internal override string? Icon => "M9 15l6-6M7 17a4 4 0 010-5.66l1.4-1.4M17 7a4 4 0 010 5.66l-1.4 1.4M3 3l18 18";
}

public sealed class HorizontalRule : EditorCommandTool
{
    internal override string Command => "insertHorizontalRule";
    internal override string Title => "Horizontal line";
    internal override string? Icon => "M4 12h16";
}

public sealed class Undo : EditorCommandTool
{
    internal override string Command => "undo";
    internal override string Title => "Undo";
    internal override string? Icon => "M9 14L4 9l5-5M4 9h10a6 6 0 010 12h-3";
}

public sealed class Redo : EditorCommandTool
{
    internal override string Command => "redo";
    internal override string Title => "Redo";
    internal override string? Icon => "M15 14l5-5-5-5M20 9H10a6 6 0 000 12h3";
}

public sealed class CleanFormatting : EditorCommandTool
{
    internal override string Command => "removeFormat";
    internal override string Title => "Clear formatting";
    internal override string? Icon => "M6 4h12M10 4l-3 16M14 4l-1.5 7M4 20l16-16";
}

/// <summary>Opens a dialog to insert or edit a link.</summary>
public sealed class CreateLink : IEditorTool { }

/// <summary>Opens a dialog to insert an image by URL.</summary>
public sealed class InsertImage : IEditorTool { }

/// <summary>Opens a dialog to insert a table.</summary>
public sealed class InsertTable : IEditorTool { }

/// <summary>Switches between the formatted view and the HTML source.</summary>
public sealed class ViewHtml : IEditorTool { }

/// <summary>Block format drop-down (paragraph, headings, quote, code).</summary>
public sealed class Format : IEditorTool { }

/// <summary>Font family drop-down.</summary>
public sealed class FontFamily : IEditorTool
{
    public IReadOnlyList<string> Fonts { get; init; } =
        ["Arial", "Georgia", "Tahoma", "Times New Roman", "Trebuchet MS", "Verdana", "Courier New"];
}

/// <summary>Font size drop-down (browser sizes 1–7).</summary>
public sealed class FontSize : IEditorTool { }

/// <summary>Text colour picker.</summary>
public sealed class ForeColor : IEditorTool { }

/// <summary>Highlight (background) colour picker.</summary>
public sealed class BackgroundColor : IEditorTool { }

/// <summary>Groups tools visually on the toolbar.</summary>
public sealed class EditorButtonGroup(params IEditorTool[] tools) : IEditorTool
{
    public IReadOnlyList<IEditorTool> Tools { get; } = tools;
}

/// <summary>Places a <c>StackEditorCustomTool</c> with the same <see cref="Name"/> on the toolbar.</summary>
public sealed class CustomTool(string name) : IEditorTool
{
    public string Name { get; } = name;
}

/// <summary>Ready-made tool lists (Telerik EditorToolSets).</summary>
public static class EditorToolSets
{
    public static List<IEditorTool> Default =>
    [
        new EditorButtonGroup(new Bold(), new Italic(), new Underline()),
        new EditorButtonGroup(new Undo(), new Redo()),
        new Format(),
        new EditorButtonGroup(new AlignLeft(), new AlignCenter(), new AlignRight(), new AlignJustify()),
        new EditorButtonGroup(new BulletedList(), new NumberedList(), new Indent(), new Outdent()),
        new EditorButtonGroup(new CreateLink(), new Unlink(), new InsertImage(), new InsertTable()),
    ];

    public static List<IEditorTool> All =>
    [
        new EditorButtonGroup(new Bold(), new Italic(), new Underline(), new Strikethrough(), new Subscript(), new Superscript()),
        new EditorButtonGroup(new Undo(), new Redo()),
        new Format(),
        new FontFamily(),
        new FontSize(),
        new EditorButtonGroup(new ForeColor(), new BackgroundColor()),
        new EditorButtonGroup(new AlignLeft(), new AlignCenter(), new AlignRight(), new AlignJustify()),
        new EditorButtonGroup(new BulletedList(), new NumberedList(), new Indent(), new Outdent()),
        new EditorButtonGroup(new CreateLink(), new Unlink(), new InsertImage(), new InsertTable(), new HorizontalRule()),
        new EditorButtonGroup(new CleanFormatting(), new ViewHtml()),
    ];
}
