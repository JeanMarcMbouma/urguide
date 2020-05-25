using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UrGuide.Model.Posts;

namespace UrGuide.Services.Posts
{
    class PostPaginationValidator : AbstractValidator<PostPagination>
    {
        public PostPaginationValidator()
        {
            RuleFor(x => x.PageNumber).Must(x => x > 0).WithMessage("PageNumber must be greater than 0");
        }
    }
}
