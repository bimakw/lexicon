using FluentValidation;
using Lexicon.Application.DTOs;

namespace Lexicon.Application.Validators;

public class CreatePostDtoValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Judul harus diisi, jangan dikosongkan.")
            .MaximumLength(200).WithMessage("Judul kepanjangan, maksimal 200 karakter saja.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Konten postingan harus ada isinya.");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID wajib diisi.");
    }
}

public class UpdatePostDtoValidator : AbstractValidator<UpdatePostDto>
{
    public UpdatePostDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Judul tidak boleh kosong saat update.")
            .MaximumLength(200).WithMessage("Maksimal 200 karakter untuk judul.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Isi konten jangan sampai hilang.");
    }
}


