using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XNuvem.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Nome do usuario")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Mantenha-me conectado")]
        public bool RememberMe { get; set; }
    }
}
