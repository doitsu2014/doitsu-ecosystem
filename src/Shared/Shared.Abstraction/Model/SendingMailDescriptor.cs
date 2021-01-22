using System.Collections.Generic;

namespace Shared.Abstraction.Model
{
    public class SendingMailDescriptor
    {
        public string From { get; set; }

        public List<string> ToList { get; set; }

        public List<string> CcList { get; set; }

        public List<string> BccList { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public List<MailAttachment> Attachments { get; set; }
    }

    public class MailAttachment
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
    }
}