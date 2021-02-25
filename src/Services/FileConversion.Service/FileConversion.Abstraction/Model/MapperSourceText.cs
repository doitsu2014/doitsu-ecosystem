using System;
using System.Collections.Generic;
using Shared.EntityFrameworkCore;

namespace FileConversion.Abstraction.Model
{
    public class MapperSourceText
    {
        public int Id { get; set; }
        public string SourceText { get; set; }
        public virtual ICollection<InputMapping> InputMappings { get; set; }
    }
}
