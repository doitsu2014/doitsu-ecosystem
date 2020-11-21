using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;

namespace Utility.Extensions
{
    public static class FunctionalExtension
    {
        public static TResult Map<T, TResult>(this T input, Func<T, TResult> outputFunc) => outputFunc(input);
        public static Task<TResult> MapAsync<T, TResult>(this T input, Func<T, Task<TResult>> outputFunc) => outputFunc(input);
        public static async Task<TResult> MapAsync<T, TResult>(this Task<T> inputTask, Func<T, Task<TResult>> outputFunc) => await outputFunc((await inputTask));
    }
}
