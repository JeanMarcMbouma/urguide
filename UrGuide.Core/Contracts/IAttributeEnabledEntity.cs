using System.Collections.Generic;
using UrGuide.Core.Attributes;

namespace UrGuide.Core.Contracts
{
    public interface IAttributeEnabledEntity : IEntity
    {
        ICollection<GenericAttribute> Attributes { get; }
    }
}