using NetTopologySuite.Geometries;

namespace Domain.Helpers;

public class LocationHelper
{
    public const short SRID = 4326;

    public static Point DefaultLocation => new(0, 0) { SRID = SRID };

    public static Point CreateLocationByCoordinates(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), latitude, "Latitude must be in range [-90, 90]");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), longitude, "Longitude must be in range [-180, 180]");

        return new Point(longitude, latitude) { SRID = SRID };
    }
}
