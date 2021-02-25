using System.Collections.Generic;
using LanguageExt;
using Shared.Abstraction.Models.Types;
using static LanguageExt.Prelude;

namespace Shared.Validations
{
    public static class GenericValidator
    {
        public static Validation<Error, T[]> ShouldNotNullOrEmpty<T>(T[] value) =>
            (value != null && value.Length > 0)
                ? Success<Error, T[]>(value)
                : Fail<Error, T[]>("value is null or empty");

        public static Validation<Error, IList<T>> ShouldNotNullOrEmpty<T>(IList<T> value) =>
            (value != null && value.Count > 0)
                ? Success<Error, IList<T>>(value)
                : Fail<Error, IList<T>>("value is null or empty");

        public static Validation<Error, ICollection<T>> ShouldNotNullOrEmpty<T>(ICollection<T> value) =>
            (value != null && value.Count > 0)
                ? Success<Error, ICollection<T>>(value)
                : Fail<Error, ICollection<T>>("value is null or empty");

        public static Validation<Error, IEnumerable<T>> ShouldNotNullOrEmpty<T>(IEnumerable<T> value) =>
            (value != null && value.Length() > 0)
                ? Success<Error, IEnumerable<T>>(value)
                : Fail<Error, IEnumerable<T>>("value is null or empty");

        public static Validation<Error, T> ShouldNotNull<T>(T value) =>
            (value != null)
                ? Success<Error, T>(value)
                : Fail<Error, T>("value is null");
    }
}