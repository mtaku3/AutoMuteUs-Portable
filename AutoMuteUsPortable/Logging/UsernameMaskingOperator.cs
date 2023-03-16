using System;
using Serilog.Enrichers.Sensitive;

namespace AutoMuteUsPortable.Logging;

public class UsernameMaskingOperator : IMaskingOperator
{
    private readonly string _Username = Environment.UserName;

    public MaskingResult Mask(string input, string mask)
    {
        if (input.Contains(_Username))
            return new MaskingResult
            {
                Match = true,
                Result = input.Replace(_Username, mask)
            };
        return MaskingResult.NoMatch;
    }
}