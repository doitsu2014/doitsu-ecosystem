using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Linq;
using System.Text;

namespace FileConversion.Core
{
    public static class Currency
    {
        private static decimal Convert(char[] firstGroup, char[][] groups, char[] fractionalGroup)
        {
            var builder = new[] {firstGroup}.Concat(groups)
                .Aggregate(new StringBuilder(64), (sb, x) =>
                {
                    sb.Append(x);
                    return sb;
                });
            if (fractionalGroup != null && fractionalGroup.Any())
            {
                builder.Append(".");
                builder.Append(fractionalGroup);
            }

            return decimal.Parse(builder.ToString());
        }

        static TextParser<char> DollarSymbol = Character.EqualTo('$');

        static TextParser<decimal> WithoutWholeNumberParser =
            from firstGroup in Character.Digit.AtMost(3)
            from groups in Character.EqualTo(',')
                .IgnoreThen(Character.Digit.Repeat(3)).Many()
            from fractional in Character.EqualTo('.')
                .IgnoreThen(Character.Digit.AtLeastOnce())
            select Convert(firstGroup, groups, fractional);

        static TextParser<decimal> WithoutFractionalParser =
            from firstGroup in Character.Digit.AtMost(3, true)
            from groups in Character.EqualTo(',')
                .IgnoreThen(Character.Digit.Repeat(3)).Many()
            from fractional in Character.EqualTo('.')
                .IgnoreThen(Character.Digit.Many()).OptionalOrDefault()
            select Convert(firstGroup, groups, fractional);

        public static TextParser<decimal> PositiveNumber =
            WithoutFractionalParser.Try().Or(WithoutWholeNumberParser);

        public static TextParser<decimal> Number =
            PositiveNumber.Between(Character.EqualTo('('), Character.EqualTo(')')).Select(pn => pn * -1)
                .Or(PositiveNumber.Select(pn => pn));

        public static TextParser<decimal> Money =
            DollarSymbol.IgnoreThen(Number).Or(
                Number.Then(DollarSymbol.OptionalOrDefault().Value));

        public static TextParser<decimal[]> MoneyList(char delimiter) =>
            Money.ManyDelimitedBy(Character.EqualTo(delimiter));
    }
}