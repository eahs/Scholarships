using System;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models.Forms
{
    public class FileAttachment
    {
        [Key]
        public int FileAttachmentId { get; set; }
        public int FileAttachmentGroupId { get; set; }
        public string FileAttachmentUuid { get; set; } = System.Guid.NewGuid().ToString();
        public FileAttachmentGroup FileAttachmentGroup { get; set; }
        public string ContentType { get; set; }  // mime-type
        public string FileName { get; set; }
        public string FileSubPath { get; set; }  // Path relative to root file storage directory
        public long Length { get; set; }
        public string SecureFileName { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
