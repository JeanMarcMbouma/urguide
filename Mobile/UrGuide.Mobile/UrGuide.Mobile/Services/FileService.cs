using System;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using Xamarin.Essentials;

namespace UrGuide.Mobile.Services
{

    public class FileService : IFileService
    {
        public IObservable<IEnumerable<FileResult>> UploadImages()
        {
            return FilePicker.PickMultipleAsync(PickOptions.Images).ToObservable();
        }
    }
}
