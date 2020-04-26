namespace UrGuide.Shared.Contracts
{
    public interface IWebHelper
    {
        string ResolveUrl(MessageTypes confirmation, object values);
        string ResolveUrl(string uri, object values);
    }
}
