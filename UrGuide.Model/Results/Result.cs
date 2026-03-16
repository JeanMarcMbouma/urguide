using BbQ.Outcome;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UrGuide.Model.Results
{
    public static class Result
    {
        public static Outcome<TResult> Of<TResult>(TResult data = default) => Outcome<TResult>.From(data);
        public struct EmptyStruct { }
        public static Outcome<EmptyStruct> Empty => Of(new EmptyStruct());
    }

    public static class OutcomeResultExtensions
    {
        public static Outcome<T> WithErrors<T>(this Outcome<T> outcome, params string[] errors)
        {
            return Outcome<T>.FromErrors(errors.Cast<object>().ToList());
        }

        public static Outcome<T> Combine<T, TSource>(this Outcome<T> outcome, Outcome<TSource> other)
        {
            if (other.IsError)
            {
                var allErrors = outcome.IsError
                    ? outcome.Errors.Concat(other.Errors).ToList()
                    : other.Errors.ToList();
                return Outcome<T>.FromErrors(allErrors);
            }
            return outcome;
        }
    }
}
