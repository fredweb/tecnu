using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XNuvem.Web.Models
{
    public class UserEditViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Login", Description="Nome usado para efetuar o login")]
        public string UserName { get; set; }
        
        [MaxLength(100)]
        [Display(Name = "Nome completo")]
        public string FullName { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Senha")]
        [MaxLength(50)]
        [MinLength(4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirmar senha")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name="Vendedor", Description="Nenhum vendedor para não ter restrição de vendedor")]
        public int SlpCode { get; set; }

        [Display(Name="Nome vendedor")]
        public string SlpName { get; set; }

        [Display(Name="OK Alimentos")]
        public bool QryGroup60 { get; set; }

        [Display(Name = "X'TOSO")]
        public bool QryGroup61 { get; set; }

        [Display(Name = "Sabor da Bahia")]
        public bool QryGroup62 { get; set; }


    }

    public class ChangePasswordViewModel
    {
        [Display(Name = "Senha atual")]
        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Nova senha")]
        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirmar senha")]
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}