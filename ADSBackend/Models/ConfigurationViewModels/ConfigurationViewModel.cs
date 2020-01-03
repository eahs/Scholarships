using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models.ConfigurationViewModels
{
    public class ConfigurationViewModel
    {
        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Attachment File Path (Include trailing slash)")]
        public string AttachmentFilePath { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Privacy Policy URL")]
        public string PrivacyPolicyUrl { get; set; }

    }
}
