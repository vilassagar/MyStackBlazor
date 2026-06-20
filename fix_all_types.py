#!/usr/bin/env python3
"""Comprehensive fix for all component type references including @using directives"""
import re
from pathlib import Path

TYPE_FIXES = {
    "Accordion": "StackAccordion",
    "Carousel": "StackCarousel",
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
}

comp_dir = Path(r'c:\MyCode\MyStackBlazor\src\MyStackBlazor\Components')
count = 0

for f in comp_dir.rglob('*.razor'):
    with open(f, 'r', encoding='utf-8') as file:
        content = file.read()
    
    orig = content
    
    for old_type, new_type in TYPE_FIXES.items():
        # Fix @using directives: @using ... OldType
        content = re.sub(
            rf'(@using\s+[\w.]*){old_type}',
            rf'\1{new_type}',
            content
        )
        # Fix [CascadingParameter] public OldType
        content = re.sub(
            rf'\[CascadingParameter\]\s+public\s+{old_type}\s+',
            f'[CascadingParameter] public {new_type} ',
            content
        )
        # Fix @using S = ... OldType.
        content = re.sub(
            rf'=\s+[\w.]*{old_type}\.',
            f'= {new_type}.',
            content
        )
    
    if content != orig:
        with open(f, 'w', encoding='utf-8') as file:
            file.write(content)
        count += 1
        print(f"Fixed: {f.name}")

print(f"\nTotal files fixed: {count}")
