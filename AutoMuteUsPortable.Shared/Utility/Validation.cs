using System.Text.RegularExpressions;

namespace AutoMuteUsPortable.Shared.Utility;

public static partial class Utils
{
    public static bool ValidateDirectoryPath(string path, bool allowRelativePath = false)
    {
        var isValid = true;

        try
        {
            Path.GetFullPath(path);
        }
        catch
        {
            isValid = false;
        }

        isValid = Regex.IsMatch(path, @"^[a-zA-Z]\:(/|\\)([^<>:""/\\|?*]+(/|\\))*([^<>:""/\\|?*]+)?$");

        if (allowRelativePath)
            isValid |= Regex.IsMatch(path, @"^(\.(/|\\)|(\.\.(/|\\))+)([^<>:""/\\|?*]+(/|\\))*([^<>:""/\\|?*]+)?$");

        return isValid;
    }

    public static bool ValidateFilePath(string path, string? extension, bool allowRelativePath = false)
    {
        var isValid = true;
        extension = extension != null ? $@"\.{extension}" : "";

        try
        {
            Path.GetFullPath(path);
        }
        catch
        {
            isValid = false;
        }

        isValid = Regex.IsMatch(path,
            $@"^[a-zA-Z]\:(/|\\)([^<>:""/\\|?*]+(/|\\))*([^<>:""/\\|?*]+){extension}$");

        if (allowRelativePath)
            isValid |= Regex.IsMatch(path,
                $@"^(\.(/|\\)|(\.\.(/|\\))+)([^<>:""/\\|?*]+(/|\\))*([^<>:""/\\|?*]+{extension})$");

        return isValid;
    }
}