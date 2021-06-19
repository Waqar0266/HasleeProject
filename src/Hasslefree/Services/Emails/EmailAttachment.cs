using System.IO;

namespace Hasslefree.Services.Emails
{
    public class EmailAttachment
    {
        public Stream Data { get; set; }
        public string Filename { get; set; }
    }
}
