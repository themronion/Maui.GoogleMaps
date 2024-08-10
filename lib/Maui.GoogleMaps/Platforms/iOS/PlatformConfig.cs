using Maui.GoogleMaps.iOS.Factories;

namespace Maui.GoogleMaps.iOS;

public sealed class PlatformConfig
{
    public IImageFactory ImageFactory { get; set; }

    public IImageFactory GetImageFactory()
    {
        return ImageFactory ?? DefaultImageFactory.Instance;
    }
}