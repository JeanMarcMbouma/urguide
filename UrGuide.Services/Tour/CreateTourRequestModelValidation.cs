using FluentValidation;
using System;
using UrGuide.Model.Tour;

namespace UrGuide.Services.Tour
{
    public class CreateTourRequestModelValidation : AbstractValidator<CreateTourRequestModel>
    {
        public CreateTourRequestModelValidation()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(200)
                .WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.PreferredDate)
                .NotEmpty()
                .WithMessage("Preferred date is required")
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Preferred date must be in the future");

            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0)
                .WithMessage("Maximum participants must be greater than 0")
                .LessThanOrEqualTo(50)
                .WithMessage("Maximum participants cannot exceed 50");

            RuleFor(x => x.MaxBudget)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maximum budget must be non-negative");

            RuleFor(x => x.RegionId)
                .NotEmpty()
                .WithMessage("Region is required");

            RuleFor(x => x.Tags)
                .MaximumLength(500)
                .WithMessage("Tags cannot exceed 500 characters");
        }
    }
}