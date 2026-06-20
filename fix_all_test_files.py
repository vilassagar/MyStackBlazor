#!/usr/bin/env python3
"""Comprehensive fix for all component names in test files"""
import re
from pathlib import Path

# Get all Stack*.razor component files to build comprehensive mapping
comp_dir = Path(r'c:\MyCode\MyStackBlazor\src\MyStackBlazor\Components')
component_names = set()

for f in comp_dir.rglob('Stack*.razor'):
    # Extract component name: StackButton.razor -> Button
    name = f.stem[5:]  # Remove "Stack" prefix
    component_names.add(name)

# Build mapping: old name -> new Stack name
COMPONENT_RENAMES = {name: f'Stack{name}' for name in component_names}

# Also include special cases that might appear in tests
COMPONENT_RENAMES.update({
    "MsPivotGrid": "StackPivotGrid",
    "MsCalendar": "StackCalendar",
    "MsVirtualList": "StackVirtualList",
})

print(f"Found {len(COMPONENT_RENAMES)} component renames to apply")

test_dir = Path(r'c:\MyCode\MyStackBlazor\tests\MyStackBlazor.UnitTests')
count = 0
files_fixed = []

for f in test_dir.rglob('*.cs'):
    with open(f, 'r', encoding='utf-8') as file:
        content = file.read()
    
    orig = content
    
    # Use word boundaries to match component names accurately
    for old_name, new_name in sorted(COMPONENT_RENAMES.items(), key=lambda x: -len(x[0])):
        # Match: ClassName<, ClassName> (with generics)
        # RenderComponent<ClassName>
        # OpenComponent<ClassName>
        # ctx.RenderComponent<ClassName>
        # : ClassName where
        
        # Replace in angle brackets: <OldName> or <OldName<
        content = re.sub(rf'<{re.escape(old_name)}(<|>|[\s,])', rf'<{new_name}\1', content)
        
        # Replace in Cast: (OldName)
        content = re.sub(rf'\({re.escape(old_name)}\)', f'({new_name})', content)
        
        # Replace in type constraints: where T : OldName
        content = re.sub(rf':\s+{re.escape(old_name)}\b', f': {new_name}', content)
        
        # Replace as property type: public OldName
        content = re.sub(rf'public\s+{re.escape(old_name)}\b', f'public {new_name}', content)
        
        # Replace in variable declaration: var x = new OldName
        content = re.sub(rf'new\s+{re.escape(old_name)}\b', f'new {new_name}', content)
        
        # Replace standalone word with word boundaries
        content = re.sub(rf'\b{re.escape(old_name)}\b', new_name, content)
    
    if content != orig:
        with open(f, 'w', encoding='utf-8') as file:
            file.write(content)
        count += 1
        files_fixed.append(f.name)
        print(f"Fixed: {f.name}")

print(f"\nTotal files fixed: {count}")
