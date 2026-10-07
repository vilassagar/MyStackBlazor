namespace MyStackBlazor.Components.Form;

/// <summary>Implemented by dropdowns that accept a <see cref="StackDropDownListPopupSettings"/> child.</summary>
public interface IDropDownListPopupSettingsHost
{
    void SetPopupSettings(StackDropDownListPopupSettings settings);
}
