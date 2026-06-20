#!/usr/bin/env python3
"""Fix CascadingParameter parent component type references"""
import re
from pathlib import Path

TYPE_FIXES = {
    "Accordion": "StackAccordion",
    "Command": "StackCommand",
    "DropdownMenu": "StackDropdownMenu",
    "Menubar": "StackMenubar",
    "NavigationMenuItem": "StackNavigationMenuItem",
    "PanelBar": "StackPanelBar",
    "RadioGroup": "StackRadioGroup",
    "ResizablePanel": "StackResizablePanel",
    "ResizablePanelGroup": "StackResizablePanelGroup",
    "Splitter": "StackSplitter",
    "Stepper": "StackStepper",
    "StepperStep": "StackStepperStep",
    "Tabs": "StackTabs",
    "ToggleGroup": "StackToggleGroup",
    "Wizard": "StackWizard",
    "WizardStep": "StackWizardStep",
    "Workflow": "StackWorkflow",
    "WorkflowStep": "StackWorkflowStep",
    "DataTable": "StackDataTable",
}

comp_dir = Path(r'c:\MyCode\MyStackBlazor\src\MyStackBlazor\Components')
count = 0

for f in comp_dir.rglob('*.razor'):
    with open(f, 'r', encoding='utf-8') as file:
        content = file.read()
    
    orig = content
    
    for old_type, new_type in TYPE_FIXES.items():
        # Fix CascadingParameter types: public OldType Parent { get; set; }
        content = re.sub(
            rf'\bpublic\s+{old_type}\s+(\w+)\s*{{\s*get;\s*set;',
            f'public {new_type} \\1 {{ get; set;',
            content
        )
        # Fix List<OldType>
        content = re.sub(
            rf'List<{old_type}>',
            f'List<{new_type}>',
            content
        )
        # Fix generic declarations: Dictionary<K, OldType>
        content = re.sub(
            rf',\s*{old_type}[>,]',
            f', {new_type}>',
            content
        )
        # Fix method parameters and variable declarations
        content = re.sub(
            rf'\b{old_type}\s+(\w+)\s*(,|\)|;|=)',
            f'{new_type} \\1 \\2',
            content
        )
        # Fix List<OldType>: foreach, method params, etc  
        content = re.sub(
            rf'\b{old_type}<',
            f'{new_type}<',
            content
        )
    
    if content != orig:
        with open(f, 'w', encoding='utf-8') as file:
            file.write(content)
        count += 1
        print(f"Fixed: {f.name}")

print(f"\nTotal files fixed: {count}")
