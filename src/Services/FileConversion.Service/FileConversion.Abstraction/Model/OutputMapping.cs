using ACOMSaaS.NetCore.EFCore.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FileConversion.Abstraction.Model
{
    public class OutputMapping : Entity
    {
        public InputType Key { get; set; }
        public string XmlConfiguration { get; set; }
        [DefaultValue(0)]
        public int NumberOfHeader { get; set; }
        [DefaultValue(0)]
        public int NumberOfFooter { get; set; }
    }
}
