namespace UrGuide.Model
{
    public class SearchParameters : PaginationParameters
    {
        public string Term { get; set; }
        public bool Nearby { get; set; }
    }
}
