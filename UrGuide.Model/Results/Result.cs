using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UrGuide.Model.Results
{
    public abstract class Result
    {
        public static Result<TResult> Of<TResult>(TResult data = default) => new Result<TResult>(data);
        public struct EmptyStruct { }
        public static Result<EmptyStruct> Empty = Of(new EmptyStruct());
    }
    public class Result<T> : Result
    {
        public Result(T data)
        {
            Data = data;
            Errors = new List<string>();
        }

        public T Data { get; }
        public bool HasError => Errors.Any();
        public ICollection<string> Errors { get; }

        public Result<T> WithErrors(params string[] errors)
        {
            Array.ForEach(errors, Errors.Add);
            return this;
        }

        public Result<T> Combine<TSource>(Result<TSource> other)
        {
            if(other.HasError)
            {
                foreach (var error in other.Errors)
                {
                    Errors.Add(error);
                }
            }
            return this;
        }
    }
}
