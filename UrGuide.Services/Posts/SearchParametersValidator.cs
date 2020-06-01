using FluentValidation;
using UrGuide.Model;

namespace UrGuide.Services.Posts
{
    class SearchParametersValidator : AbstractValidator<SearchParameters>
    {
        public SearchParametersValidator()
        {
            RuleFor(x => x.PageNumber).Must(x => x > 0).WithMessage("PageNumber must be greater than 0");
        }
    }
    class PaginationParameterValidator : AbstractValidator<PaginationParameters>
    {
        public PaginationParameterValidator()
        {
            RuleFor(x => x.PageNumber).Must(x => x > 0).WithMessage("PageNumber must be greater than 0");
        }
    }
}
