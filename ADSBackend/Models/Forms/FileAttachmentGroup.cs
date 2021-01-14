using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models.Forms
{
    public class FileAttachmentGroup
    {
        [Key]
        public int FileAttachmentGroupId { get; set; }

        public int ProfileId { get; set; }  // Id of owner of file

        public List<FileAttachment> FileAttachments { get; set; }
    }
}
