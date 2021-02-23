using System.IO;
using System.Reflection;

namespace FileConversion.Abstraction
{
    public class BaseOutputModel
    {
        public virtual string ToStringWithDelimited(string delimited)
        {
            var result = "";
            var properties = this.GetType().GetProperties();
            var lengthOfProps = properties.Length;
            var count = 0;
            foreach (PropertyInfo propertyInfo in properties)
            {
                count += 1;
                delimited = count != lengthOfProps ? delimited : string.Empty;
                result += $"{propertyInfo.GetValue(this)}{delimited}";
            }
            return result;
        }
    }

}

