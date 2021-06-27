﻿using Hasslefree.Services.Common;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Common;
using System;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Accounts
{
	[AccessControlFilter(Permission = "Director")]
	public class ManageFirmController : BaseController
	{
		/* Dependencies */
		private IUpdateFirmService UpdateService { get; }
		private IGetFirmService GetService { get; }

		/* CTOR */
		public ManageFirmController
		(
			IUpdateFirmService updateService,
			IGetFirmService getService
		)
		{
			UpdateService = updateService;
			GetService = getService;
		}

		/* GET */
		[HttpGet]
		[Route("account/manage-firm")]
		public ActionResult Index()
		{
			// Get the model
			var model = GetService.Get();

			// Set select lists in ViewBag
			SetViewBag();

			// View
			return View("../Accounts/Firm", model);
		}

		/* POST */
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("account/manage-firm")]
		public ActionResult Index(FirmModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					UpdateService
					.WithPhysicalAddress(model.PhysicalAddress1, model.PhysicalAddress2, model.PhysicalAddress3, model.PhysicalAddressTown, model.PhysicalAddressCode, model.PhysicalAddressCountry, model.PhysicalAddressRegion)
					.WithPostalAddress(model.PostalAddress1, model.PostalAddress2, model.PostalAddress3, model.PostalAddressTown, model.PostalAddressCode, model.PostalAddressCountry, model.PostalAddressRegion)
					.WithSettings(model.BusinessName, model.TradeName, model.Phone, model.Fax, model.Email, model.ReferenceNumber, model.AiNumber)
					.Update();

					return Redirect("/account/manage-firm");
				}
			}
			catch (Exception ex)
			{
				while (ex.InnerException != null) ex = ex.InnerException;
				Core.Logging.Logger.LogError(ex);
				ModelState.AddModelError(String.Empty, ex.Message);
			}

			// Set select lists in ViewBag
			SetViewBag();

			// View
			return View("../Accounts/Firm", model);
		}

		#region Private
		private void SetViewBag()
		{
			ViewBag.Title = "Manage Firm";
		}

		#endregion
	}
}