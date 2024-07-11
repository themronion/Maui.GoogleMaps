using System.Collections.Concurrent;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace Maui.GoogleMaps.iOS.Factories;

public sealed class DefaultImageFactory : IImageFactory
{
    private static readonly Lazy<DefaultImageFactory> _instance = new(() => new());

    public static DefaultImageFactory Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, UIImage> _cacheDictionary = [];

    private DefaultImageFactory()
    {
    }

    public UIImage ToUIImage(BitmapDescriptor bitmapDescriptor, IMauiContext mauiContext)
    {
        if (bitmapDescriptor.Id != null && _cacheDictionary.TryGetValue(bitmapDescriptor.Id, out var cachedBitmap))
        {
            return cachedBitmap;
        }

        var uiImage = GetUIImage(bitmapDescriptor, mauiContext);
        if (bitmapDescriptor.Id != null)
        {
            _cacheDictionary.TryAdd(bitmapDescriptor.Id, uiImage);
        }

        return uiImage;
    }

    private UIImage GetUIImage(BitmapDescriptor descriptor, IMauiContext mauiContext)
    {
        switch (descriptor.Type)
        {
            case BitmapDescriptorType.Default:
                return Google.Maps.Marker.MarkerImage(descriptor.Color.ToPlatform());

            case BitmapDescriptorType.Bundle:
                return UIImage.FromBundle(descriptor.BundleName);

            case BitmapDescriptorType.Stream:
                var stream = descriptor.Stream.Invoke();
                if (stream.CanSeek && stream.Position > 0)
                {
                    stream.Position = 0;
                }

                // Resize to screen scale
                return UIImage.LoadFromData(NSData.FromStream(stream), UIScreen.MainScreen.Scale);

            case BitmapDescriptorType.AbsolutePath:
                return UIImage.FromFile(descriptor.AbsolutePath);

            case BitmapDescriptorType.View:
                var iconView = descriptor.View();
                var image = Utils.ConvertMauiViewToImage(iconView, mauiContext);
                return image;

            default:
                return Google.Maps.Marker.MarkerImage(UIColor.Red);
        }
    }
}