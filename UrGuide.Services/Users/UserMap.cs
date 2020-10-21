using AutoMapper;
using System.Linq;
using UrGuide.Services.Helpers;

namespace UrGuide.Services.Users
{
    class UserMap : Profile
    {
        public UserMap()
        {
            CreateMap<Data.Entities.Users.User, Model.Users.User>()
                .ForMember(u => u.ProfileImage, x => x.MapFrom(f => f.ProfileImage.ImageUrl))
                .ForMember(u => u.FullName, x => x.MapFrom(f => f.FullName))
                .ForMember(u => u.UserName, x => x.MapFrom(f => f.Attributes.First(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.UserName))))
                 .ForMember(u => u.Gender, x => x.MapFrom(f => f.Attributes.First(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Gender))))
                .ForMember(u => u.PhoneNumber, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Phone))))
                .ForMember(u => u.Twitter, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Twitter))))
                .ForMember(u => u.LinkedIn, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.LinkedIn))))
                .ForMember(u => u.FaceBook, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.FaceBook))))
                .ForMember(u => u.Google, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Google))))
                .ForMember(u => u.Instagram, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Instagram))))
                .ForMember(u => u.Rating, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Rating))))
                .ForMember(u => u.Address, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Address))))
                .ForMember(u => u.FirstName, x => x.MapFrom(f => f.FirstName))
                .ForMember(u => u.LastName, x => x.MapFrom(f => f.LastName))
                .ForMember(u => u.BirthDay, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.BirthDay))))
                .ForMember(u => u.City, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.City))))
                .ForMember(u => u.Country, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Country))))
                .ForMember(u => u.Description, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Description))))
                .ForMember(u => u.Gender, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Gender))))
                .ForMember(u => u.IsGuide, x => x.MapFrom(f =>
                    f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.GuideOptIn)
                    && a.Value == Constants.Yes)))
                .ForMember(u => u.IsPremium, x => x.MapFrom(f =>
                    f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Subscription)
                    && a.Value == nameof(Subscriptions.Premium))));

            CreateMap<Data.Entities.Users.User, Model.Users.UserInfo>()
                .ForMember(u => u.ProfileImage, x => x.MapFrom(f => f.ProfileImage.ImageUrl))
                .ForMember(u => u.FullName, x => x.MapFrom(f => f.FullName))
                .ForMember(u => u.Rating, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Rating))))
                .ForMember(u => u.FirstName, x => x.MapFrom(f => f.FirstName))
                .ForMember(u => u.LastName, x => x.MapFrom(f => f.LastName))
                .ForMember(u => u.City, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.City))))
                .ForMember(u => u.Country, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Country))))
                .ForMember(u => u.Description, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Description))));

            CreateMap<Data.Entities.Users.Notification, Model.Users.Notification>()
                .ForMember(x => x.AuthorImage, u => u.MapFrom(x => x.Sender.Id == Constants.SystemUserId ? $"thumbs/{Constants.SystemUserId}.png" : x.Sender.ProfileImage.ImageUrl))
                .ForMember(x => x.AuthorId, u => u.MapFrom(x => x.Sender.Id))
                .ForMember(x => x.Content, u => u.MapFrom(x => x.Content))
                .ForMember(x => x.ReferenceLink, u => u.MapFrom(x => x.ReferenceLink))
                .ForMember(x => x.Read, u => u.MapFrom(x => x.Read))
                .ForMember(x => x.Created, u => u.MapFrom(x => DateTimeHelper.GetDateTime(x.Created, System.DateTimeKind.Utc)))
                .ForMember(x => x.IsSystem, u => u.MapFrom(x => x.IsSystem));
        }
    }
}
