using System;
using System.Linq;

namespace AutoMuteUsPortable.UI.Setup.Utils
{
    public static class RandomExtensions
    {
        public static string RandomString(this Random random, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
