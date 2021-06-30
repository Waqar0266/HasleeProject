using Hasslefree.Core;
using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Models.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers
{
	public class CategoryMenuController : BaseController
	{
		#region Private Properties

		private IReadOnlyRepository<Category> CategoryRepo { get; }
		private ICacheManager CacheManager { get; }
		private ISessionManager SessionManager { get; }


		#endregion

		#region Constructor

		public CategoryMenuController
		(
			//Repos
			IReadOnlyRepository<Category> categoryRepo,

			//Managers
			ICacheManager cacheManager,
			ISessionManager sessionManager
		)
		{
			//Repos
			CategoryRepo = categoryRepo;

			//Managers
			CacheManager = cacheManager;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[ChildActionOnly]
		public ActionResult CategoryMenu(int categoryId = 0, bool authenticated = false)
		{
			var personId = SessionManager.Login?.PersonId ?? 0;
			var list = CacheManager.Get(CacheKeys.FrontEnd.Menu.MenuItems.Category(authenticated, categoryId, personId), CacheKeys.Time.Hour, () =>
			{
				var path = categoryId > 0
					? CategoryRepo.Table.FirstOrDefault(c => c.CategoryId == categoryId)?.Path ?? ""
					: "";

				var categories = (from c in (from c in CategoryRepo.Table
											 where categoryId != c.CategoryId &&
												 c.Path.StartsWith(path + "/") &&
												 c.Path != path &&
												 !c.Hidden
											 select new
											 {
												 c.CategoryId,
												 c.Path,
												 c.NestedLevel,
												 c.ParentCategoryId,
												 c.Name,
												 c.DisplayOrder,
												 c.Hidden
											 }).Distinct()
								  orderby c.NestedLevel, c.DisplayOrder
								  select new
								  {
									  c.NestedLevel,
									  c.DisplayOrder,
									  Text = c.Name,
									  CategoryId = c.CategoryId,
									  c.Path,
									  LinkUrl = "/products/browse?categoryids=" + c.CategoryId,
									  ParentId = c.ParentCategoryId
								  }).ToList();

				return categories.Select(c => new MenuItem()
				{
					NestedLevel = c.NestedLevel,
					DisplayOrder = c.DisplayOrder,
					Text = c.Text,
					CategoryId = c.CategoryId,
					Path = c.Path,
					LinkUrl = c.LinkUrl,
					ParentId = c.ParentId,
				}).ToList();
			});

			return PartialView("_CategoryMenu", list);
		}

		#endregion
	}
}