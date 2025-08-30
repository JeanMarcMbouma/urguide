namespace UrGuide.MAUI.Models
{
    // Placeholder models that will be migrated from the original project
    public class PostItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Additional properties will be migrated
    }

    public class DiscoverItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        // Additional properties will be migrated
    }
}

namespace UrGuide.MAUI.Models.API
{
    public class PostCreationModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Additional properties will be migrated
    }
}