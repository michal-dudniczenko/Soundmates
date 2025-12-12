namespace Soundmates.Infrastructure.Repositories.Utils;

public static class RepositoryUtils
{
    public static void ValidateLimitOffset(int limit, int offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
    }

    public static double CalculateHaversineDistance(double originLat, double originLon, double destLat, double destLon)
    {
        const double earthRadiusKm = 6371.0;
        double originLatRad = originLat * (Math.PI / 180.0);
        double originLonRad = originLon * (Math.PI / 180.0);
        double destLatRad = destLat * (Math.PI / 180.0);
        double destLonRad = destLon * (Math.PI / 180.0);

        // Haversine formula
        double dLat = (destLatRad - originLatRad) / 2.0;
        double dLon = (destLonRad - originLonRad) / 2.0;
        double a = Math.Pow(Math.Sin(dLat), 2.0) +
                   Math.Cos(originLatRad) * Math.Cos(destLatRad) *
                   Math.Pow(Math.Sin(dLon), 2.0);

        double c = 2.0 * Math.Asin(Math.Sqrt(a));
        return earthRadiusKm * c;
    }
}
