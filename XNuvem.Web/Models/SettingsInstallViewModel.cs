using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XNuvem.Web.Models
{
    public class InstallViewModel
    {
        [Required]
        [Display(Name = "Connection string")]
        public string ConnectionString { get; set; }

        [Required]
        [MinLength(4)]
        [Display(Name = "Nome do usuário")]
        public string UserName { get; set; }

        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Compare("Password")]
        [Display(Name = "Confirmar senha")]
        public string ConfirmPassword { get; set; }
    }

    public class UpdateViewModel
    {
        public bool ReadyToUpdate { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}