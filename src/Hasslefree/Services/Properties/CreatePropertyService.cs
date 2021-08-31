using Hasslefree.Core;
using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Properties;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Media.Pictures;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Transactions;

namespace Hasslefree.Services.Properties
{
	public class CreatePropertyService : ICreatePropertyService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<Property> PropertyRepo { get; }
		private IDataRepository<Category> CategoryRepo { get; }

		//Services
		private IUploadPictureService UploadPicture { get; }

		//Other
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private Property _property;
		private int _categoryId;

		#endregion

		#region Constructor

		public CreatePropertyService
		(
			//Repos
			IDataRepository<Property> propertyRepo,
			IDataRepository<Category> categoryRepo,

			//Services
			IUploadPictureService uploadPicture,

			//Other
			ICacheManager cache
			)
		{
			// Repos
			PropertyRepo = propertyRepo;
			CategoryRepo = categoryRepo;

			//Services
			UploadPicture = uploadPicture;

			//Other
			Cache = cache;
		}

		#endregion

		#region ICreateAgentFormService

		public int PropertyId { get; private set; }

		public ICreatePropertyService New(PropertyType type, string title, string address, string description, decimal price, string privatePropertyId)
		{
			_property = new Property
			{
				Title = title,
				Address = address,
				Description = description,
				Price = price,
				PrivatePropertyId = privatePropertyId,
				PropertyType = type
			};

			return this;
		}

		public ICreatePropertyService WithOverviewKeyValues(string key, string value)
		{
			_property.OverviewKeyValues.Add(new PropertyOverviewKeyValue() { Key = key, Value = value });
			return this;
		}

		public ICreatePropertyService WithRoomsKeyValues(string key, string value)
		{
			_property.RoomKeyValues.Add(new PropertyRoomKeyValue() { Key = key, Value = value });
			return this;
		}

		public ICreatePropertyService WithExternalFeaturesKeyValues(string key, string value)
		{
			_property.ExternalFeaturesKeyValues.Add(new PropertyExternalFeaturesKeyValue() { Key = key, Value = value });
			return this;
		}

		public ICreatePropertyService WithBuildingKeyValues(string key, string value)
		{
			_property.BuildingKeyValues.Add(new PropertyBuildingKeyValue() { Key = key, Value = value });
			return this;
		}

		public ICreatePropertyService WithOtherFeaturesKeyValues(string key, string value)
		{
			_property.OtherFeaturesKeyValues.Add(new PropertyOtherFeaturesKeyValue() { Key = key, Value = value });
			return this;
		}

		public ICreatePropertyService WithLocation(string suburb, string city, string province)
		{
			var dbProvince = CategoryRepo.Table.FirstOrDefault(c => c.Name == province);
			if (dbProvince == null)
			{
				dbProvince = new Category()
				{
					Name = province,
					Path = $"/{province}"
				};

				CategoryRepo.Insert(dbProvince);
			}

			var dbCity = CategoryRepo.Table.FirstOrDefault(c => c.Name == city && c.ParentCategoryId == dbProvince.CategoryId);
			if (dbCity == null)
			{
				dbCity = new Category()
				{
					Name = suburb,
					Path = $"/{province}/{city}",
					ParentCategoryId = dbProvince.CategoryId
				};

				CategoryRepo.Insert(dbCity);
			}

			var dbSuburb = CategoryRepo.Table.FirstOrDefault(c => c.Name == suburb && c.ParentCategoryId == dbCity.CategoryId);
			if (dbSuburb == null)
			{
				dbSuburb = new Category()
				{
					Name = suburb,
					Path = $"/{province}/{city}/{suburb}",
					ParentCategoryId = dbCity.CategoryId,
					NestedLevel = 2
				};

				CategoryRepo.Insert(dbSuburb);
			}

			_categoryId = dbSuburb.CategoryId;

			return this;
		}

		public ICreatePropertyService WithImages(List<string> images)
		{
			//add the signatures
			UploadPicture.WithPath($"property_images/{_property.PrivatePropertyId}");

			foreach (var image in images)
			{
				var name = image.Replace("https://images.prop24.com/", "") + ".jpeg";
				var imageData = new WebClient().DownloadData(image);

				UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
				{
					Action = Web.Models.Common.CrudAction.Create,
					File = imageData,
					Format = Core.Domain.Media.PictureFormat.Jpeg,
					Key = name,
					Name = name,
					MimeType = "image/jpeg",
					AlternateText = name
				});
			}

			var pictures = UploadPicture.Save();

			foreach (var picture in pictures) _property.Images.Add(new PropertyPicture() { Picture = picture });

			return this;
		}

		public bool Create()
		{
			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				_property.CategoryId = _categoryId;
				PropertyRepo.Insert(_property);

				scope.Complete();
			}

			// Set property object
			PropertyId = _property.PropertyId;

			Cache.RemoveByPattern("/menu/menu-items/category");
			Cache.RemoveByPattern(CacheKeys.Server.Filter.Path);

			return true;
		}

		#endregion
	}
}
