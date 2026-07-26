using ArgentinaLightHouses.Models;

namespace ArgentinaLightHouses.Tests;

public class WeatherRecordTests
{
    [Theory]
    [InlineData(-0.1, true)]
    [InlineData(0.0, true)]
    [InlineData(0.1, false)]
    public void IsFrost_UsesInclusiveFreezingThreshold(double temperatureCelsius, bool expected)
    {
        var record = new WeatherRecord { TemperatureCelsius = temperatureCelsius };

        Assert.Equal(expected, record.IsFrost);
    }

    [Theory]
    [InlineData(59.9, false)]
    [InlineData(60.0, true)]
    [InlineData(60.1, true)]
    public void IsHighWind_UsesInclusiveSixtyKilometersThreshold(double windSpeedKmh, bool expected)
    {
        var record = new WeatherRecord { WindSpeedKmh = windSpeedKmh };

        Assert.Equal(expected, record.IsHighWind);
    }

    [Theory]
    [InlineData(79, false)]
    [InlineData(80, true)]
    [InlineData(82, true)]
    [InlineData(83, false)]
    [InlineData(94, false)]
    [InlineData(95, true)]
    [InlineData(99, true)]
    [InlineData(100, false)]
    public void IsStorm_MatchesConfiguredWmoCodeRanges(int weatherCode, bool expected)
    {
        var record = new WeatherRecord { WeatherCode = weatherCode };

        Assert.Equal(expected, record.IsStorm);
    }

    [Fact]
    public void ExtremeWeatherCssClass_PrefersStormOverWindAndFrost()
    {
        var record = new WeatherRecord
        {
            TemperatureCelsius = 0,
            WindSpeedKmh = 60,
            WeatherCode = 95
        };

        Assert.Equal("alh-weather-storm", record.ExtremeWeatherCssClass);
    }

    [Fact]
    public void ExtremeWeatherCssClass_PrefersWindOverFrost_WhenNoStormExists()
    {
        var record = new WeatherRecord
        {
            TemperatureCelsius = 0,
            WindSpeedKmh = 60,
            WeatherCode = 79
        };

        Assert.Equal("alh-weather-wind", record.ExtremeWeatherCssClass);
    }

    [Fact]
    public void ExtremeWeatherCssClass_ReturnsFrost_WhenOnlyFrostExists()
    {
        var record = new WeatherRecord
        {
            TemperatureCelsius = 0,
            WindSpeedKmh = 59.9,
            WeatherCode = 79
        };

        Assert.Equal("alh-weather-frost", record.ExtremeWeatherCssClass);
    }

    [Fact]
    public void ExtremeWeatherCssClass_ReturnsEmpty_WhenNoExtremeWeatherExists()
    {
        var record = new WeatherRecord
        {
            TemperatureCelsius = 3,
            WindSpeedKmh = 20,
            WeatherCode = 3
        };

        Assert.Equal(string.Empty, record.ExtremeWeatherCssClass);
    }
}
