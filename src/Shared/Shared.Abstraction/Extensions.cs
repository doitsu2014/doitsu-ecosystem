﻿using System.Collections.Generic;
using System.Linq;

namespace Shared.Abstraction
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
        public static bool IsNullOrWhitespace(this string value) => string.IsNullOrWhiteSpace(value);
        public static string ComposeStrings(this IEnumerable<string> value, string blur)
            => value.Aggregate((a, b) => $"{a}{blur}{b}");
    }
}