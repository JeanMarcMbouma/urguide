using FluentValidation;
using UrGuide.Model;

namespace UrGuide.Services.Shared
{
    public class SetAttributeValidation : AbstractValidator<SetAttribute>
    {
        public SetAttributeValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Value).NotEmpty();
        }
    }
}
