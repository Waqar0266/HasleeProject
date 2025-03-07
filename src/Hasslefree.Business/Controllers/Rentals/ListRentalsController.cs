﻿using Hasslefree.Core;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	[AccessControlFilter(Permission = "Agent,Director,Landlord")]
	[AgentFilter]
	public class ListRentalsController : BaseController
	{
		private IListRentalService List { get; }
		private IWebHelper WebHelper { get; }

		public ListRentalsController(IListRentalService list, IWebHelper webHelper)
		{
			List = list;
			WebHelper = webHelper;
		}

		[HttpGet, Route("account/rentals")]
		public ActionResult Index(string search = null, int page = 0, int pageSize = 50)
		{
			List
				.WithSearch(search)
				.WithPaging(page, pageSize);

			var list = List.List();

			// Set the needed view bags
			PrepViewBag(search, list.Page, list.PageSize, list.TotalRecords);

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(list.Items, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/List", list.Items);

			// Normal HTML
			return View("../Rentals/List", list.Items);
		}

		#region Private Methods

		private void PrepViewBag(string search, int page, int pageSize, int totalRecords)
		{
			ViewBag.Search = search;
			ViewBag.Page = page;
			ViewBag.PageSize = pageSize;
			ViewBag.TotalRecords = totalRecords;

			ViewBag.Title = "Rentals";
		}

		#endregion
	}
}