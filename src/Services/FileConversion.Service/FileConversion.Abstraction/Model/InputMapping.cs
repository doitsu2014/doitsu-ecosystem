using ACOMSaaS.NetCore.EFCore.Abstractions.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FileConversion.Abstraction.Model
{
    public class InputMapping : Entity
    {
        public string Key { get; set; }
        public InputType InputType { get; set; }

        public string XmlConfiguration { get; set; }
        public StreamType StreamType { get; set; }
        public string Mapper { get; set; }
        public Nullable<int> MapperSourceTextId { get; set; }
        public string Description { get; set; }

        public virtual MapperSourceText MapperSourceText {get;set;}
}
}
