using System;
using System.Collections.Generic;
using Shared.Validations;
using Xunit;

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

            Assert.True(GenericValidator.ShouldNotNullOrEmpty(listData, "listData should not be null or empty.").IsSuccess);
            Assert.True(GenericValidator.ShouldNotNullOrEmpty(listEmptyData, "listEmptyData should not be null or empty.").IsFail);
            Assert.True(GenericValidator.ShouldNotNullOrEmpty(listEmptyObject, "listEmptyObject should not be null or empty.").IsFail);
            Assert.True(GenericValidator.ShouldNotNullOrEmpty(listEmptyObject, "listEmptyObject should not be null or empty.").IsFail);
        }

        [Fact]
        public void TestStringValidator_MinLength_MaxLength()
        {
            var emptyStr = string.Empty;
            var str1 = "1";
            var str2 = "123";
            var str3 = "12345";

            var case1 = StringValidator.MinStrLength(1);
            var case2 = StringValidator.MaxStrLength(3);

            Assert.True(case1(emptyStr).IsFail);
            Assert.True(case2(str3).IsFail);
            Assert.True((case1(str2) | case2(str2)).IsSuccess);
        }
    }
}