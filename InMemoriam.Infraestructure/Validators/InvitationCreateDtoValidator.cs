using FluentValidation;
using InMemoriam.Infraestructure.DTOs;

namespace InMemoriam.Infraestructure.Validators
{
    public class InvitationCreateDtoValidator : AbstractValidator<InvitationCreateDto>
    {
        public InvitationCreateDtoValidator()
        {
            RuleFor(x => x.MemorialId).GreaterThan(0);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(r => r is "Owner" or "Admin" or "Editor" or "Viewer")
                .WithMessage("Role inválido. Valores permitidos: Owner, Admin, Editor, Viewer.");
        }
    }
}
