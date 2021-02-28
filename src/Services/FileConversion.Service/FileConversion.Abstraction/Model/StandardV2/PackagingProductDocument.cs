namespace FileConversion.Abstraction.Model.StandardV2
{
    public class PackagingProductDocument : IStandardModel
    {
        public string Sku { get; set; } 
        public string Name { get; set; } 
        public string Price { get; set; } 
        public int Quantity { get; set; } 
        
        public string CategorySku { get; set; } 
        public string CategoryName { get; set; } 
    }
}