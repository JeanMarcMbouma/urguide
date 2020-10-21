using Microsoft.EntityFrameworkCore;

namespace UrGuide.Core.Attributes
{
    [Owned]
    public class GenericAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static implicit operator string(GenericAttribute attribute)
        {
            return attribute?.Value;
        }

        public static implicit operator int(GenericAttribute attribute)
        {
            return int.TryParse(attribute.Value, out int val) ? val : 0;
        }

        public static implicit operator bool(GenericAttribute attribute)
        {
            if (attribute == null)
                return false;
            if (string.IsNullOrEmpty(attribute.Value)
                || attribute.Value.Equals(Constants.No)
                || attribute.Value.Equals("1")
                || attribute.Value.Equals(false.ToString(), System.StringComparison.OrdinalIgnoreCase))
                return false;
            return true;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}