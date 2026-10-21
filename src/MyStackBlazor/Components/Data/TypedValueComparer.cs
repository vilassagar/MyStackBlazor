using System.Collections;
using System.Globalization;

namespace MyStackBlazor.Components.Data;

/// <summary>
/// Compares cell values by their real type: nulls first, numbers numerically (across numeric types),
/// dates chronologically, strings case-insensitively in the current culture.
/// </summary>
public sealed class TypedValueComparer : IComparer<object?>
{
    public static readonly TypedValueComparer Instance = new();

    public int Compare(object? x, object? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        if (x is string sx && y is string sy)
            return string.Compare(sx, sy, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);

        var tx = x.GetType();
        var ty = y.GetType();
        if (tx != ty && IsNumeric(tx) && IsNumeric(ty))
        {
            try { return Convert.ToDecimal(x, CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(y, CultureInfo.InvariantCulture)); }
            catch (OverflowException) { return Convert.ToDouble(x, CultureInfo.InvariantCulture).CompareTo(Convert.ToDouble(y, CultureInfo.InvariantCulture)); }
        }

        if (tx == ty && x is IComparable c) return c.CompareTo(y);
        return Comparer.Default.Compare(x.ToString(), y.ToString());
    }

    private static bool IsNumeric(Type t) => Type.GetTypeCode(t) is
        TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or
        TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
}
