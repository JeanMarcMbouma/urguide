using FluentValidation;
using UrGuide.Model.Posts;

namespace UrGuide.Services.Posts
{
    class SeatReservationModelValidator : AbstractValidator<SeatReservationModel>
    {
        public SeatReservationModelValidator()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.Seats).Must(r => r > 0).WithMessage("Seats must be greater than 0");
        }
    }
}
