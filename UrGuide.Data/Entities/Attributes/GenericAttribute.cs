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

        public static implicit operator int(GenericAttribute attribute)
        {
            return int.TryParse(attribute.Value, out int val) ? val : 0;
        }


        public override string ToString()
        {
            return Value;
        }
    }
}