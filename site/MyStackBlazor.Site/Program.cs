using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyStackBlazor;
using MyStackBlazor.DataGrid;
using MyStackBlazor.Site;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMyStackBlazor();
builder.Services.AddMyStackBlazorDataGrid();

await builder.Build().RunAsync();
