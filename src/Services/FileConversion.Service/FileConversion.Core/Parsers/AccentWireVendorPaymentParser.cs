using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using ACOMSaaS.NetCore.Abstractions.Parser;
using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface.Parsers;
using Optional;
using Optional.Linq;
using Superpower;
using Superpower.Parsers;

namespace FileConversion.Core.Parsers
{
    public class AccentWireVendorPaymentParser : ICustomParser<VendorPayment>
    {
        public string Key => "accent.wire.custom";

        public Option<ImmutableList<VendorPayment>, string> Parse(byte[] content)
        {
            var stringContent = Encoding.UTF8.GetString(content);
            var result = AccentWireSpParser.VendorPaymentParser
                .AtLeastOnceDelimitedBy(Common.NewLine.Repeat(3))
                .TryParse(stringContent);
            return result.HasValue ?
                Option.Some<ImmutableList<VendorPayment>, string>(result.Value.ToImmutableList()) :
                Option.None<ImmutableList<VendorPayment>, string>(
                    result.ToString().Replace("\r", "\\r").Replace("\n", "\\n"));
        }

        private class AccentWireSpParser
        {
            private static char[] digits = Enumerable.Range(0, 10).Select(i => (char)('0' + i)).ToArray();

            private static TextParser<char> BackSlash { get; } = Character.EqualTo('/');

            private static TextParser<int> IntDigits(int count) =>
                Character.Digit.Repeat(count).Select(chars => int.Parse(new string(chars)));

            private static TextParser<int> IntDigitsUpTo(int count) =>
                from chars in Character.Digit.AtMost(count)
                select int.Parse(new string(chars));

            public static TextParser<DateTime> SimpleDateParser =
                from month in IntDigitsUpTo(2)
                from day in BackSlash.IgnoreThen(IntDigitsUpTo(2))
                from year in BackSlash.IgnoreThen(IntDigits(4))
                select new DateTime(year, month, day);

            public static TextParser<string> CheckNumberParser =
                from parts in Character.Matching(c => !char.IsWhiteSpace(c) && !char.IsControl(c), "")
                    .Many().ManyDelimitedBy(Character.EqualTo(' '))
                select new string(parts[parts.Length - 1]);

            public static TextParser<Invoice> InvoiceParser =
                from invoiceNumber in Character.Digit.AtLeastOnce().Select(arr => new string(arr))
                from invoiceDate in Character.WhiteSpace.IgnoreThen(SimpleDateParser)
                from amounts in Character.WhiteSpace.IgnoreThen(
                    Currency.Money.ManyDelimitedBy(Character.EqualTo(' ')))
                select new Invoice
                {
                    InvoiceNumber = invoiceNumber,
                    InvoiceDate = invoiceDate,
                    GrossAmount = amounts[0],
                    Discount = amounts[2],
                    PaidNetAmount = amounts[3]
                };

            static TextParser<string> IgnoredRemittanceNotification =
                from _ in Common.NewLine.Repeat(2)
                from __ in Common.LinePrefix("* * *").Or(Common.NewLine.Value(""))
                select string.Empty;

            static TextParser<string> IgnoredSummary =
                from _ in IgnoredRemittanceNotification
                from __ in SimpleDateParser.Then(Character.WhiteSpace.Value)
                    .Then(Currency.Money.Value).Then(Common.NewLine.Value)
                from ___ in Common.IgnoredNonEmptyLine.AtLeastOnce()
                select string.Empty;

            public static TextParser<Address> AddressParser =
                from line1 in Common.ExceptNewLine.Many()
                from _ in Common.NewLine
                from arr in Character.ExceptIn(' ', '\r', '\n').AtLeastOnce()
                    .AtLeastOnceDelimitedBy(Character.EqualTo(' ').AtLeastOnce())
                    .Select(ar => ar.Select(e => new string(e)).ToArray())
                select new Address
                {
                    Line1 = new string(line1),
                    City = arr.Take(arr.Length - 2).Aggregate((x, y) => $"{x} {y}"),
                    StateProvince = arr[arr.Length - 2],
                    ZipCode = arr[arr.Length - 1]
                };

            static TextParser<Invoice[]> InvoiceListParser =
                from _ in Common.LinePrefix("Our Voucher Number")
                from payments in InvoiceParser.Try().ManyDelimitedBy(Common.NewLine)
                from __ in Common.NewLine
                from ___ in Currency.Money.Then(Character.WhiteSpace.Value).Repeat(4)
                select payments;

            public static TextParser<VendorPayment> VendorPaymentParser =
                from vendorId in Common.LinePrefix("Vendor ID ")
                from checkNumber in Common.IgnoredLine.IgnoreThen(CheckNumberParser)
                from payments in Common.NewLine.IgnoreThen(InvoiceListParser)
                from _ in IgnoredSummary
                from vendorName in Common.NewLine.IgnoreThen(Common.ExceptNewLine.Many())
                    .Select(chars => new string(chars))
                from address in Common.NewLine.IgnoreThen(AddressParser)
                from i in Character.AnyChar.Many()
                select new VendorPayment
                {
                    VendorId = vendorId,
                    Number = checkNumber,
                    Invoices = payments,
                    VendorName = vendorName,
                    Address = address
                };
        }
    }
}
