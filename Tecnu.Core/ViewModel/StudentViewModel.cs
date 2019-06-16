using FluentValidation;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tecnu.Core.ViewModel
{
    [Validator(typeof(StudentViewModelValidation))]
    public class StudentViewModel
    {
        [Display(Name = "Id")]
        public long? Id { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Date de Nascimento")]
        [DataType(DataType.Date)]
        public string DateBirth { get; set; }

        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        [Display(Name = "Telefone")]
        public string Telephone { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Informações Adicionais")]
        public string AdditionalInformation { get; set; }
    }

    public class StudentViewModelValidation : AbstractValidator<StudentViewModel>
    {
        public StudentViewModelValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("O campo Nome não pode estar vazio.");
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Cpf).Matches(new Regex(@"([0-9]{2}[\.]?[0-9]{3}[\.]?[0-9]{3}[\/]?[0-9]{4}[-]?[0-9]{2})|([0-9]{3}[\.]?[0-9]{3}[\.]?[0-9]{3}[-]?[0-9]{2})"));
            RuleFor(x => x.DateBirth).Matches(new Regex(@"([\d\/]+)")).WithMessage("O campo Data de Nascimento não pode estar vazio.");
            RuleFor(x => x.Telephone).NotEmpty().WithMessage("O campo Telefone não pode estar vazio.");
            RuleFor(x => x.Email).NotNull().WithMessage("O campo E-mail não pode estar vazio.");
            RuleFor(x => x.AdditionalInformation).NotNull().WithMessage("O campo E-mail não pode estar vazio.");
        }
    }
}