using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
