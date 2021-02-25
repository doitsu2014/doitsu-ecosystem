using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Linq;

namespace FileConversion.Core
{
    public static class Common
    {
        private static char[] newLineCharacters = new[] {'\r', '\n'};

        public static TextParser<char> NewLine =
            Span.EqualTo("\r\n").Value('\n').Try().Or(Character.In(newLineCharacters));

        public static TextParser<char> ExceptNewLine = Character.ExceptIn(newLineCharacters);

        public static TextParser<char> IgnoredLine = ExceptNewLine.Many().IgnoreThen(NewLine);

        public static TextParser<char> IgnoredNonEmptyLine = ExceptNewLine.AtLeastOnce().IgnoreThen(NewLine);

        public static TextParser<T[]> AtMost<T>(this TextParser<T> parser, int count, bool required = false)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            return input =>
            {
                var result = new T[count];
                var remainder = input;

                for (int i = 0; i < count; i++)
                {
                    var r = parser(remainder);
                    if (!r.HasValue)
                        return (required && i == 0)
                            ? Result.CastEmpty<T, T[]>(r)
                            : Result.Value(result.Take(i).ToArray(), input, remainder);

                    result[i] = r.Value;
                    remainder = r.Remainder;
                }

                return Result.Value(result.Take(count).ToArray(), input, remainder);
            };
        }

        public static TextParser<char> NonWhitespace =
            Character.Matching(c => !char.IsWhiteSpace(c), "non-whitespace");

        public static TextParser<string> LinePrefix(string prefix, bool includePrefix = false) =>
            Span.EqualTo(prefix).IgnoreThen(ExceptNewLine.Many().Then(NewLine.Value))
                .Select(chars => (includePrefix ? prefix : string.Empty) + new string(chars));

        public static TextParser<string[]> TrimmedWords =
            from arr in Character.WhiteSpace.Many().IgnoreThen(
                NonWhitespace.AtLeastOnce().AtLeastOnceDelimitedBy(
                    Character.WhiteSpace.AtLeastOnce()))
            select arr.Select(e => new string(e)).ToArray();

        public static TextParser<string> TrimmedLine =
            from arr in TrimmedWords
            select arr.Aggregate((x, y) => $"{x} {y}");
    }
}