using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hasslefree.Core.Configuration.Media;
using Hasslefree.Services.Configuration;
using Hasslefree.Web.Models.Media.Pictures;

namespace Hasslefree.Services.Media.Pictures
{
	public class DefaultTransformService : IDefaultTransformService
	{
		#region Constants

		#endregion

		#region Private Properties

		// Services
		private ISettingsService SettingsService { get; }

		#endregion

		#region Fields

		private MediaTransformSettings _settings;
		private PictureType _for;

		#endregion

		#region Constructor

		public DefaultTransformService(
				ISettingsService settingsService
			)
		{
			// Services
			SettingsService = settingsService;
		}
		#endregion

		#region IDefaultTransformService

		public IDefaultTransformService With(Int32? height = null, Int32? width = null, String fitMode = null, String format = null,
			Int32? cropX1 = null, Int32? cropY1 = null, Int32? cropX2 = null, Int32? cropY2 = null,
			Boolean whitespaceTrim = false)
		{
			Input = GetTransforms(new MediaTransforms()
			{
				Height = height,
				Width = width,
				FitMode = fitMode,
				Format = format,
				CropX1 = cropX1,
				CropY1 = cropY1,
				CropX2 = cropX2,
				CropY2 = cropY2,
				WhiteSpaceTrim = whitespaceTrim
			});
			_for = PictureType.Input;
			return this;
		}

		public IDefaultTransformService For(PictureType type)
		{
			_for = type;
			return this;
		}

		public String Get()
		{
			GetSettings();

			switch(_for)
			{
				case PictureType.Input: return Input;
				case PictureType.Product: return Product;
				case PictureType.Category: return Category;
				case PictureType.Manufacturer: return Manufacturer;
				case PictureType.Logo: return Logo;
				case PictureType.Favicon: return Favicon;
				case PictureType.Content: return Content;
			}
			return "";
		}

		#endregion

		#region Private Methods

		private String Input { get; set; }

		private String _product;
		private String Product
		{
			get
			{
				if(_product != null)
					return _product;

				return _product = GetTransforms(_settings.Product);
			}
		}

		private String _category;
		private String Category
		{
			get
			{
				if(_category != null)
					return _category;

				return _category = GetTransforms(_settings.Category);
			}
		}

		private String _manufacturer;
		private String Manufacturer
		{
			get
			{
				if(_manufacturer != null)
					return _manufacturer;

				return _manufacturer = GetTransforms(_settings.Manufacturer);
			}
		}

		private String _logo;
		private String Logo
		{
			get
			{
				if(_logo != null)
					return _logo;

				return _logo = GetTransforms(_settings.Logo);
			}
		}

		private String _favicon;
		private String Favicon
		{
			get
			{
				if(_favicon != null)
					return _favicon;

				return _favicon = GetTransforms(_settings.Favicon);
			}
		}

		private String _content;
		private String Content
		{
			get
			{
				if(_content != null)
					return _content;

				return _content = GetTransforms(_settings.Content);
			}
		}

		private void GetSettings()
		{
			if(_settings != null)
				return;

			_settings = SettingsService.LoadSetting<MediaTransformSettings>();
		}

		private static String GetTransforms(MediaTransforms data)
		{
			var transforms = new List<String>();

			if((data.Height ?? 0) > 0)
				transforms.Add($"height={data.Height}");

			if((data.Width ?? 0) > 0)
				transforms.Add($"width={data.Width}");

			if(!String.IsNullOrWhiteSpace(data.FitMode))
				transforms.Add($"mode={data.FitMode}");

			if(!String.IsNullOrWhiteSpace(data.Format) && data.Format != "Default")
				transforms.Add($"format={data.Format}");

			if((data.CropX1 ?? 0) > 0 &&
			   (data.CropY1 ?? 0) > 0 ||
			   (data.CropX2 ?? 0) > 0 &&
			   (data.CropY2 ?? 0) > 0)
			{
				transforms.Add($"crop=({data.CropX1},{data.CropY1},{data.CropX2},{data.CropY2})");
			}

			if(data.WhiteSpaceTrim)
				transforms.Add("trim.threshold=80");

			return transforms.Count > 0 ? String.Join("&", transforms) : "";
		}

		#endregion
	}
}
