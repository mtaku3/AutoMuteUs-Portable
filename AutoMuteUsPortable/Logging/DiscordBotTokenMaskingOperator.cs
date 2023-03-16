using System;
using System.Text;
using System.Text.RegularExpressions;
using Serilog.Enrichers.Sensitive;

namespace AutoMuteUsPortable.Logging;

public class DiscordBotTokenMaskingOperator : IMaskingOperator
{
    private const string DiscordBotTokenPattern = @"[A-Za-z0-9-_+/]+\.[A-Za-z0-9-_+/]+\.[A-Za-z0-9-_+/]+";

    public MaskingResult Mask(string input, string mask)
    {
        if (!Regex.IsMatch(input, DiscordBotTokenPattern)) return MaskingResult.NoMatch;

        // Validate only the first section of the token

        var split = input.Split(".");

        var encodedUserId = split[0];
        encodedUserId = encodedUserId.Replace("-", "+").Replace("_", "/");
        if (encodedUserId.Length % 4 > 0)
        {
            var padding = 4 - encodedUserId.Length % 4;
            encodedUserId = encodedUserId.PadRight(encodedUserId.Length + padding, '=');
        }

        try
        {
            var decodedUserId = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUserId));

            if (Regex.IsMatch(decodedUserId, "[0-9]+"))
                return new MaskingResult
                {
                    Match = true,
                    Result = Regex.Replace(input, DiscordBotTokenPattern, mask)
                };
        }
        catch
        {
            // ignored
        }

        return MaskingResult.NoMatch;
    }
}