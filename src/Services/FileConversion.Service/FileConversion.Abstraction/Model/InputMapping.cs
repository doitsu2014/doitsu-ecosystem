namespace FileConversion.Abstraction.Model
{
    public class InputMapping
    {
        public string Key { get; set; }
        public InputType InputType { get; set; }
        public string XmlConfiguration { get; set; }
        
        public StreamType StreamType { get; set; }
        public string Mapper { get; set; }
        public int? MapperSourceTextId { get; set; }
        public string Description { get; set; }
        public virtual MapperSourceText MapperSourceText { get; set; }
    }
}