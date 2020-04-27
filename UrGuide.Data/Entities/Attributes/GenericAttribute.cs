using Microsoft.EntityFrameworkCore;

namespace UrGuide.Data.Entities.Attributes
{
    [Owned]
    public class GenericAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static implicit operator string (GenericAttribute attribute)
        {
            return attribute.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}