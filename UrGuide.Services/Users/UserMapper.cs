using System.Linq;
using UrGuide.Services.Helpers;

namespace UrGuide.Services.Users
{
    public static class UserMapper
    {
        public static Model.Users.User ToUser(Data.Entities.Users.User source)
        {
            return new Model.Users.User
            {
                Id = source.Id,
                ProfileImage = source.ProfileImage?.ImageUrl,
                FullName = source.FullName?.ToString(),
                FirstName = source.FirstName,
                LastName = source.LastName,
                UserName = source.Attributes.First(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.UserName)),
                Gender = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Gender)),
                PhoneNumber = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Phone)),
                Twitter = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Twitter)),
                LinkedIn = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.LinkedIn)),
                FaceBook = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.FaceBook)),
                Google = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Google)),
                Instagram = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Instagram)),
                Rating = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Rating)),
                Address = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Address)),
                BirthDay = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.BirthDay)),
                City = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.City)),
                Country = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Country)),
                Description = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Description)),
                IsGuide = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.GuideOptIn)
                    && a.Value == Constants.Yes),
                IsPremium = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Subscription)
                    && a.Value == nameof(Subscriptions.Premium))
            };
        }

        public static Model.Users.UserInfo ToUserInfo(Data.Entities.Users.User source)
        {
            var ratingAttr = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Rating));
            return new Model.Users.UserInfo
            {
                Id = source.Id,
                ProfileImage = source.ProfileImage?.ImageUrl,
                FullName = source.FullName?.ToString(),
                FirstName = source.FirstName,
                LastName = source.LastName,
                Rating = ratingAttr != null ? (int)ratingAttr : 0,
                City = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.City)),
                Country = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Country)),
                Description = source.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Description))
            };
        }

        public static Model.Users.Notification ToNotification(Data.Entities.Users.Notification source)
        {
            return new Model.Users.Notification
            {
                Id = source.Id,
                AuthorImage = source.Sender?.Id == Constants.SystemUserId
                    ? $"thumbs/{Constants.SystemUserId}.png"
                    : source.Sender?.ProfileImage?.ImageUrl,
                AuthorId = source.Sender?.Id,
                Content = source.Content,
                ReferenceLink = source.ReferenceLink,
                Read = source.Read,
                Created = DateTimeHelper.GetDateTime(source.Created, System.DateTimeKind.Utc),
                IsSystem = source.IsSystem
            };
        }
    }
}
