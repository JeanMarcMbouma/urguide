using NetTopologySuite;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Threading.Tasks;
using UrGuide.Data.Entities.Contracts;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Extensions
{
    public static class GeoEntityExtensions
    {
        public static async Task SetLocationAsync(this IGeoEntity entity, IUserContext userContext, IIPStackService iPStackService)
        {
            if (userContext is null)
            {
                throw new ArgumentNullException(nameof(userContext));
            }

            if (iPStackService is null)
            {
                throw new ArgumentNullException(nameof(iPStackService));
            }

            var geo = await iPStackService.GetAsync(userContext.IPAddress);

            if (geo != null)
            {
                entity.Location = Convert(geo);
            }
        }

        public static async Task<Point> GetLocationAsync(this IIPStackService iPStackService, IUserContext userContext)
        {
            if (userContext is null)
            {
                throw new ArgumentNullException(nameof(userContext));
            }

            if (iPStackService is null)
            {
                throw new ArgumentNullException(nameof(iPStackService));
            }

            var geo = await iPStackService.GetAsync(userContext.IPAddress);

            if (geo != null)
            {
                return Convert(geo);
            }
            return null;
        }

        private static Point Convert(UrGuide.Shared.IPStackInfo geo)
        {
            //var epsg3857ProjectedCoordinateSystem = ProjectedCoordinateSystem.WebMercator;
            //var epsg4326GeographicCoordinateSystem = GeographicCoordinateSystem.WGS84;

            //var coordinateTransformationFactory = new CoordinateTransformationFactory();
            //var coordinateTransformation = coordinateTransformationFactory.CreateFromCoordinateSystems(epsg4326GeographicCoordinateSystem, epsg3857ProjectedCoordinateSystem);

            //var epsg4326Coordinate = new GeoAPI.Geometries.Coordinate(geo.Longitude, geo.Latitude);

            //var epsg3857Coordinate = coordinateTransformation.MathTransform.Transform(epsg4326Coordinate);

            return NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326)
                .CreatePoint(new Coordinate(geo.Longitude, geo.Latitude));
        }
    }
}
