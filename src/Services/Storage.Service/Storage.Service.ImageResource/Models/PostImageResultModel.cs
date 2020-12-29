using Storage.Service.ApplicationCore.Enums;

namespace Storage.Service.ImageResource.Models
{
    public class PostImageResultModel
    {
        public string Subject { get; set; }
        public string OriginalFileName { get; set; }
        public string Url { get; set; }
        public PostImageResultStatusEnum Status { get; set; }
    }
}