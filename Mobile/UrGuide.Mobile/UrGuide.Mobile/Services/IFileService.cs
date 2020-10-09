using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace UrGuide.Mobile.Services
{
    public interface IFileService
    {
        IObservable<IEnumerable<FileResult>> UploadImages();
    }
}
