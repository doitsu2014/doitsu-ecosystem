using Xunit;
using static Shared.LanguageExt.Validators.StringValidator;

namespace Shared.Test.LanguageExt.Validators
{
    public class StringValidatorTests
    {
        [Fact]
        public void TestStringValidator_MinLength_MaxLength()
        {
            var emptyStr = string.Empty;
            var str2 = "123";
            var str3 = "12345";

            var case1 = MinStrLength(1);
            var case2 = MaxStrLength(3);

            Assert.True(case1(emptyStr).IsFail);
            Assert.True(case2(str3).IsFail);
            Assert.True((case1(str2) | case2(str2)).IsSuccess);
        }
    }
}