using Microsoft.AspNetCore.Components.Forms;
using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class SignatureTests : TestContext
{
    public SignatureTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Initializes_Canvas_With_Options()
    {
        var cut = RenderComponent<StackSignature>(p => p
            .Add(s => s.Color, "#123456")
            .Add(s => s.StrokeWidth, 3)
            .Add(s => s.Smooth, true)
            .Add(s => s.Width, "400px")
            .Add(s => s.Height, "150px"));

        cut.Find("canvas").Should().NotBeNull();
        cut.Find("div[style]").GetAttribute("style").Should().Be("width:400px;height:150px");
        var init = JSInterop.VerifyInvoke("MyStackBlazor.signatureInit");
        init.Arguments.Should().HaveCount(3);
    }

    [Fact]
    public async Task Stroke_From_Js_Updates_Value_And_Clear_Resets()
    {
        string? value = null;
        object? changed = null;
        var cut = RenderComponent<StackSignature>(p => p
            .Add(s => s.ValueChanged, v => value = v)
            .Add(s => s.OnChange, v => changed = v));

        await cut.InvokeAsync(() => cut.Instance.OnSignatureChanged("data:image/png;base64,AAA"));
        value.Should().Be("data:image/png;base64,AAA");
        changed.Should().Be("data:image/png;base64,AAA");

        cut.Find("button[aria-label='Clear signature']").Click();
        value.Should().BeNull();
    }

    [Fact]
    public async Task ReadOnly_Ignores_Strokes_And_Hides_Clear()
    {
        string? value = "data:x";
        var cut = RenderComponent<StackSignature>(p => p
            .Add(s => s.Value, "data:x")
            .Add(s => s.ReadOnly, true)
            .Add(s => s.ValueChanged, v => value = v));
        await cut.InvokeAsync(() => cut.Instance.OnSignatureChanged("data:new"));
        value.Should().Be("data:x");
        cut.FindAll("button[aria-label='Clear signature']").Should().BeEmpty();
    }

    [Fact]
    public void Maximize_Opens_Dialog_And_HideLine()
    {
        var cut = RenderComponent<StackSignature>(p => p.Add(s => s.HideLine, true));
        cut.FindAll(".border-dashed").Should().BeEmpty();
        cut.Find("button[aria-label='Maximize signature']").Click();
        cut.Find("[role=dialog]").Should().NotBeNull();
        cut.FindAll("canvas").Should().HaveCount(2);
        cut.Find("button[aria-label='Minimize signature']").Click();
        cut.FindAll("[role=dialog]").Should().BeEmpty();
    }

    [Fact]
    public void Maximizable_False_Hides_Button()
    {
        var cut = RenderComponent<StackSignature>(p => p.Add(s => s.Maximizable, false));
        cut.FindAll("button[aria-label='Maximize signature']").Should().BeEmpty();
    }
}

public class FileSelectTests : TestContext
{
    public FileSelectTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public async Task Selected_Files_Are_Validated_And_Readable()
    {
        FileSelectEventArgs? selected = null;
        var cut = RenderComponent<StackFileSelect>(p => p
            .Add(f => f.AllowedExtensions, [".txt"])
            .Add(f => f.MaxFileSize, 10)
            .Add(f => f.OnSelect, a => selected = a));

        cut.FindComponent<InputFile>().UploadFiles(
            InputFileContent.CreateFromText("hello", "a.txt"),
            InputFileContent.CreateFromText("hello", "b.png"),
            InputFileContent.CreateFromText("this is too long", "c.txt"));

        selected!.Files.Select(f => (f.Name, f.IsValid)).Should().Equal(("a.txt", true), ("b.png", false), ("c.txt", false));
        selected.Files[1].InvalidExtension.Should().BeTrue();
        selected.Files[2].InvalidMaxFileSize.Should().BeTrue();

        await using var stream = selected.Files[0].Stream;
        using var reader = new StreamReader(stream);
        (await reader.ReadToEndAsync()).Should().Be("hello");

        cut.FindAll("li[data-file]").Should().HaveCount(3);
        cut.FindAll("[role=alert]").Select(a => a.TextContent).Should().Equal("File type not allowed.", "File is too large.");
    }

    [Fact]
    public void OnSelect_Cancel_And_Remove()
    {
        var cancel = RenderComponent<StackFileSelect>(p => p.Add(f => f.OnSelect, (FileSelectEventArgs a) => a.IsCancelled = true));
        cancel.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromText("x", "a.txt"));
        cancel.FindAll("li").Should().BeEmpty();

        FileSelectEventArgs? removed = null;
        var cut = RenderComponent<StackFileSelect>(p => p.Add(f => f.OnRemove, a => removed = a));
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromText("x", "a.txt"));
        cut.Find("button[aria-label='Remove a.txt']").Click();
        removed!.Files.Single().Name.Should().Be("a.txt");
        cut.FindAll("li").Should().BeEmpty();
    }

    [Fact]
    public void Accept_Defaults_To_Allowed_Extensions()
    {
        var cut = RenderComponent<StackFileSelect>(p => p.Add(f => f.AllowedExtensions, ["pdf", ".docx"]).Add(f => f.Multiple, false));
        var input = cut.Find("input[type=file]");
        input.GetAttribute("accept").Should().Be(".pdf,.docx");
        input.HasAttribute("multiple").Should().BeFalse();
    }
}

public class UploadTests : TestContext
{
    public UploadTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private static StackUpload.JsFile File(string id, string name, long size = 100) => new(id, name, size, "text/plain");

    [Fact]
    public async Task AutoUpload_Sends_Valid_Files_With_Headers()
    {
        var cut = RenderComponent<StackUpload>(p => p
            .Add(u => u.SaveUrl, "/api/save")
            .Add(u => u.AllowedExtensions, [".txt"])
            .Add(u => u.OnUpload, (UploadEventArgs a) => a.RequestHeaders["X-Token"] = "abc"));

        await cut.InvokeAsync(() => cut.Instance.OnJsFilesSelected([File("f1", "a.txt"), File("f2", "b.exe")]));

        var send = JSInterop.VerifyInvoke("MyStackBlazor.uploadSend");
        send.Arguments[1].Should().Be("f1");
        send.Arguments[2].Should().Be("/api/save");
        ((Dictionary<string, string>)send.Arguments[4]!).Should().ContainKey("X-Token");
        cut.Find("li[data-file='a.txt']").GetAttribute("data-status").Should().Be("Uploading");
        cut.Find("li[data-file='b.exe'] p.text-destructive").TextContent.Should().Be("File type not allowed.");
    }

    [Fact]
    public async Task Progress_Success_And_Error_Events()
    {
        int? progress = null;
        UploadSuccessEventArgs? success = null;
        UploadErrorEventArgs? error = null;
        var cut = RenderComponent<StackUpload>(p => p
            .Add(u => u.SaveUrl, "/api/save")
            .Add(u => u.OnProgress, a => progress = a.Progress)
            .Add(u => u.OnSuccess, a => success = a)
            .Add(u => u.OnError, a => error = a));

        await cut.InvokeAsync(() => cut.Instance.OnJsFilesSelected([File("f1", "a.txt"), File("f2", "b.txt")]));
        await cut.InvokeAsync(() => cut.Instance.OnJsProgress("f1", 40));
        progress.Should().Be(40);
        cut.Find("li[data-file='a.txt'] [role=progressbar]").GetAttribute("aria-valuenow").Should().Be("40");

        await cut.InvokeAsync(() => cut.Instance.OnJsCompleted("f1", true, 200, "OK", "saved"));
        await cut.InvokeAsync(() => cut.Instance.OnJsCompleted("f2", false, 500, "Error", "boom"));

        success!.Request.ResponseText.Should().Be("saved");
        success.Operation.Should().Be(UploadOperationType.Upload);
        error!.Request.Status.Should().Be(500);
        cut.Find("li[data-file='a.txt']").GetAttribute("data-status").Should().Be("Uploaded");
        cut.Find("button[aria-label='Retry b.txt']").Should().NotBeNull();
    }

    [Fact]
    public async Task Manual_Upload_Shows_Upload_Button()
    {
        var cut = RenderComponent<StackUpload>(p => p.Add(u => u.SaveUrl, "/api/save").Add(u => u.AutoUpload, false));
        await cut.InvokeAsync(() => cut.Instance.OnJsFilesSelected([File("f1", "a.txt")]));
        JSInterop.Invocations.Should().NotContain(i => i.Identifier == "MyStackBlazor.uploadSend");
        cut.FindAll("button").First(b => b.TextContent.Trim() == "Upload").Click();
        JSInterop.VerifyInvoke("MyStackBlazor.uploadSend");
    }

    [Fact]
    public async Task Removing_An_Uploaded_File_Calls_RemoveUrl()
    {
        JSInterop.Setup<StackUpload.RemoveResult>("MyStackBlazor.uploadRemove", _ => true).SetResult(new StackUpload.RemoveResult(true, 200, "OK", ""));
        UploadSuccessEventArgs? success = null;
        var cut = RenderComponent<StackUpload>(p => p
            .Add(u => u.RemoveUrl, "/api/remove")
            .Add(u => u.Files, [new UploadFileInfo { Name = "old.pdf", Size = 10 }])
            .Add(u => u.OnSuccess, a => success = a));

        cut.Find("li[data-file='old.pdf']").GetAttribute("data-status").Should().Be("Uploaded");
        await cut.InvokeAsync(() => cut.Instance.RemoveFileAsync(cut.Instance.CurrentFiles[0].Id));

        JSInterop.VerifyInvoke("MyStackBlazor.uploadRemove");
        success!.Operation.Should().Be(UploadOperationType.Remove);
        cut.FindAll("li").Should().BeEmpty();
    }

    [Fact]
    public async Task OnSelect_Cancel_Forgets_Files()
    {
        var cut = RenderComponent<StackUpload>(p => p
            .Add(u => u.SaveUrl, "/api/save")
            .Add(u => u.OnSelect, (UploadSelectEventArgs a) => a.IsCancelled = true));
        await cut.InvokeAsync(() => cut.Instance.OnJsFilesSelected([File("f1", "a.txt")]));
        cut.FindAll("li").Should().BeEmpty();
        JSInterop.VerifyInvoke("MyStackBlazor.uploadForget");
    }
}
