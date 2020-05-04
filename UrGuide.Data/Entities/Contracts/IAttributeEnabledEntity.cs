using System.Collections.Generic;
using UrGuide.Data.Entities.Attributes;

namespace UrGuide.Data.Entities.Contracts
{
    public interface IAttributeEnabledEntity : IEntity
    {
        ICollection<GenericAttribute> Attributes { get; }
    }
}