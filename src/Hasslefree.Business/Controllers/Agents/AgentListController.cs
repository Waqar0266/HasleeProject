using Hasslefree.Core;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Services.Agents.Crud.Filters;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Agents
{
	[AccessControlFilter(Permission = "Director")]
	public class AgentListController : BaseController
	{
		#region Properties

		// Services
		private IListAgentsService ListService { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public AgentListController
		(
			IListAgentsService listService,
			IWebHelper webHelper
		)
		{
			// Services
			ListService = listService;

			// Other
			WebHelper = webHelper;
		}

		#endregion

		[HttpGet, Route("account/agents")]
		public ActionResult List
		(
			SortBy sortBy = SortBy.None,
			FilterBy filterBy = FilterBy.None,
			string search = null,
			int page = 0,
			int pageSize = 50
		)
		{
			ListService
				.SortBy(sortBy.ToString())
				.FilterBy(filterBy.ToString())
				.WithSearch(search)
				.WithPaging(page, pageSize);

			var list = ListService.List();

			// Set the needed view bags
			PrepViewBag(sortBy, filterBy, search, list.Page, list.PageSize, list.TotalRecords);

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(list.Items, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Catalog/Categories/List", list.Items);

			// Normal HTML
			return View("../Agents/List", list.Items);
		}

		#region Private Methods

		private void PrepViewBag(SortBy sortBy, FilterBy filterBy, string search, int page, int pageSize, int totalRecords)
		{
			ViewBag.SortBy = sortBy;
			ViewBag.SortByList = Enum.GetValues(typeof(SortBy)).Cast<SortBy>().ToList();

			ViewBag.FilterBy = filterBy;
			ViewBag.FilterByList = Enum.GetValues(typeof(FilterBy)).Cast<FilterBy>().ToList();

			ViewBag.Search = search;
			ViewBag.Page = page;
			ViewBag.PageSize = pageSize;
			ViewBag.TotalRecords = totalRecords;

			ViewBag.Title = "Agents";
		}

		#endregion
	}
}