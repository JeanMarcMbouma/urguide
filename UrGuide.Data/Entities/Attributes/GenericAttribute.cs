using Microsoft.EntityFrameworkCore;

namespace UrGuide.Data.Entities.Attributes
{
    [Owned]
    public class GenericAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}