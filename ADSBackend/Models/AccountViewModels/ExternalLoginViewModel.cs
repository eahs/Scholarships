using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
