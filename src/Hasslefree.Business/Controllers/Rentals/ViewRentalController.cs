using Hasslefree.Core;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	[AccessControlFilter(Permission = "Director,Landlord,Agent")]
	public class ViewRentalController : BaseController
	{
		#region Private Properties

		// Services
		private IGetRentalService GetRentalService { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public ViewRentalController
		(
			//Services
			IGetRentalService getRentalService,

			//Other
			IWebHelper webHelper
		)
		{
			// Services
			GetRentalService = getRentalService;

			// Other
			WebHelper = webHelper;
		}

		#endregion

		[HttpGet, Route("account/rental")]
		public ActionResult View(int rentalId)
		{
			// Model
			var rental = GetRentalService[rentalId].Get();

			if (rental == null)
			{
				if (WebHelper.IsAjaxRequest())
					return Json(new { }, JsonRequestBehavior.AllowGet);

				return RedirectToAction("Index", "ListRentals");
			}

			// View bags
			PrepViewBag();

			// Ajax
			if (WebHelper.IsAjaxRequest())
				return PartialView("../Rentals/View", rental);

			// Default
			return View("../Rentals/View", rental);
		}

		#region Private 

		/// <summary>
		/// Set required view bags for action and the crud title
		/// </summary>
		private void PrepViewBag()
		{
			// Set current crud title to display
			ViewBag.Title = "View Rental";
		}

		#endregion
	}
}