using System.Windows.Media.Imaging;

namespace AutoStrongClient.Models;

internal class ImageData
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public BitmapImage Image { get; set; }
}
