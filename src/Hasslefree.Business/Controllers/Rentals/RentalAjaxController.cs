﻿using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Data;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	[AccessControlFilter(Permission = "Agent,Director")]
	public class RentalAjaxController : BaseController
	{
		//Repos
		private IReadOnlyRepository<Rental> RentalRepo { get; }

		public RentalAjaxController(
			//Repos
			IReadOnlyRepository<Rental> rentalRepo
			)
		{
			//Repos
			RentalRepo = rentalRepo;
		}

		[HttpGet, Route("rentals/query")]
		public ActionResult Index(string phrase)
		{
			var rentals = RentalRepo.Table.ToList();
			var results = rentals.Where(a => a.Premises.ToLower().Contains(phrase) || a.Address.ToLower().Contains(phrase)).Select(x => new { RentalId = x.RentalId, Name = x.Premises + " (" + x.Address + ")" }).ToList();
			return Json(results, JsonRequestBehavior.AllowGet);
		}
	}
}