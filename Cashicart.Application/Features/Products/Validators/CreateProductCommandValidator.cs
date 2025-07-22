using Cashicart.Application.Common.Options;
using Cashicart.Application.Features.Products.Commands;
using Cashicart.Common.Options;
using FluentValidation;
using Microsoft.Extensions.Options;


namespace Cashicart.Application.Features.Products.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator(IOptions<LocalizationOptions> options)
        {
            var supportedLangs = options.Value.SupportedLanguages;

            RuleForEach(x => x.Translations).ChildRules(translation =>
            {
                translation.RuleFor(t => t.Language)
                    .NotEmpty().WithMessage("Language code is required.")
                    .Must(lang => supportedLangs.Contains(lang.ToLower()))
                    .WithMessage(t => $"Language '{t.Language}' is not supported.");

                translation.RuleFor(t => t.Name)
                    .NotEmpty().WithMessage("Name is required.");
            });
        }
    }
}
