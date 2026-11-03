"""Check the actual release artifacts, including internal dependency versions and assets."""
from pathlib import Path
import sys
from zipfile import ZipFile
import xml.etree.ElementTree as ET

folder, version = Path(sys.argv[1]), sys.argv[2]
expected = {"MyStackBlazor", *[f"MyStackBlazor.{name}" for name in
    ("Core", "Security", "Accessibility", "Calendar", "DataGrid", "Localization")]}
seen = set()
for path in folder.glob("*.nupkg"):
    with ZipFile(path) as package:
        names = package.namelist()
        nuspec = next(name for name in names if name.endswith(".nuspec"))
        root = ET.fromstring(package.read(nuspec))
        def elements(tag):
            return root.findall(f".//{{*}}{tag}")
        name = elements("id")[0].text
        assert name in expected and name not in seen, f"Unexpected/duplicate package: {name}"
        seen.add(name)
        assert elements("version")[0].text == version, f"Wrong version: {path}"
        assert any(n.endswith(f"/{name}.dll") for n in names), f"Missing DLL: {path}"
        for dep in elements("dependency"):
            if dep.attrib['id'] in expected:
                assert dep.attrib['version'] in (version, f"[{version}, )", f"[{version},)", f"[{version}]"), dep.attrib
        if name == "MyStackBlazor":
            for asset in ("staticwebassets/css/mystackblazor.css", "staticwebassets/js/dialog.js"):
                assert asset in names and len(package.read(asset)) > 0, f"Missing/empty asset: {asset}"
assert seen == expected, f"Missing packages: {expected - seen}"
print(f"Verified {len(seen)} packages at {version}")
