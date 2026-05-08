using ArgentinaLightHouses.Data;
using ArgentinaLightHouses.Models;

namespace ArgentinaLightHouses.Tests;

public class LighthouseImageUrlTests
{
    private readonly List<Lighthouse> _lighthouses = LighthouseRepository.GetAll();

    // ── Model tests ────────────────────────────────────────────────────────────

    [Fact]
    public void Lighthouse_ImageUrl_DefaultsToNull()
    {
        var lh = new Lighthouse();
        Assert.Null(lh.ImageUrl);
    }

    [Fact]
    public void Lighthouse_ImageUrl_CanHoldValidAbsoluteUrl()
    {
        const string url = "https://upload.wikimedia.org/wikipedia/commons/some/image.jpg";
        var lh = new Lighthouse { ImageUrl = url };
        Assert.Equal(url, lh.ImageUrl);
    }

    // ── Repository tests ───────────────────────────────────────────────────────

    [Fact]
    public void GetAll_AtLeastOneLighthouseHasNonNullImageUrl()
    {
        Assert.Contains(_lighthouses, lh => lh.ImageUrl is not null);
    }

    [Fact]
    public void GetAll_AllNonNullImageUrlsAreWellFormedHttpsUris()
    {
        var withUrl = _lighthouses.Where(lh => lh.ImageUrl is not null).ToList();
        Assert.All(withUrl, lh =>
        {
            var isValid = Uri.TryCreate(lh.ImageUrl, UriKind.Absolute, out var uri)
                          && uri.Scheme == Uri.UriSchemeHttps;
            Assert.True(isValid,
                $"Lighthouse '{lh.Name}' has an invalid or non-https ImageUrl: '{lh.ImageUrl}'");
        });
    }

    [Fact]
    public void GetAll_NoLighthouseHasEmptyStringImageUrl()
    {
        Assert.All(_lighthouses, lh =>
            Assert.False(lh.ImageUrl == string.Empty,
                $"Lighthouse '{lh.Name}' has an empty string ImageUrl — must be null or a valid URL"));
    }

    // ── Edge case ──────────────────────────────────────────────────────────────

    [Fact]
    public void Lighthouse_NullImageUrl_DoesNotThrowWhenAccessed()
    {
        var lh = new Lighthouse { ImageUrl = null };
        var ex = Record.Exception(() => _ = lh.ImageUrl);
        Assert.Null(ex);
    }
}
