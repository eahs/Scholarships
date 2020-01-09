using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models.ConfigurationViewModels
{
    public class ConfigurationViewModel
    {
        public string ErrorMessage { get; set; }

        [Required]
        [Display(Name = "Attachment File Path")]
        public string AttachmentFilePath { get; set; }

        [Required]
        [Display(Name = "Transcript File Path")]
        public string TranscriptFilePath { get; set; }


        [DataType(DataType.Url)]
        [Display(Name = "Privacy Policy URL")]
        public string PrivacyPolicyUrl { get; set; }

        public string RootWebPath { get; set; }
    }
}
