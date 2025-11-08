using FluentValidation;
using InMemoriam.Infraestructure.DTOs;

namespace InMemoriam.Infraestructure.Validators
{
    public class MediaAssetDtoValidator : AbstractValidator<MediaAssetDto>
    {
        public MediaAssetDtoValidator()
        {
            RuleFor(x => x.MemorialId)
                .GreaterThan(0).WithMessage("MemorialId debe ser mayor a 0.")
                .WithErrorCode("MED_MEMID");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es obligatorio.")
                .MaximumLength(200).WithMessage("El título admite hasta 200 caracteres.")
                .WithErrorCode("MED_TITLE");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("La descripción admite hasta 1000 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithErrorCode("MED_DESC");

            RuleFor(x => x.Kind)
                .NotEmpty().WithMessage("El tipo (Kind) es obligatorio.")
                .WithErrorCode("MED_KIND");

            RuleFor(x => x.StorageKey)
                .NotEmpty().WithMessage("La clave de almacenamiento es obligatoria.")
                .MaximumLength(300).WithMessage("La clave de almacenamiento admite hasta 300 caracteres.")
                .WithErrorCode("MED_STOREKEY");

            RuleFor(x => x.Checksum)
                .NotEmpty().WithMessage("El checksum es obligatorio.")
                .MaximumLength(128).WithMessage("El checksum admite hasta 128 caracteres.")
                .WithErrorCode("MED_HASH");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("La fecha es obligatoria (yyyy-MM-dd).")
                .WithErrorCode("MED_DATE");
        }
    }

}
