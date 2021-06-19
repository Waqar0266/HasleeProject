using System;
using Hasslefree.Core.Domain.Media;

namespace Hasslefree.Core.Configuration
{
    /// <summary>
    /// Settings for media i.e. pictures, videos, documents etc.
    /// </summary>
    public class MediaSettings : ISettings
    {
        /// <summary>
        /// Set defaults
        /// </summary>
        public MediaSettings()
        {
            MediaStorage = MediaStorage.Disk;

            ButtonPictureSize = 60;
            ThumbnailPictureSize = 120;
			SmallPictureSize = 240;
            StandardPictureSize = 480;
			LargePictureSize = 640;
            ZoomPictureSize = 1280;
			ProductListPictureWidth = 262;
			ProductListPictureHeight = 262;
			MiniCartPictureWidth = 80;
			MiniCartPictureHeight = 80;
			ProductDetailCarouselPictureWidth = 1026;
			ProductDetailCarouselPictureHeight = 1026;
			ProductDetailCarouselThumbnailPictureWidth = 106;
			ProductDetailCarouselThumbnailPictureHeight = 106;
			ButtonPictureWidth = 40;
			ButtonPictureHeight = 40;
			ThumbnailPictureWidth = 80;
			ThumbnailPictureHeight = 80;
			SmallPictureWidth = 240;
			ButtonPictureHeight = 240;
			StandardPictureWidth = 480;
			StandardPictureHeight = 480;
			LargePictureWidth = 720;
			LargePictureHeight = 720;

			StorageRootPath = "/media";
            PictureFolderName = "pictures";
            VideoFolderName = "videos";
            DocumentFolderName = "documents";
            ArchiveFolderName = "archives";
            MiscFolderName = "misc";

            LogoPictureSize = 200;
        }

        /// <summary>
        /// Where does media get stored
        /// </summary>
        public MediaStorage MediaStorage { get; set; }

        /* The various sizes to resize pictures */
        public Int32 ButtonPictureSize { get; set; }
        public Int32 ThumbnailPictureSize { get; set; }
		public Int32 SmallPictureSize { get; set; }
        public Int32 StandardPictureSize { get; set; }
		public Int32 LargePictureSize { get; set; }
        public Int32 ZoomPictureSize { get; set; }
        public Int32 LogoPictureSize { get; set; }
		public Int32 ProductListPictureWidth { get; set; }
	    public Int32 ProductListPictureHeight { get; set; }
		public Int32 MiniCartPictureWidth { get; set; }
		public Int32 MiniCartPictureHeight { get; set; }
		public Int32 ProductDetailCarouselPictureWidth { get; set; }
		public Int32 ProductDetailCarouselPictureHeight { get; set; }
		public Int32 ProductDetailCarouselThumbnailPictureWidth { get; set; }
		public Int32 ProductDetailCarouselThumbnailPictureHeight { get; set; }
		public Int32 ButtonPictureWidth { get; set; }
		public Int32 ButtonPictureHeight { get; set; }
		public Int32 ThumbnailPictureWidth { get; set; }
		public Int32 ThumbnailPictureHeight { get; set; }
		public Int32 SmallPictureWidth { get; set; }
		public Int32 SmallPictureHeight { get; set; }
		public Int32 StandardPictureWidth { get; set; }
		public Int32 StandardPictureHeight { get; set; }
		public Int32 LargePictureWidth { get; set; }
		public Int32 LargePictureHeight { get; set; }

		/// <summary>
		/// The relative path where media is stored
		/// </summary>
		public String StorageRootPath { get; set; }

        /* The names of folders where media is stored */
        public String PictureFolderName { get; set; }
        public String VideoFolderName { get; set; }
        public String AudioFolderName { get; set; }
        public String DocumentFolderName { get; set; }
        public String ArchiveFolderName { get; set; }
        public String MiscFolderName { get; set; }
    }
}
