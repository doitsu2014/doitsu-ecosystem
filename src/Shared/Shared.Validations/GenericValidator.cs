using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Shared.Validations
{
    public class GenericValidator
    {
        public static Validation<string, IList<T>> ShouldNotNullOrEmpty<T>(IList<T> value, string failMessage) =>
            (value != null && value.Count > 0)
                ? Success<string, IList<T>>(value)
                : Fail<string, IList<T>>(failMessage);

        public static Validation<string, ICollection<T>> ShouldNotNullOrEmpty<T>(ICollection<T> value, string failMessage) =>
            (value != null && value.Count > 0)
                ? Success<string, ICollection<T>>(value)
                : Fail<string, ICollection<T>>(failMessage);

        public static Validation<string, IEnumerable<T>> ShouldNotNullOrEmpty<T>(IEnumerable<T> value, string failMessage) =>
            (value != null && value.Length() > 0)
                ? Success<string, IEnumerable<T>>(value)
                : Fail<string, IEnumerable<T>>(failMessage);
    }
}
