using FluentValidation;
using InMemoriam.Infraestructure.DTOs;
using System.Globalization;

namespace InMemoriam.Infraestructure.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es requerido")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(30);

            RuleFor(x => x.Telephone)
                .MaximumLength(10).When(x => !string.IsNullOrWhiteSpace(x.Telephone))
                .Matches(@"^\d+$").When(x => !string.IsNullOrWhiteSpace(x.Telephone))
                .WithMessage("El teléfono debe ser numérico");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("La fecha de nacimiento es requerida")
                .Must(BeValidDate).WithMessage("Formato de fecha inválido. Use dd-MM-yyyy");
        }

        private bool BeValidDate(string value)
        {
            return DateTime.TryParseExact(
                value, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dt)
                   && dt >= new DateTime(1900, 1, 1)
                   && dt <= new DateTime(2100, 12, 31);
        }
    }
}
