using System;

namespace UrGuide.Data.Entities.Contracts
{
    public interface ILastUpdatableEntity : IEntity
    {
        DateTime LastUpdated { get; set; }
    }
}