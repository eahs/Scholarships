using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models.ConfigurationViewModels
{
    public class ConfigurationViewModel
    {
        public string ErrorMessage { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Application Email")]
        public string ApplicationEmail { get; set; } = "scholarship@eastonsd.org";

        [DataType(DataType.Password)]
        [Display(Name = "Application Email Password")]
        public string ApplicationEmailPassword { get; set; }

        public string RootWebPath { get; set; }
    }
}
