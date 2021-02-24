using System;
using Shared.EntityFrameworkCore;

namespace FileConversion.Abstraction.Model
{
    public class InputMappingKey
    {
        public string Key { get; set; }
        public InputType InputType { get; set; }
    }

    public class InputMapping : Entity<InputMappingKey>
    {
        public string XmlConfiguration { get; set; }
        public StreamType StreamType { get; set; }
        public string Mapper { get; set; }
        public int? MapperSourceTextId { get; set; }
        public string Description { get; set; }
        public virtual MapperSourceText MapperSourceText { get; set; }
    }
}