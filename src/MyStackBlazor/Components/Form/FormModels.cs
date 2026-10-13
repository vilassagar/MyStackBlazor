using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MyStackBlazor.Components.Form;

/// <summary>Whether labels sit above (Vertical) or beside (Horizontal) the editors.</summary>
public enum FormOrientation
{
    Vertical,
    Horizontal,
}

/// <summary>Alignment of the form buttons.</summary>
public enum FormButtonsLayout
{
    Start,
    Center,
    End,
    Stretch,
}

/// <summary>How validation messages are shown for form items.</summary>
public enum FormValidationMessageType
{
    Inline,
    Tooltip,
    None,
}

/// <summary>Editor used for a form item (by default it follows the property type).</summary>
public enum FormEditorType
{
    TextBox,
    TextArea,
    MaskedTextBox,
    NumericTextBox,
    DatePicker,
    DateTimePicker,
    TimePicker,
    CheckBox,
    Switch,
    DropDownList,
}

/// <summary>Position of a validation tooltip relative to its target.</summary>
public enum TooltipPosition
{
    Top,
    Bottom,
    Left,
    Right,
}

/// <summary>Builds editors for model properties (used by the auto-generated StackForm items).</summary>
internal static class FormEditorFactory
{
    private static readonly Type[] NumericTypes =
        [typeof(int), typeof(long), typeof(short), typeof(byte), typeof(decimal), typeof(double), typeof(float), typeof(uint), typeof(ulong), typeof(ushort), typeof(sbyte)];

    public static PropertyInfo? Property(object model, string field) =>
        model.GetType().GetProperty(field, BindingFlags.Public | BindingFlags.Instance);

    /// <summary>Properties a generated form shows: public, read/write, simple types, not [Editable(false)] or [ScaffoldColumn(false)].</summary>
    public static IEnumerable<PropertyInfo> EditableProperties(object model) =>
        model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
            .Where(p => p.GetCustomAttribute<EditableAttribute>()?.AllowEdit != false)
            .Where(p => p.GetCustomAttribute<ScaffoldColumnAttribute>()?.Scaffold != false)
            .Where(p => DefaultEditor(p.PropertyType) is not null);

    public static FormEditorType? DefaultEditor(Type type)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;
        if (t == typeof(string)) return FormEditorType.TextBox;
        if (t == typeof(bool)) return FormEditorType.CheckBox;
        if (t.IsEnum) return FormEditorType.DropDownList;
        if (NumericTypes.Contains(t)) return FormEditorType.NumericTextBox;
        if (t == typeof(DateTime) || t == typeof(DateOnly) || t == typeof(DateTimeOffset)) return FormEditorType.DatePicker;
        if (t == typeof(TimeOnly) || t == typeof(TimeSpan)) return FormEditorType.TimePicker;
        return null;
    }

    /// <summary>Label from [Display(Name)], [DisplayName] or the property name split into words.</summary>
    public static string Label(PropertyInfo property) =>
        property.GetCustomAttribute<DisplayAttribute>()?.GetName()
        ?? property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
        ?? Regex.Replace(property.Name, "(?<=[a-z0-9])([A-Z])", " $1");

    public static bool IsRequired(PropertyInfo property) => property.GetCustomAttribute<RequiredAttribute>() is not null;

    /// <summary>A two-way bound editor for <paramref name="property"/> of <paramref name="model"/>.</summary>
    public static RenderFragment Editor(object receiver, object model, PropertyInfo property, FormEditorType editor, string id, bool enabled) => builder =>
    {
        var type = property.PropertyType;
        var value = property.GetValue(model);
        var expression = Expression.Lambda(typeof(Func<>).MakeGenericType(type), Expression.Property(Expression.Constant(model), property));
        var callback = typeof(FormEditorFactory).GetMethod(nameof(Callback), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(type)
            .Invoke(null, [receiver, (Action<object?>)(v => property.SetValue(model, v))])!;
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        void Common(RenderTreeBuilder b)
        {
            b.AddAttribute(1, "Value", value);
            b.AddAttribute(2, "ValueChanged", callback);
            b.AddAttribute(3, "ValueExpression", expression);
            b.AddAttribute(4, "Id", id);
            b.AddAttribute(5, "Enabled", enabled);
        }

        switch (editor)
        {
            case FormEditorType.CheckBox or FormEditorType.Switch when underlying == typeof(bool):
                builder.OpenComponent(0, editor == FormEditorType.Switch ? typeof(StackSwitch) : typeof(StackCheckbox));
                builder.AddAttribute(1, "Checked", value is true);
                builder.AddAttribute(2, "CheckedChanged", EventCallback.Factory.Create<bool>(receiver, (bool v) => property.SetValue(model, v)));
                builder.AddAttribute(3, "Id", id);
                builder.AddAttribute(4, "Disabled", !enabled);
                builder.CloseComponent();
                return;

            case FormEditorType.DropDownList when underlying.IsEnum:
                builder.OpenComponent(0, typeof(StackDropDownList<,>).MakeGenericType(underlying, type));
                Common(builder);
                builder.AddAttribute(6, "Data", Enum.GetValues(underlying));
                if (type != underlying) builder.AddAttribute(7, "DefaultText", "Select…");
                builder.CloseComponent();
                return;

            case FormEditorType.NumericTextBox:
                builder.OpenComponent(0, typeof(StackNumericTextBox<>).MakeGenericType(type));
                Common(builder);
                builder.CloseComponent();
                return;

            case FormEditorType.DatePicker or FormEditorType.DateTimePicker or FormEditorType.TimePicker:
                var picker = editor switch
                {
                    FormEditorType.DateTimePicker => typeof(StackDateTimePicker<>),
                    FormEditorType.TimePicker     => typeof(StackTimePicker<>),
                    _                             => typeof(StackDatePicker<>),
                };
                builder.OpenComponent(0, picker.MakeGenericType(type));
                Common(builder);
                builder.CloseComponent();
                return;

            case FormEditorType.TextArea:
                builder.OpenComponent<StackTextarea>(0);
                Common(builder);
                builder.CloseComponent();
                return;

            case FormEditorType.MaskedTextBox:
                builder.OpenComponent<StackMaskedTextBox>(0);
                Common(builder);
                builder.CloseComponent();
                return;

            default:
                builder.OpenComponent<StackTextBox>(0);
                Common(builder);
                builder.AddAttribute(6, "DebounceDelay", 0);
                builder.CloseComponent();
                return;
        }
    };

    private static EventCallback<T> Callback<T>(object receiver, Action<object?> set) =>
        EventCallback.Factory.Create<T>(receiver, (T v) => set(v));
}
