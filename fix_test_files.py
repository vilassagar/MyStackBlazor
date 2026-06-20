#!/usr/bin/env python3
"""Fix all component names in test files"""
import re
from pathlib import Path

# Map old names to new Stack-prefixed names
COMPONENT_RENAMES = {
    "MsPivotGrid": "StackPivotGrid",
    "Switch": "StackSwitch",
    "DropDownTree": "StackDropDownTree",
    "FileManager": "StackFileManager",
    "TreeView": "StackTreeView",
    "Input": "StackInput",
    "Textarea": "StackTextarea",
    "Select": "StackSelect",
    "Slider": "StackSlider",
    "Rating": "StackRating",
    "NumericInput": "StackNumericInput",
    "RadioGroup": "StackRadioGroup",
    "TabsList": "StackTabsList",
}

test_dir = Path(r'c:\MyCode\MyStackBlazor\tests\MyStackBlazor.UnitTests')
count = 0
files_fixed = []

for f in test_dir.rglob('*.cs'):
    with open(f, 'r', encoding='utf-8') as file:
        content = file.read()
    
    orig = content
    
    for old_name, new_name in COMPONENT_RENAMES.items():
        # Replace <OldName> with <NewName> (generic and non-generic)
        content = re.sub(rf'\b{old_name}(<[^>]+>)?\b', new_name + (r'\1' if r'\1' in content else ''), content)
        
        # More precise: replace specific patterns in RenderComponent calls
        content = re.sub(rf'RenderComponent<{old_name}>', f'RenderComponent<{new_name}>', content)
        content = re.sub(rf'OpenComponent<{old_name}>', f'OpenComponent<{new_name}>', content)
        content = re.sub(rf'ctx\.RenderComponent<{old_name}>', f'ctx.RenderComponent<{new_name}>', content)
        
        # Handle nullable generics like TreeView<T> -> StackTreeView<T>
        content = re.sub(rf'{old_name}<', f'{new_name}<', content)
        
        # Replace in Cast expressions: (TreeView<T>) -> (StackTreeView<T>)
        content = re.sub(rf'\({old_name}', f'({new_name}', content)
    
    if content != orig:
        with open(f, 'w', encoding='utf-8') as file:
            file.write(content)
        count += 1
        files_fixed.append(f.name)
        print(f"Fixed: {f.name}")

print(f"\nTotal files fixed: {count}")
if files_fixed:
    print("\nFiles updated:")
    for fname in files_fixed:
        print(f"  - {fname}")
