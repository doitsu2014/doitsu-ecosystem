using System.ComponentModel;
using Shared.EntityFrameworkCore;

namespace FileConversion.Abstraction.Model
{
    public class OutputMapping
    {
        public InputType Id { get; set; }
        
        public bool IsXml { get; set; }
        public string XmlConfiguration { get; set; }
        [DefaultValue(0)]
        public int? NumberOfHeader { get; set; }
        [DefaultValue(0)]
        public int? NumberOfFooter { get; set; }
    }
}
