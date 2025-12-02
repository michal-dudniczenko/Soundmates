namespace Soundmates.Tests.Repositories;

using Soundmates.Infrastructure.Repositories.Utils;
using System;
using Xunit;

public class RepositoryUtilsTests
{
    // ---------------------------------------------------------------------
    // ValidateLimitOffset
    // ---------------------------------------------------------------------
    [Theory]
    [InlineData(1, 0)]
    [InlineData(10, 5)]
    [InlineData(100, 999)]
    public void ValidateLimitOffset_DoesNotThrow_ForValidValues(int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit, offset);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, 0)]
    [InlineData(-10, 5)]
    public void ValidateLimitOffset_Throws_ForNonPositiveLimit(int limit, int offset)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RepositoryUtils.ValidateLimitOffset(limit, offset)
        );
    }

    [Theory]
    [InlineData(1, -1)]
    [InlineData(5, -100)]
    public void ValidateLimitOffset_Throws_ForNegativeOffset(int limit, int offset)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RepositoryUtils.ValidateLimitOffset(limit, offset)
        );
    }

    // ---------------------------------------------------------------------
    // CalculateHaversineDistance
    // ---------------------------------------------------------------------
    [Fact]
    public void CalculateHaversineDistance_SamePoint_ReturnsZero()
    {
        double lat = 40.7128;
        double lon = -74.0060;

        double distance = RepositoryUtils.CalculateHaversineDistance(lat, lon, lat, lon);

        Assert.Equal(0, distance, 5); // allow small floating point tolerance
    }

    [Fact]
    public void CalculateHaversineDistance_KnownDistance_ReturnsApproximate()
    {
        // New York City to Los Angeles
        double nyLat = 40.7128;
        double nyLon = -74.0060;
        double laLat = 34.0522;
        double laLon = -118.2437;

        double distance = RepositoryUtils.CalculateHaversineDistance(nyLat, nyLon, laLat, laLon);

        // Actual distance ~3940 km
        Assert.InRange(distance, 3900, 4000);
    }

    [Fact]
    public void CalculateHaversineDistance_AntipodalPoints_ReturnsHalfEarthCircumference()
    {
        // Approx antipodes
        double lat1 = 0;
        double lon1 = 0;
        double lat2 = 0;
        double lon2 = 180;

        double distance = RepositoryUtils.CalculateHaversineDistance(lat1, lon1, lat2, lon2);

        // Half Earth's circumference ~20015 km
        Assert.InRange(distance, 20000, 20020);
    }
}

