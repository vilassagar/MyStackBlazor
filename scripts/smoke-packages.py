"""Compile a clean Razor consumer against packed libraries, not project references."""
from pathlib import Path
import subprocess
import sys
import tempfile
from xml.sax.saxutils import escape

feed, version = Path(sys.argv[1]).resolve(), sys.argv[2]
packages = ['MyStackBlazor', *[f'MyStackBlazor.{name}' for name in
    ('Core', 'Accessibility', 'DataGrid', 'Calendar', 'Localization', 'Security')]]
with tempfile.TemporaryDirectory(prefix='mystack-consumer-') as directory:
    folder = Path(directory)
    references = '\n'.join(f'<PackageReference Include="{name}" Version="{escape(version)}" />' for name in packages)
    (folder / 'Consumer.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable></PropertyGroup>
  <ItemGroup><FrameworkReference Include="Microsoft.AspNetCore.App" />{references}</ItemGroup>
</Project>''')
    (folder / 'Consumer.razor').write_text('''@using MyStackBlazor.Components.Common
@using MyStackBlazor.Components.Form
@using MyStackBlazor.Components.Overlays
<StackButton>Save</StackButton>
<StackInput @bind-Value="_name" />
<StackDialog IsOpen="true" AriaLabel="Package smoke test">@_name</StackDialog>
@code { private string? _name; }
''')
    (folder / 'Registration.cs').write_text('''using Microsoft.Extensions.DependencyInjection;
using MyStackBlazor;
public static class Registration {
    public static IServiceCollection AddComponents(IServiceCollection services) => services.AddMyStackBlazor();
}
''')
    subprocess.run(['dotnet', 'restore', str(folder / 'Consumer.csproj'), '--source', str(feed),
                    '--source', 'https://api.nuget.org/v3/index.json', '--disable-parallel'], check=True)
    subprocess.run(['dotnet', 'build', str(folder / 'Consumer.csproj'), '--no-restore', '-c', 'Release', '-m:1'], check=True)
print('Packed-library Razor consumer compiled successfully')
