using AutoMapper;
using UrGuide.Mobile.API;

namespace UrGuide.Mobile.Mapping
{
    class UserProfile : Profile {
        public UserProfile()
        {
            CreateMap<UserInfo, Model.Users.UserInfo>()
                .ForMember(x => x.ProfileImage, x => x.MapFrom(y => $"{GlobalSetting.DefaultEndpoint}/{y.ProfileImage}"));
        }
    }
}
