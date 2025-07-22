

namespace Cashicart.Common.Options
{
    public class ImageUploadOptions
    {
        public long MaxImageSizeBytes { get; set; }
        public long MaxTotalSizeBytes { get; set; }
        public int ResizeMaxWidth { get; set; }
        public int ResizeMaxHeight { get; set; }
        public int ThumbnailSize { get; set; }
    }
}
