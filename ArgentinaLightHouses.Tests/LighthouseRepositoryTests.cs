using ArgentinaLightHouses.Data;
using ArgentinaLightHouses.Models;

namespace ArgentinaLightHouses.Tests;

public class LighthouseRepositoryTests
{
    private readonly List<Lighthouse> _lighthouses = LighthouseRepository.GetAll();

    [Fact]
    public void GetAll_ReturnsNonNullList()
    {
        Assert.NotNull(_lighthouses);
    }

    [Fact]
    public void GetAll_ReturnsNonEmptyList()
    {
        Assert.NotEmpty(_lighthouses);
    }

    [Fact]
    public void GetAll_ReturnsExactly61Lighthouses()
    {
        Assert.Equal(61, _lighthouses.Count);
    }

    [Fact]
    public void GetAll_AllLighthousesHaveNonEmptyName()
    {
        Assert.All(_lighthouses, lh =>
            Assert.False(string.IsNullOrWhiteSpace(lh.Name),
                $"Lighthouse at ({lh.Latitude}, {lh.Longitude}) has an empty Name"));
    }

    [Fact]
    public void GetAll_AllLighthousesHaveNonEmptyLocation()
    {
        Assert.All(_lighthouses, lh =>
            Assert.False(string.IsNullOrWhiteSpace(lh.Location),
                $"Lighthouse '{lh.Name}' has an empty Location"));
    }

    [Fact]
    public void GetAll_AllLighthousesHaveNonEmptyDescription()
    {
        Assert.All(_lighthouses, lh =>
            Assert.False(string.IsNullOrWhiteSpace(lh.Description),
                $"Lighthouse '{lh.Name}' has an empty Description"));
    }

    [Fact]
    public void GetAll_AllLatitudesValidForArgentineTerritory()
    {
        // Argentina mainland runs ~-22 to -56; Argentine Antarctic Territory extends to ~-65.
        // Two lighthouses are in Antártida Argentina (Faro 1ro. de Mayo ~-64.3, Faro Esperanza ~-63.4).
        Assert.All(_lighthouses, lh =>
            Assert.InRange(lh.Latitude, -66.0, -22.0));
    }

    [Fact]
    public void GetAll_AllLongitudesValidForArgentineTerritory()
    {
        // Argentine territory (mainland + Atlantic + Antarctic) spans roughly -53 to -74 longitude.
        Assert.All(_lighthouses, lh =>
            Assert.InRange(lh.Longitude, -74.0, -53.0));
    }

    [Fact]
    public void GetAll_NoDuplicateCoordinates()
    {
        var coordinatePairs = _lighthouses
            .Select(lh => (lh.Latitude, lh.Longitude))
            .ToList();

        var distinctCount = coordinatePairs.Distinct().Count();
        Assert.Equal(coordinatePairs.Count, distinctCount);
    }

    [Fact]
    public void GetAll_AtLeastOneLighthouseInPatagonia()
    {
        // Patagonia is generally considered south of ~40°S
        var patagonian = _lighthouses.Where(lh => lh.Latitude < -40.0).ToList();
        Assert.NotEmpty(patagonian);
    }

    [Fact]
    public void GetAll_AtLeastOneLighthouseInTierraDelFuego()
    {
        // Tierra del Fuego lighthouses are generally south of -52°S
        var tierraDelFuego = _lighthouses.Where(lh => lh.Latitude < -52.0).ToList();
        Assert.NotEmpty(tierraDelFuego);
    }
}
