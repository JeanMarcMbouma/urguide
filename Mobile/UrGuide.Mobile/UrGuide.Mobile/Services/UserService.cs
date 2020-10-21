using AutoMapper;
using Sharpnado.Presentation.Forms.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrGuide.Mobile.API;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Model.Results;

namespace UrGuide.Mobile.Services
{
    class UserService : IUserService
    {
        public UsersClient UsersClient { get; }
        public FeedbackClient FeedbackClient { get; }
        public CatalogsClient CatalogsClient { get; }
        public IMapper Mapper { get; }

        public UserService(UsersClient usersClient, FeedbackClient feedbackClient, CatalogsClient catalogsClient, IMapper mapper)
        {
            UsersClient = usersClient ?? throw new ArgumentNullException(nameof(usersClient));
            FeedbackClient = feedbackClient ?? throw new ArgumentNullException(nameof(feedbackClient));
            CatalogsClient = catalogsClient ?? throw new ArgumentNullException(nameof(catalogsClient));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public Result<bool> ChangePassword(Model.Users.ChangePasswordModel model)
        {
            return Result.Of(false).WithErrors("Not yet implemented");
        }

        public async Task<Model.Users.UserInfo> GetUserInfo(string id = null)
        {
            var user =  await UsersClient.InfoAsync(id);
            return Mapper.Map<Model.Users.UserInfo>(user);
        }

        public Result<bool> SaveProfile(Model.Users.UpdateGuideModel model)
        {
            return Result.Of(false).WithErrors("Not yet implemented");
        }

        public async Task<PageResult<Model.Shared.AuthoredFeedback>> GetUserFeedback(string userId, int pageNumber)
        {
            var items = await FeedbackClient.UsersAsync(userId, pageNumber);
            return new PageResult<Model.Shared.AuthoredFeedback>(items.ItemsCount,
                Mapper.Map<IEnumerable<Model.Shared.AuthoredFeedback>>(items.Items).ToList());
        }

        public async Task<IEnumerable<GalleryItem>> GetGalleryItems(string userId)
        {
            var items = await CatalogsClient.AllAsync(userId);
            return Mapper.Map<IEnumerable<GalleryItem>>(items).ToList();
        }
    }
}
