using FluentValidation;
using InMemoriam.Infraestructure.DTOs;

namespace InMemoriam.Infraestructure.Validators
{
    public class MemorialDtoValidator : AbstractValidator<MemorialDto>
    {
        public MemorialDtoValidator()
        {
            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("El slug es obligatorio.")
                .MaximumLength(120).WithMessage("El slug admite hasta 120 caracteres.")
                .WithErrorCode("MEM_SLUG");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("El nombre completo es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre completo admite hasta 200 caracteres.")
                .WithErrorCode("MEM_NAME");

            RuleFor(x => x.Visibility)
                .NotEmpty().WithMessage("La visibilidad es obligatoria.")
                .Must(v => v is "Private" or "LinkOnly")
                .WithMessage("Visibilidad inválida. Valores permitidos: Private, LinkOnly.")
                .WithErrorCode("MEM_VIS");

            RuleFor(x => x.OwnerUserId)
                .GreaterThan(0).WithMessage("OwnerUserId debe ser mayor a 0.")
                .WithErrorCode("MEM_OWNER");
        }
    }
}
