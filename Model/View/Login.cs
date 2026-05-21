using System.ComponentModel.DataAnnotations;

namespace RosATest.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Введите правильную электронную почту.")]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; } = "";
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";
    }
}