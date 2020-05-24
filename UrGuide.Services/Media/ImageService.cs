using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using UrGuide.Model.Shared;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Media
{
    class ImageService : IImageService
    {
        public ImageService(IWebHelper webHelper)
        {
            WebHelper = webHelper ?? throw new ArgumentNullException(nameof(webHelper));
        }

        public IWebHelper WebHelper { get; }

        private const int LandscapeImageWidth = 1080;
        private const int LandscapeImageHeight = 608;
        private const int PortraitImageWidth = 1080;
        private const int PortraitImageHeight = 1350;
        private const int AvatarWidth = 200;
        private const int AvatarHeight = 200;
        private readonly ImageFormatManager _formatManager = new ImageFormatManager();
        public void SaveImage(Data.Entities.Shared.Image imageFile)
        {
            var base64String = imageFile.ImageUrl.Split(',')[1];
            var base64Image = Convert.FromBase64String(base64String);
            using (var image = Image.Load(base64Image))
            {
                var size = image.Size();
                {
                    Image thumb;
                    Image imageToSave;
                    if (size.Width > size.Height) // image lanscape
                    {
                        imageToSave = image.Clone(x => x.Resize(new Size(LandscapeImageWidth, LandscapeImageHeight)));
                        thumb = image.Clone(x => x.Resize(new Size((LandscapeImageWidth / 3), (LandscapeImageHeight / 3))));
                    }
                    else // portrait
                    {
                        imageToSave = image.Clone(x => x.Resize(new Size(PortraitImageWidth, PortraitImageHeight)));
                        thumb = image.Clone(x => x.Resize(new Size((PortraitImageWidth / 3), (PortraitImageHeight / 3))));
                    }

                    string imageFileName = imageFile.Id + ".png";
                    EnsureImagePathExists();
                    var imagePath = System.IO.Path.Combine(WebHelper.ImageDirectoryPath, imageFileName);
                    var thumbPath = System.IO.Path.Combine(WebHelper.ThumbImageDirectoryPath, imageFileName);
                    imageToSave.Save(imagePath);
                    thumb.Save(thumbPath);
                    imageToSave.Dispose();
                    thumb.Dispose();
                    imageFile.ImageUrl = WebHelper.ResolveImageUrl(imageFileName);
                    var imageUrl = WebHelper.ResolveImageUrl(imageFileName);
                }
            }
        }

        private void EnsureImagePathExists()
        {
            if (!Directory.GetParent(WebHelper.ImageDirectoryPath).Exists)
            {
                Directory.CreateDirectory(Directory.GetParent(WebHelper.ImageDirectoryPath).FullName);
            }
            if (!Directory.Exists(WebHelper.ImageDirectoryPath))
            {
                Directory.CreateDirectory(WebHelper.ImageDirectoryPath);
            }

            if (!Directory.Exists(WebHelper.ThumbImageDirectoryPath))
            {
                Directory.CreateDirectory(WebHelper.ThumbImageDirectoryPath);
            }
        }

        public string SaveAvatar(string userId, ImageFileModel imageFile = null)
        {
            if(imageFile == null)
            {
                imageFile = new ImageFileModel
                {
                    ImageBase64 = Constants.UnknownImage
                };
            }
            var base64String = imageFile.ImageBase64.Split(',')[1];
            var base64Image = Convert.FromBase64String(base64String);
            using var image = Image.Load(base64Image);
            using Image avatar = image.Clone(x => x.Resize(new Size(AvatarWidth, AvatarHeight), AvatarHeight/2));
            string imageFileName = $"{userId}.png";

            EnsureImagePathExists();
            var imagePath = System.IO.Path.Combine(WebHelper.ImageDirectoryPath, imageFileName);
            avatar.Save(imagePath);
            return WebHelper.ResolveImageUrl(imageFileName);
        }

        public void DeleteImage(Data.Entities.Shared.Image image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            string imageFileName = image.Id + ".png";
            var imagePath = System.IO.Path.Combine(WebHelper.ImageDirectoryPath, imageFileName);
            var thumbPath = System.IO.Path.Combine(WebHelper.ThumbImageDirectoryPath, imageFileName);
            if (File.Exists(imagePath))
                File.Delete(imagePath);
            if (File.Exists(thumbPath))
                File.Delete(thumbPath);
        }

        public void DeleteImages(ICollection<Data.Entities.Shared.Image> images)
        {
            foreach (var image in images)
            {
                DeleteImage(image);
            }
        }
    }

    static class ImageServiceExtensions
    {
        // Implements a full image mutating pipeline operating on IImageProcessingContext
        public static IImageProcessingContext Resize(this IImageProcessingContext processingContext, Size size, float cornerRadius)
        {
            return processingContext.Resize(new ResizeOptions
            {
                Size = size,
                Mode = ResizeMode.Crop
            }).ApplyRoundedCorners(cornerRadius);
        }

        // This method can be seen as an inline implementation of an `IImageProcessor`:
        // (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)
        private static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
        {
            Size size = ctx.GetCurrentSize();
            IPathCollection corners = BuildCorners(size.Width, size.Height, cornerRadius);

            var graphicOptions = new GraphicsOptions
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // enforces that any part of this shape that has color is punched out of the background
            };
            

            return ctx.Fill(new ShapeGraphicsOptions { 
                GraphicsOptions = graphicOptions,
                ShapeOptions = new ShapeOptions { IntersectionRule = IntersectionRule.Nonzero }
            }, Color.LimeGreen, corners);
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the original around the center of the image

            float rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

            // move it across the width of the image - the width of the shape
            IPath cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }
}
