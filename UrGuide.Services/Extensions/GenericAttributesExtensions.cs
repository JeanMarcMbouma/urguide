using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UrGuide.Data.Entities.Attributes;
using UrGuide.Data.Entities.Posts;
using UAttribute = UrGuide.Data.Entities.Users.AttributeTypes;
namespace UrGuide.Services.Extensions
{
    static class GenericAttributesExtensions
    {
        public static T Get<T> (this ICollection<GenericAttribute> attributes, string name, T defaultValue = default)
        {
            var attr = attributes.FirstOrDefault(a => a.Name.Equals(name));
            return attr != null ? (T)Convert.ChangeType(attr.Value, typeof(T)) : defaultValue;
        }

        public static T Get<T>(this ICollection<GenericAttribute> attributes, AttributeTypes name, T defaultValue = default)
        {
            var attr = attributes.FirstOrDefault(a => a.Name.Equals(Enum.GetName(typeof(AttributeTypes), name)));
            return attr != null ? (T)Convert.ChangeType(attr.Value, typeof(T)) : defaultValue;
        }

        public static T Get<T>(this ICollection<GenericAttribute> attributes, UAttribute name, T defaultValue = default)
        {
            var attr = attributes.FirstOrDefault(a => a.Name.Equals(Enum.GetName(typeof(UAttribute), name)));
            return attr != null ? (T)Convert.ChangeType(attr.Value, typeof(T)) : defaultValue;
        }

        public static GenericAttribute GetItem(this ICollection<GenericAttribute> attributes, UAttribute name)
        {
            var attr = attributes.FirstOrDefault(a => a.Name.Equals(Enum.GetName(typeof(UAttribute), name)));
            return attr;
        }

        public static GenericAttribute GetItem(this ICollection<GenericAttribute> attributes, AttributeTypes name)
        {
            var attr = attributes.FirstOrDefault(a => a.Name.Equals(Enum.GetName(typeof(AttributeTypes), name)));
            return attr;
        }
    }
}
