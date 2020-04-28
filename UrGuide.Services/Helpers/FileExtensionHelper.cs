namespace UrGuide.Services.Helpers
{
    class FileExtensionHelper
    {
        public static string GetImageMimeType(Model.Shared.ImageFileModel file) => GetImageMimeType(file.Name);
        
        public static string GetImageMimeType(string name)
        {
            var ext = System.IO.Path.GetExtension(name);
            return ext switch
            {
                "png" => "image/png",
                "jpg" => "image/jpg",
                "tif" => "image/tiff",
                "tiff" => "image/tiff",
                "webp" => "image/webp",
                "gif" => "image/gif",
                _ => "image/jpeg"
            };
        }
    }
}
