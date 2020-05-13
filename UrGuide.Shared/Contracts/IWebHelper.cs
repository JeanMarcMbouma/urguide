namespace UrGuide.Shared.Contracts
{
    public interface IWebHelper
    {
        string ResolveUrl(MessageTypes confirmation, object values);
        string ResolveUrl(string uri, object values);
        string ResolveImageUrl(string imageSeoName);
        string ResolveImageThumbUrl(string imageSeoName);
        string ImageDirectoryPath { get; }
        string ThumbImageDirectoryPath { get; }
    }
}
