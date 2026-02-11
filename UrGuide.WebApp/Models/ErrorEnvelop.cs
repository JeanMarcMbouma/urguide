using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace UrGuide.WebApp.Models
{
    public class ErrorEnvelop<T>
    {
        public ErrorEnvelop(IEnumerable<T> errors)
        {
            Errors = errors ?? Enumerable.Empty<T>();
        }
        public IEnumerable<T> Errors { get; }

        
    }

    public static class ErrorEnvelop
    {
        public static ErrorEnvelop<T> Create<T>(IEnumerable<T> errors) => new ErrorEnvelop<T>(errors);
        public static ErrorEnvelop<string> Create(IEnumerable<IdentityError> errors) => new ErrorEnvelop<string>(errors.Select(x => x.Description));
        public static ErrorEnvelop<string> Create(ModelStateDictionary modelState) => new ErrorEnvelop<string>(modelState.SelectMany(x => x.Value?.Errors.Select(y => y.ErrorMessage) ?? Enumerable.Empty<string>()));
    }
}
