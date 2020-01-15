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

        [Required]
        [Display(Name = "Student Data File Path")]
        public string StudentDataFilePath { get; set; }


        [DataType(DataType.EmailAddress)]
        [Display(Name = "Application Email")]
        public string ApplicationEmail { get; set; } = "scholarship@eastonsd.org";

        [DataType(DataType.Password)]
        [Display(Name = "Application Email Password")]
        public string ApplicationEmailPassword { get; set; }

        public string RootWebPath { get; set; }
    }
}
