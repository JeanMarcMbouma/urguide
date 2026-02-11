using AutoMapper;
using DataTourRequest = UrGuide.Data.Entities.Tour.TourRequest;
using DataTourRequestStatus = UrGuide.Data.Entities.Tour.TourRequestStatus;

namespace UrGuide.Services.Tour
{
    public class TourRequestMap : Profile
    {
        public TourRequestMap()
        {
            CreateMap<DataTourRequest, Model.Tour.TourRequestModel>()
                .ForMember(x => x.RequesterName, u => u.MapFrom(x => x.Requester != null ? $"{x.Requester.FirstName} {x.Requester.LastName}" : string.Empty))
                .ForMember(x => x.RegionName, u => u.MapFrom(x => x.Region != null ? x.Region.Name : string.Empty))
                .ForMember(x => x.CreatedAt, u => u.MapFrom(x => x.CreatedAt))
                .ForMember(x => x.UpdatedAt, u => u.MapFrom(x => x.UpdatedAt))
                .ForMember(x => x.Status, u => u.MapFrom(x => (Model.Tour.TourRequestStatus)x.Status));

            CreateMap<Model.Tour.CreateTourRequestModel, DataTourRequest>()
                .ForMember(x => x.TourRequestId, opt => opt.Ignore())
                .ForMember(x => x.RequesterId, opt => opt.Ignore())
                .ForMember(x => x.Requester, opt => opt.Ignore())
                .ForMember(x => x.Region, opt => opt.Ignore())
                .ForMember(x => x.Status, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.Status, u => u.MapFrom(x => DataTourRequestStatus.Open));
        }
    }
}