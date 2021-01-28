using System.Runtime.Serialization;
using LanguageExt;

namespace Shared.Abstraction.Models.Types
{
    public class Error : NewType<Error, string>
    {
        public Error(string value) : base(value)
        {
        }
    }
}