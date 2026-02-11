using System.Collections.Generic;
using UrGuide.Data.Entities.Shared;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Contracts
{
    public interface IImageService
    {
        void SaveImage(Image imageFile);
        string SaveAvatar(string userId, ImageFileModel? imageFile = null);
        void DeleteImage(Image i);
        void DeleteImages(ICollection<Image> images);
    }
}
