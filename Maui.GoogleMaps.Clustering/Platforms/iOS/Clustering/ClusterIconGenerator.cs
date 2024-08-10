// Original code from https://github.com/mierzynskim/Xamarin.Forms.GoogleMaps.Clustering/
// Original author code from https://github.com/sferhah
using CoreGraphics;
using Foundation;
using Google.Maps.Utils;
using UIKit;
using Maui.GoogleMaps.iOS.Factories;
using Microsoft.Maui.Platform;
using Maui.GoogleMaps.Clustering.Logics;

namespace Maui.GoogleMaps.Clustering.Platforms.iOS.Clustering
{
    internal class ClusterIconGenerator : GMUDefaultClusterIconGenerator
    {
        private readonly NSCache iconCache;
        private readonly ClusterOptions options;
        private readonly IMauiContext mauiContext;

        public ClusterIconGenerator(ClusterOptions options, IMauiContext mauiContext)
        {
            iconCache = new NSCache();
            this.options = options;
            this.mauiContext = mauiContext;
        }

        public override UIImage IconForSize(nuint size)
        {
            string text;
            nuint bucketIndex = 0;

            if (options.EnableBuckets)
            {
                var buckets = options.Buckets;
                bucketIndex = BucketIndexForSize((nint)size);
                text = size < (nuint)buckets[0] ? size.ToString() : $"{buckets[bucketIndex]}+";
            }
            else
            {
                text = size.ToString();
            }

            return options.RendererCallback != null
                ? DefaultImageFactory.Instance.ToUIImage(options.RendererCallback(text), mauiContext)
                : options.RendererImage != null
                ? GetIconForText(text, DefaultImageFactory.Instance.ToUIImage(options.RendererImage, mauiContext))
                : GetIconForText(text, bucketIndex);
        }

        private nuint BucketIndexForSize(nint size)
        {
            uint index = 0;
            var buckets = options.Buckets;

            while (index + 1 < buckets.Length && buckets[index + 1] <= size)
            {
                ++index;
            }

            return index;
        }

        private UIImage GetIconForText(string text, UIImage baseImage)
        {
            var nsText = new NSString(text);
            var icon = iconCache.ObjectForKey(nsText);
            if (icon != null)
            {
                return (UIImage)icon;
            }

            var size = baseImage.Size;

            UIGraphics.BeginImageContextWithOptions(size, false, 0.0f);
            baseImage.Draw(new CGRect(0, 0, size.Width, size.Height));
            var rect = new CGRect(0, 0, baseImage.Size.Width, baseImage.Size.Height);

            var attributes = new UIStringAttributes(NSDictionary.FromObjectsAndKeys(
                objects: [UIFont.BoldSystemFontOfSize(12), NSParagraphStyle.Default, options.RendererTextColor.ToPlatform()],
                keys: [UIStringAttributeKey.Font, UIStringAttributeKey.ParagraphStyle, UIStringAttributeKey.ForegroundColor]
            ));

            var textSize = nsText.GetSizeUsingAttributes(attributes);
            var textRect = CGRectExtensions.Inset(rect, (rect.Size.Width - textSize.Width) / 2, (rect.Size.Height - textSize.Height) / 2);
            nsText.DrawString(CGRectExtensions.Integral(textRect), attributes);

            var newImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            iconCache.SetObjectforKey(newImage, nsText);
            return newImage;
        }

        private UIImage GetIconForText(string text, nuint bucketIndex)
        {
            var nsText = new NSString(text);
            var icon = iconCache.ObjectForKey(nsText);
            if (icon != null)
            {
                return (UIImage)icon;
            }

            var attributes = new UIStringAttributes(NSDictionary.FromObjectsAndKeys(
                objects: [UIFont.BoldSystemFontOfSize(14), NSParagraphStyle.Default, options.RendererTextColor.ToPlatform()],
                keys: [UIStringAttributeKey.Font, UIStringAttributeKey.ParagraphStyle, UIStringAttributeKey.ForegroundColor]
            ));

            var textSize = nsText.GetSizeUsingAttributes(attributes);
            var rectDimension = Math.Max(20, Math.Max(textSize.Width, textSize.Height)) + 3 * bucketIndex + 6;
            var rect = new CGRect(0.0f, 0.0f, rectDimension, rectDimension);

            UIGraphics.BeginImageContext(rect.Size);
            UIGraphics.BeginImageContextWithOptions(rect.Size, false, 0);

            var ctx = UIGraphics.GetCurrentContext();
            ctx.SaveState();

            bucketIndex = (nuint)Math.Min((int)bucketIndex, options.BucketColors.Length - 1);
            var backColor = options.BucketColors[bucketIndex];
            ctx.SetFillColor(backColor.ToCGColor());
            ctx.FillEllipseInRect(rect);
            ctx.RestoreState();

            UIColor.White.SetColor();
            var textRect = rect.Inset((rect.Size.Width - textSize.Width) / 2, (rect.Size.Height - textSize.Height) / 2);
            nsText.DrawString(textRect.Integral(), attributes);

            var newImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            iconCache.SetObjectforKey(newImage, nsText);

            return newImage;
        }
    }
}

