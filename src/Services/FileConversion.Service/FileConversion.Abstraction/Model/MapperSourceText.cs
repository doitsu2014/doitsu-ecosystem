using System;
using System.Collections.Generic;
using Shared.EntityFrameworkCore;

namespace FileConversion.Abstraction.Model
{
    public class MapperSourceText : Entity<Guid>
    {
        public string SourceText { get; set; }

        public virtual ICollection<InputMapping> InputMappings { get; set; }
    }
}
