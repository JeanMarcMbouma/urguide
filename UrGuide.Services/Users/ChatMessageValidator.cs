using FluentValidation;
using UrGuide.Model.Messages;
using UrGuide.Services.Contracts;

namespace UrGuide.Services.Users
{
    class ChatMessageValidator : AbstractValidator<ChatMessage>
    {
        public ChatMessageValidator(IUserService userService)
        {
            RuleFor(x => x.To).NotEmpty().MustAsync(async (x, cancellationToken) =>
            {
                var r = await userService.ExistsAsync(x, cancellationToken);
                return r.Data;
            });
            RuleFor(x => x.Content).NotEmpty().MaximumLength(500);
        }
    }
}
