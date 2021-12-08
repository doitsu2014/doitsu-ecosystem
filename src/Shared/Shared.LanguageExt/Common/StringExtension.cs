using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt;

namespace Shared.LanguageExt.Common
{
    public static class StringExtension
    {
        public static string Join(this IEnumerable<string> seq, string separator = "")
            => new StringBuilder().AppendJoin(separator, seq).ToString();

        public static StringBuilder AddRandomNumber(this StringBuilder builder, int start, int stop)
        {
            // Generate a random number  
            Random random = new Random();
            // Any random integer   
            int num = random.Next(start, stop);
            return builder.Append(num);
        }

        public static StringBuilder AddRandomAlphabet(this StringBuilder builder, int size, bool lowerCase = false)
        {
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + (lowerCase ? 97 : 65))));
                builder.Append(ch);
            }

            return builder;
        }

        public static StringBuilder AddRandomSpecialString(this StringBuilder builder, int size)
        {
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                var charCode = int.MinValue;
                do
                {
                    charCode = Convert.ToInt32(Math.Floor(13 * random.NextDouble() + 33));
                } while (charCode == 34 || charCode == 39);

                ch = Convert.ToChar(charCode);
                builder.Append(ch);
            }

            return builder;
        }

        public static byte[] ToBytes(this string stringValue)
            => Encoding.UTF8.GetBytes(stringValue);

        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
        
        public static bool IsNullOrWhitespace(this string value) => string.IsNullOrWhiteSpace(value);

        public static string ComposeStrings(this IEnumerable<string> value, string blur)
            => value.Aggregate((a, b) => $"{a}{blur}{b}");

        public static string ComposeStrings(this Seq<string> value, string blur)
            => value.Aggregate((a, b) => $"{a}{blur}{b}");
    }
}