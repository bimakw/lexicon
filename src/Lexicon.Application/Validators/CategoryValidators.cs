using FluentValidation;
using Lexicon.Application.DTOs;

namespace Lexicon.Application.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama kategori tidak boleh kosong.")
            .MaximumLength(100).WithMessage("Nama kategori maksimal 100 karakter.");
    }
}

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama kategori wajib diisi untuk update.")
            .MaximumLength(100).WithMessage("Maksimal 100 karakter untuk nama kategori.");
    }
}


