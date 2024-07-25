using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter email")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string email { get; set; } = null!;

        public string? username { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [Display(Name = "Password")]

        [DataType(DataType.Password)]
        public string password { get; set; } = null!;

        public int roleID { get; set; } = 0;

        public int adminUserID { get; set; } = 0;

        //[Required]
        [Display(Name = "User Type")]
        public int userType { get; set; } = 0;
    }
}
