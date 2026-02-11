using System;
using System.Collections.Generic;
using System.Linq;
using UrGuide.Core.Attributes;
using UrGuide.Data.Entities.Posts;
using UAttribute = UrGuide.Data.Entities.Users.AttributeTypes;
namespace UrGuide.Services.Extensions
{
    static class GenericAttributesExtensions
    {

        public static GenericAttribute? GetItem(this ICollection<GenericAttribute> attributes, UAttribute name)
        {
            var attr = attributes.FirstOrDefault(a => a.Name.Equals(Enum.GetName(name)));
            return attr;
        }

        public static GenericAttribute? GetItem(this ICollection<GenericAttribute> attributes, AttributeTypes name)
        {
            var attr = attributes.FirstOrDefault(a => a.Name.Equals(Enum.GetName(name)));
            return attr;
        }
    }
}
