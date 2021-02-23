using ACOMSaaS.NetCore.EFCore.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileConversion.Abstraction.Model
{
    public class MapperSourceText : Entity<int>
    {
        public string SourceText { get; set; }

        public virtual ICollection<InputMapping> InputMappings { get; set; }
    }
}
