using AutoMapper;
using System.Linq;

namespace UrGuide.Services.Users
{
    class UserMap : Profile
    {
        public UserMap()
        {
            CreateMap<Data.Entities.Users.User, Model.Users.User>()
                .ForMember(u => u.ProfileImage, x => x.MapFrom(f => f.ProfileImage.ImageBase64))
                .ForMember(u => u.FullName, x => x.MapFrom(f => f.FullName))
                .ForMember(u => u.UserName, x => x.MapFrom(f => f.Attributes.First(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.UserName))))
                .ForMember(u => u.PhoneNumber, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Phone))))
                .ForMember(u => u.Twitter, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Twitter))))
                .ForMember(u => u.LinkedIn, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.LinkedIn))))
                .ForMember(u => u.FaceBook, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.FaceBook))))
                .ForMember(u => u.Google, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Google))))
                .ForMember(u => u.Instagram, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Instagram))))
                .ForMember(u => u.Rating, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Rating))))
                .ForMember(u => u.Address, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Address))))
                .ForMember(u => u.FirstName, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.FirstName))))
                .ForMember(u => u.LastName, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.LastName))))
                .ForMember(u => u.BirthDay, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.BirthDay))))
                .ForMember(u => u.City, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.City))))
                .ForMember(u => u.Country, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Country))))
                .ForMember(u => u.Description, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Description))))
                .ForMember(u => u.IsGuide, x => x.MapFrom(f =>
                    f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.GuideOptIn)
                    && a.Value == Constants.Yes)))
                .ForMember(u => u.IsPremium, x => x.MapFrom(f =>
                    f.Attributes.FirstOrDefault(a => a.Name == nameof(Data.Entities.Users.AttributeTypes.Subscription)
                    && a.Value == nameof(Subscriptions.Premium))));


        }
    }
}
