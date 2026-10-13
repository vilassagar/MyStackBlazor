using MyStackBlazor.Components.Form;
using MyStackBlazor.Components.Form.EditorTools;

namespace MyStackBlazor.UnitTests.Components;

public class EditorTests : TestContext
{
    public EditorTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private static List<object?> ExecArgs(BunitJSInterop js) =>
        js.Invocations.Where(i => i.Identifier == "MyStackBlazor.editorExec").Select(i => i.Arguments[1] + "|" + i.Arguments[2]).Cast<object?>().ToList();

    [Fact]
    public void Initializes_With_Value_And_Default_Tools()
    {
        var cut = RenderComponent<StackEditor>(p => p.Add(e => e.Value, "<p>Hello</p>").Add(e => e.Placeholder, "Write…"));
        var init = JSInterop.VerifyInvoke("MyStackBlazor.editorInit");
        init.Arguments[2].Should().Be("<p>Hello</p>");
        cut.Find("[role=textbox]").GetAttribute("contenteditable").Should().Be("true");
        cut.Find("[role=textbox]").GetAttribute("data-placeholder").Should().Be("Write…");
        cut.Find("button[aria-label=Bold]").Should().NotBeNull();
        cut.Find("select[aria-label=Format]").Should().NotBeNull();
        cut.FindAll("select[aria-label='Font size']").Should().BeEmpty("not in the Default tool set");
    }

    [Fact]
    public void All_Tool_Set_And_Custom_List()
    {
        var all = RenderComponent<StackEditor>(p => p.Add(e => e.Tools, EditorToolSets.All));
        all.Find("select[aria-label='Font size']").Should().NotBeNull();
        all.Find("button[aria-label='View HTML']").Should().NotBeNull();

        var custom = RenderComponent<StackEditor>(p => p.Add(e => e.Tools, [new Bold(), new Italic()]));
        custom.FindAll("[role=toolbar] button").Select(b => b.GetAttribute("aria-label")).Should().Equal("Bold", "Italic");
    }

    [Fact]
    public async Task Input_From_Js_Updates_Value()
    {
        string? value = null;
        var cut = RenderComponent<StackEditor>(p => p.Add(e => e.ValueChanged, v => value = v));
        await cut.InvokeAsync(() => cut.Instance.OnEditorInput("<p><b>Hi</b></p>"));
        value.Should().Be("<p><b>Hi</b></p>");
    }

    [Fact]
    public async Task Selection_State_Marks_Buttons_And_Format()
    {
        var cut = RenderComponent<StackEditor>();
        await cut.InvokeAsync(() => cut.Instance.OnEditorState(new Dictionary<string, object?> { ["bold"] = true, ["italic"] = false, ["formatBlock"] = "h2" }));
        cut.Find("button[aria-label=Bold]").GetAttribute("aria-pressed").Should().Be("true");
        cut.Find("button[aria-label=Italic]").GetAttribute("aria-pressed").Should().Be("false");
        cut.Find("select[aria-label=Format]").GetAttribute("value").Should().Be("h2");
    }

    [Fact]
    public async Task Toolbar_Runs_Commands()
    {
        var cut = RenderComponent<StackEditor>(p => p.Add(e => e.Tools, EditorToolSets.All));
        cut.Find("button[aria-label=Bold]").Click();
        await cut.Find("select[aria-label=Format]").ChangeAsync(new ChangeEventArgs { Value = "h3" });
        await cut.Find("select[aria-label='Font size']").ChangeAsync(new ChangeEventArgs { Value = "5" });
        ExecArgs(JSInterop).Should().Equal("bold|", "formatBlock|<h3>", "fontSize|5");
    }

    [Fact]
    public async Task Link_Dialog_Inserts_Encoded_Anchor()
    {
        JSInterop.Setup<string>("MyStackBlazor.editorSaveSelection", _ => true).SetResult("selected");
        var cut = RenderComponent<StackEditor>();
        cut.Find("button[aria-label='Insert link']").Click();
        cut.Find("[role=dialog] input").Change("https://example.com/?a=1&b=\"2\"");
        cut.Find("[role=dialog] input[type=checkbox]").Change(true);
        cut.FindAll("[role=dialog] button").First(b => b.TextContent == "Insert").Click();

        var html = (string)JSInterop.Invocations.Last(i => i.Identifier == "MyStackBlazor.editorExec").Arguments[2]!;
        html.Should().Be("<a href=\"https://example.com/?a=1&amp;b=&quot;2&quot;\" target=\"_blank\" rel=\"noopener noreferrer\">selected</a>");
        await Task.CompletedTask;
    }

    [Fact]
    public void Table_Dialog_Inserts_Table()
    {
        var cut = RenderComponent<StackEditor>();
        cut.Find("button[aria-label='Insert table']").Click();
        cut.FindAll("[role=dialog] input")[0].Change(3);
        cut.FindAll("[role=dialog] input")[1].Change(2);
        cut.FindAll("[role=dialog] button").First(b => b.TextContent == "Insert").Click();
        var html = (string)JSInterop.Invocations.Last(i => i.Identifier == "MyStackBlazor.editorExec").Arguments[2]!;
        html.Split("<tr>").Length.Should().Be(4, "3 rows");
        html.Split("<td>").Length.Should().Be(7, "6 cells");
    }

    [Fact]
    public void View_Html_Edits_Source()
    {
        string? value = "<p>a</p>";
        var cut = RenderComponent<StackEditor>(p => p
            .Add(e => e.Value, "<p>a</p>")
            .Add(e => e.Tools, [new ViewHtml()])
            .Add(e => e.ValueChanged, v => value = v));
        cut.Find("button[aria-label='View HTML']").Click();
        cut.Find("textarea[aria-label='HTML source']").Input("<h1>b</h1>");
        cut.Find("button[aria-label='View HTML']").Click();
        value.Should().Be("<h1>b</h1>");
        JSInterop.VerifyInvoke("MyStackBlazor.editorSetHtml");
    }

    [Fact]
    public void ReadOnly_And_Custom_Tool()
    {
        var cut = RenderComponent<StackEditor>(p => p
            .Add(e => e.ReadOnly, true)
            .Add(e => e.Tools, [new Bold(), new CustomTool("Stamp")])
            .Add(e => e.EditorCustomTools, b =>
            {
                b.OpenComponent<StackEditorCustomTool>(0);
                b.AddAttribute(1, nameof(StackEditorCustomTool.Name), "Stamp");
                b.AddAttribute(2, nameof(StackEditorCustomTool.ChildContent), (RenderFragment)(c => c.AddMarkupContent(0, "<button class='stamp'>Stamp</button>")));
                b.CloseComponent();
            }));
        cut.Find("[role=textbox]").GetAttribute("contenteditable").Should().Be("false");
        cut.Find("button[aria-label=Bold]").HasAttribute("disabled").Should().BeTrue();
        cut.Find(".stamp").Should().NotBeNull();
    }
}
