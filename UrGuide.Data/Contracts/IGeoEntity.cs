using NetTopologySuite.Geometries;

namespace UrGuide.Data.Entities.Contracts
{
    public interface IGeoEntity
    {
        Point Location { get; set; }
    }
}