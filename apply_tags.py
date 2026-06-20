#!/usr/bin/env python3
import re
from pathlib import Path

RENAMES = {
    "Avatar": "StackAvatar", "Badge": "StackBadge", "Button": "StackButton", "Chip": "StackChip", "ChipList": "StackChipList",
    "Direction": "StackDirection", "Empty": "StackEmpty", "Icon": "StackIcon", "Item": "StackItem", "Kbd": "StackKbd",
    "Label": "StackLabel", "Separator": "StackSeparator", "Skeleton": "StackSkeleton", "Spinner": "StackSpinner",
    "ThemeProvider": "StackThemeProvider", "ThemeToggle": "StackThemeToggle", "Typography": "StackTypography",
    "AutoComplete": "StackAutoComplete", "Checkbox": "StackCheckbox", "ColorPalette": "StackColorPalette", "ColorPicker": "StackColorPicker",
    "Combobox": "StackCombobox", "DatePicker": "StackDatePicker", "DateRangePicker": "StackDateRangePicker", "DateTimePicker": "StackDateTimePicker",
    "DropDownTree": "StackDropDownTree", "FileDownload": "StackFileDownload", "FileUpload": "StackFileUpload", "FloatingLabel": "StackFloatingLabel",
    "FormField": "StackFormField", "Input": "StackInput", "InputGroup": "StackInputGroup", "InputOtp": "StackInputOtp",
    "ListBox": "StackListBox", "MarkdownEditor": "StackMarkdownEditor", "MaskedTextBox": "StackMaskedTextBox", "MultiColumnComboBox": "StackMultiColumnComboBox",
    "MultiSelect": "StackMultiSelect", "NativeSelect": "StackNativeSelect", "NumericInput": "StackNumericInput", "RadioGroup": "StackRadioGroup",
    "RadioGroupItem": "StackRadioGroupItem", "RangeSlider": "StackRangeSlider", "Rating": "StackRating", "Select": "StackSelect",
    "Slider": "StackSlider", "Switch": "StackSwitch", "Textarea": "StackTextarea", "TimePicker": "StackTimePicker",
    "TimePicker12": "StackTimePicker12", "TimePicker24": "StackTimePicker24", "Accordion": "StackAccordion", "AccordionItem": "StackAccordionItem",
    "AnimationContainer": "StackAnimationContainer", "AspectRatio": "StackAspectRatio", "Card": "StackCard", "CardContent": "StackCardContent",
    "CardDescription": "StackCardDescription", "CardFooter": "StackCardFooter", "CardHeader": "StackCardHeader", "CardTitle": "StackCardTitle",
    "Carousel": "StackCarousel", "ChunkProgressBar": "StackChunkProgressBar", "Collapsible": "StackCollapsible", "GridLayout": "StackGridLayout",
    "Loader": "StackLoader", "PanelBar": "StackPanelBar", "PanelBarItem": "StackPanelBarItem", "ResizableHandle": "StackResizableHandle",
    "ResizablePanel": "StackResizablePanel", "ResizablePanelGroup": "StackResizablePanelGroup", "ScrollArea": "StackScrollArea",
    "Splitter": "StackSplitter", "SplitterPane": "StackSplitterPane", "AppBar": "StackAppBar", "Breadcrumb": "StackBreadcrumb",
    "BreadcrumbItem": "StackBreadcrumbItem", "ButtonGroup": "StackButtonGroup", "CarouselSlide": "StackCarouselSlide", "ContextMenu": "StackContextMenu",
    "ContextMenuItem": "StackContextMenuItem", "DropDownButton": "StackDropDownButton", "DropdownMenu": "StackDropdownMenu", "DropdownMenuItem": "StackDropdownMenuItem",
    "FloatingActionButton": "StackFloatingActionButton", "Menubar": "StackMenubar", "MenubarMenu": "StackMenubarMenu", "NavigationMenu": "StackNavigationMenu",
    "NavigationMenuContent": "StackNavigationMenuContent", "NavigationMenuItem": "StackNavigationMenuItem", "NavigationMenuLink": "StackNavigationMenuLink",
    "NavigationMenuList": "StackNavigationMenuList", "NavigationMenuTrigger": "StackNavigationMenuTrigger", "Pagination": "StackPagination",
    "Sidebar": "StackSidebar", "SplitButton": "StackSplitButton", "Stepper": "StackStepper", "StepperStep": "StackStepperStep",
    "Tabs": "StackTabs", "TabsContent": "StackTabsContent", "TabsList": "StackTabsList", "TabsTrigger": "StackTabsTrigger",
    "ToggleButton": "StackToggleButton", "ToggleGroup": "StackToggleGroup", "ToggleGroupItem": "StackToggleGroupItem", "Toolbar": "StackToolbar",
    "Wizard": "StackWizard", "WizardStep": "StackWizardStep", "Workflow": "StackWorkflow", "WorkflowStep": "StackWorkflowStep",
    "AdvancedTable": "StackAdvancedTable", "Alert": "StackAlert", "AlertDescription": "StackAlertDescription", "AlertTitle": "StackAlertTitle",
    "DataTable": "StackDataTable", "FileManager": "StackFileManager", "ListView": "StackListView", "MsVirtualList": "StackVirtualList",
    "PivotGrid": "StackPivotGrid", "Progress": "StackProgress", "Table": "StackTable", "TableBody": "StackTableBody", "TableCell": "StackTableCell",
    "TableHead": "StackTableHead", "TableHeader": "StackTableHeader", "TableRow": "StackTableRow", "TreeList": "StackTreeList",
    "TreeView": "StackTreeView", "TreeViewNode": "StackTreeViewNode", "AlertDialog": "StackAlertDialog", "Command": "StackCommand",
    "CommandEmpty": "StackCommandEmpty", "CommandGroup": "StackCommandGroup", "CommandInput": "StackCommandInput", "CommandItem": "StackCommandItem",
    "CommandList": "StackCommandList", "Dialog": "StackDialog", "DialogFooter": "StackDialogFooter", "DialogHeader": "StackDialogHeader",
    "DialogTitle": "StackDialogTitle", "Drawer": "StackDrawer", "FileViewer": "StackFileViewer", "HoverCard": "StackHoverCard",
    "ImageViewer": "StackImageViewer", "Popover": "StackPopover", "Popup": "StackPopup", "Sheet": "StackSheet", "Toaster": "StackToaster",
    "Tooltip": "StackTooltip", "Window": "StackWindow", "BarChart": "StackBarChart", "GaugeChart": "StackGaugeChart",
    "LineChart": "StackLineChart", "PieChart": "StackPieChart", "CardsBlock": "StackCardsBlock", "CompactCard": "StackCompactCard",
    "ContactFormBlock": "StackContactFormBlock", "DashboardCard": "StackDashboardCard", "DashboardSidebar": "StackDashboardSidebar",
    "FeaturesBlock": "StackFeaturesBlock", "FilterPanel": "StackFilterPanel", "FooterBlock": "StackFooterBlock", "FlyoutNavigation": "StackFlyoutNavigation",
    "HeroBlock": "StackHeroBlock", "IncentivesBlock": "StackIncentivesBlock", "LocationsBlock": "StackLocationsBlock", "LoginForm": "StackLoginForm",
    "NewsletterBlock": "StackNewsletterBlock", "PartnersBlock": "StackPartnersBlock", "PricingBlock": "StackPricingBlock", "ProductCard": "StackProductCard",
    "SignUpForm": "StackSignUpForm", "SortBar": "StackSortBar", "StatsBlock": "StackStatsBlock", "TestimonialsBlock": "StackTestimonialsBlock",
    "TopNavbar": "StackTopNavbar", "AIUsageMonitoring": "StackAIUsageMonitoring", "AutomotiveIndustry": "StackAutomotiveIndustry",
    "ControlShowcase": "StackControlShowcase", "FashionCatalogue": "StackFashionCatalogue", "FintechDashboard": "StackFintechDashboard",
    "HealthcareAdmin": "StackHealthcareAdmin", "InternationalBank": "StackInternationalBank", "MachineManufacturing": "StackMachineManufacturing",
    "ProjectTracker": "StackProjectTracker", "RealEstate": "StackRealEstate", "SaaSProduct": "StackSaaSProduct", "SocialMediaManagement": "StackSocialMediaManagement",
    "Travel": "StackTravel", "MsCalendar": "StackCalendar",
}

def process(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    orig = content
    for old, new in RENAMES.items():
        content = re.sub(rf'<{old}\b([\s\n>@])', f'<{new}\\1', content)
        content = re.sub(rf'</{old}>', f'</{new}>', content)
    if content != orig:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        return True
    return False

comp_dir = Path(r'c:\MyCode\MyStackBlazor\src\MyStackBlazor\Components')
count = 0
for f in comp_dir.rglob('*.razor'):
    if process(str(f)):
        count += 1
        print(f"Updated: {f.name}")

print(f"\nTotal files updated: {count}")
