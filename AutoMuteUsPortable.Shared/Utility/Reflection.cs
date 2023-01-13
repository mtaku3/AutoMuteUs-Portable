using System.Reflection;

namespace AutoMuteUsPortable.Shared.Utility;

public static partial class Utils
{
    public static PropertyInfo? PropertyInfoByName(object obj, string name)
    {
        return obj.GetType().GetProperty(name);
    }

    public static T? PropertyByName<T>(object obj, string name)
    {
        var info = PropertyInfoByName(obj, name);
        var value = info.GetValue(obj);
        return value is T t ? t : default;
    }
}