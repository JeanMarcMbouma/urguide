using UrGuide.Core.Contracts;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.Contracts
{
    public interface IUserOwnedEntity : IEntity
    {
        User User { get; }
    }
}