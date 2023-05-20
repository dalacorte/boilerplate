using Business.Domain.Extensions;
using Business.Domain.Model;
using FluentValidation;

namespace Business.Domain.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(r => r.Username)
                .NotEmpty()
                    .WithMessage("Can't be empty")
                .MaximumLength(20)
                    .WithMessage("Too large")
                .MinimumLength(3)
                    .WithMessage("Too short");

            RuleFor(r => r.Password)
                .NotEmpty()
                    .WithMessage("Can't be empty")
                .MinimumLength(3)
                    .WithMessage("Too short")
                .Must(m => m.IsStrongPassword())
                    .WithMessage("Too easy");

            RuleFor(r => r.Email)
                .NotEmpty()
                    .WithMessage("Can't be empty")
                .Must(m => m.IsValidEmail())
                    .WithMessage("Not valid");

            RuleFor(r => r.Username)
                .NotEmpty()
                    .WithMessage("Can't be empty")
                .MaximumLength(50)
                    .WithMessage("Too large")
                .MinimumLength(3)
                    .WithMessage("Too short");

            RuleFor(r => r.ProfilePicture)
                .NotEmpty()
                    .WithMessage("Can't be empty")
                .Must(m => m.IsValidURL())
                    .WithMessage("Not valid");

            //RuleFor(r => r.CreatedDate)
            //    .GreaterThan(DateTime.Now)
            //        .WithMessage("Not today");
        }
    }
}
