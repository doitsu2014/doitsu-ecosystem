using System.Collections.Generic;
using LanguageExt;
using Shared.Validations;
using Xunit;
using static LanguageExt.Prelude;
using static Shared.Validations.StringValidator;
using static Shared.Validations.GenericValidator;

namespace Tests.Shared
{
    public class ValidationTest
    {

        [Fact]
        public void TestListGenericValidator()
        {
            var listData = new List<int>() {1, 2, 3, 4, 5};
            var listEmptyData = new List<object>();
            IEnumerable<object> listEmptyObject = null;
            object objectNull = null;
            var objectNotNull = (a: 1, b: 2);

            Assert.True(ShouldNotNullOrEmpty(listData).IsSuccess);
            Assert.True(ShouldNotNullOrEmpty(listEmptyData).IsFail);
            Assert.True(ShouldNotNullOrEmpty(listEmptyObject).IsFail);
            Assert.True(ShouldNotNullOrEmpty(listEmptyObject).IsFail);
        }

        [Fact]
        public void TestStringValidator_MinLength_MaxLength()
        {
            var emptyStr = string.Empty;
            var str1 = "1";
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