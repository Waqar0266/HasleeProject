using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Web.Models.Catalog.Categories.Get;
using Hasslefree.Web.Models.Media.Pictures;
using System;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class GetCategoryService : IGetCategoryService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<Category> CategoryRepo { get; }
		private IDataRepository<Picture> PictureRepo { get; }

		#endregion

		#region Constructor

		public GetCategoryService
		(
			IDataRepository<Category> categoryRepo,
			IDataRepository<Picture> pictureRepo
		)
		{
			// Repos
			CategoryRepo = categoryRepo;
			PictureRepo = pictureRepo;
		}

		#endregion

		#region IGetCategoryService

		public CategoryWarning Warning { get; private set; }

		public CategoryGet this[int categoryId, bool includeDates = true, bool includeProducts = false]
		{
			get
			{
				if (categoryId <= 0) return CategoryNotFound();

				var category = CategoryQuery(categoryId);

				if (category == null) return CategoryNotFound();

				return new CategoryGet
				{
					CategoryId = category.CategoryId,
					CreatedOn = includeDates ? category.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? category.ModifiedOn : (DateTime?)null,
					Path = category.Path,
					Name = category.Name,
					Description = category.Description,
					NestedLevel = category.NestedLevel,
					DisplayOrder = category.DisplayOrder,
					ParentCategoryId = category.ParentCategoryId,
					Hidden = category.Hidden,
					Tag = category.Tag,
					Picture = category.PictureId.HasValue && category.Picture != null ? new PictureModel
					{
						PictureId = category.PictureId.Value,
						CreatedOn = includeDates ? category.Picture.CreatedOn : (DateTime?)null,
						ModifiedOn = includeDates ? category.Picture.ModifiedOn : (DateTime?)null,
						Name = category.Picture.Name,
						Path = category.Picture.Path,
						AltText = category.Picture.AltText,
						DisplayOrder = 0,
						FormatEnum = category.Picture.FormatEnum,
						Transforms = category.Picture.Transforms
					} : null
				};
			}
		}

		public QueryFutureValue<Category> FutureValue(int categoryId) => categoryId <= 0 ? null : CategoryRepo.Table.DeferredFirstOrDefault(c => c.CategoryId == categoryId).FutureValue();

		#endregion

		#region Private Methods

		private dynamic CategoryNotFound()
		{
			Warning = new CategoryWarning(CategoryWarningCode.CategoryNotFound);
			return null;
		}

		private Category CategoryQuery(int categoryId)
		{
			var categoryType = EntityType.Category.ToString();

			var cFuture = (from c in CategoryRepo.Table
						   where c.CategoryId == categoryId
						   select c).DeferredFirstOrDefault().FutureValue();

			var picFuture = (from c in CategoryRepo.Table
							 where c.CategoryId == categoryId && c.PictureId.HasValue
							 join p in PictureRepo.Table on c.PictureId.Value equals p.PictureId
							 select p).DeferredFirstOrDefault().FutureValue();

			var category = cFuture.Value;

			if (category == null) return null;

			category.Picture = category.Picture ?? picFuture.Value;

			return category;
		}

		#endregion
	}
}