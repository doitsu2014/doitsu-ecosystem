using System.Text;
using Shared.LanguageExt.Common;
using Xunit;

namespace Shared.Test.LanguageExt.Common;

public class StringExtensionTests
{
    [Fact]
    public void TestRandomString()
    {
        var randomString = new StringBuilder();
        randomString.AddRandomSpecialString(2);
        randomString.AddRandomNumber(1000, 9999);
        randomString.AddRandomAlphabet(5);
        randomString.AddRandomSpecialString(2);
        randomString.AddRandomAlphabet(5, lowerCase: true);

        var toStr = randomString.ToString();
        Assert.True(randomString.Length == 18);
    }
}