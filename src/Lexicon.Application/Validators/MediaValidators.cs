using FluentValidation;
using Lexicon.Application.DTOs;

namespace Lexicon.Application.Validators;

public class UploadMediaDtoValidator : AbstractValidator<UploadMediaDto>
{
    public UploadMediaDtoValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name cannot exceed 255 characters");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required")
            .MaximumLength(100).WithMessage("Content type cannot exceed 100 characters");

        RuleFor(x => x.Size)
            .GreaterThan(0).WithMessage("File size must be greater than 0");
    }
}
