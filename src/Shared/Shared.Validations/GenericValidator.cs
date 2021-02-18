using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Shared.Validations
{
    public class GenericValidator
    {
        public static Validation<string, IList<T>> ShouldNotNullOrEmpty<T>(IList<T> value) =>
            (value != null && value.Count > 0)
                ? Success<string, IList<T>>(value)
                : Fail<string, IList<T>>("value is null or empty");

        public static Validation<string, ICollection<T>> ShouldNotNullOrEmpty<T>(ICollection<T> value) =>
            (value != null && value.Count > 0)
                ? Success<string, ICollection<T>>(value)
                : Fail<string, ICollection<T>>("value is null or empty");

        public static Validation<string, IEnumerable<T>> ShouldNotNullOrEmpty<T>(IEnumerable<T> value) =>
            (value != null && value.Length() > 0)
                ? Success<string, IEnumerable<T>>(value)
                : Fail<string, IEnumerable<T>>("value is null or empty");

        public static Validation<string, T> ShouldNotNull<T>(T value) =>
            (value != null)
                ? Success<string, T>(value)
                : Fail<string, T>("value is null");
    }
}