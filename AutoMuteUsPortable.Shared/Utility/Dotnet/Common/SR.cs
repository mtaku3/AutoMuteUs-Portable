//The MIT License (MIT)

//Copyright(c) 2022 mtaku3
//Copyright(c).NET Foundation and Contributors

//    All rights reserved.

//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.

//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Resources;

namespace AutoMuteUsPortable.Shared.Utility.Dotnet.Common;

internal static partial class SR
{
    private static readonly bool s_usingResourceKeys =
        AppContext.TryGetSwitch("System.Resources.UseSystemResourceKeys", out var usingResourceKeys)
            ? usingResourceKeys
            : false;

    // This method is used to decide if we need to append the exception message parameters to the message when calling SR.Format.
    // by default it returns the value of System.Resources.UseSystemResourceKeys AppContext switch or false if not specified.
    // Native code generators can replace the value this returns based on user input at the time of native code generation.
    // The Linker is also capable of replacing the value of this method when the application is being trimmed.
    private static bool UsingResourceKeys()
    {
        return s_usingResourceKeys;
    }

    internal static string GetResourceString(string resourceKey)
    {
        if (UsingResourceKeys()) return resourceKey;

        string? resourceString = null;
        try
        {
            resourceString = ResourceManager.GetString(resourceKey);
        }
        catch (MissingManifestResourceException)
        {
        }

        return resourceString!; // only null if missing resources
    }

    internal static string GetResourceString(string resourceKey, string defaultString)
    {
        var resourceString = GetResourceString(resourceKey);

        return resourceKey == resourceString || resourceString == null ? defaultString : resourceString;
    }

    internal static string Format(string resourceFormat, object? p1)
    {
        if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1);

        return string.Format(resourceFormat, p1);
    }

    internal static string Format(string resourceFormat, object? p1, object? p2)
    {
        if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1, p2);

        return string.Format(resourceFormat, p1, p2);
    }

    internal static string Format(string resourceFormat, object? p1, object? p2, object? p3)
    {
        if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1, p2, p3);

        return string.Format(resourceFormat, p1, p2, p3);
    }

    internal static string Format(string resourceFormat, params object?[]? args)
    {
        if (args != null)
        {
            if (UsingResourceKeys()) return resourceFormat + ", " + string.Join(", ", args);

            return string.Format(resourceFormat, args);
        }

        return resourceFormat;
    }

    internal static string Format(IFormatProvider? provider, string resourceFormat, object? p1)
    {
        if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1);

        return string.Format(provider, resourceFormat, p1);
    }

    internal static string Format(IFormatProvider? provider, string resourceFormat, object? p1, object? p2)
    {
        if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1, p2);

        return string.Format(provider, resourceFormat, p1, p2);
    }

    internal static string Format(IFormatProvider? provider, string resourceFormat, object? p1, object? p2, object? p3)
    {
        if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1, p2, p3);

        return string.Format(provider, resourceFormat, p1, p2, p3);
    }

    internal static string Format(IFormatProvider? provider, string resourceFormat, params object?[]? args)
    {
        if (args != null)
        {
            if (UsingResourceKeys()) return resourceFormat + ", " + string.Join(", ", args);

            return string.Format(provider, resourceFormat, args);
        }

        return resourceFormat;
    }
}