using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Listings;
using Hasslefree.Services.Properties;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Rentals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	[AccessControlFilter(Permission = "Agent,Director")]
	public class LinkProperty24Controller : BaseController
	{
		//Helper & Managers
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		//Services
		private IGetProperty24Service GetPropertyService { get; }
		private ICreatePropertyService CreatePropertyService { get; }
		private IUpdateRentalService UpdateRentalService { get; }

		public LinkProperty24Controller(
			//Helper & Managers
			IWebHelper webHelper,
			ISessionManager sessionManager,

			//Services
			IGetProperty24Service getPropertyService,
			ICreatePropertyService createPropertyService,
			IUpdateRentalService updateRentalService)
		{
			//Helpers
			WebHelper = webHelper;
			SessionManager = sessionManager;

			//Services
			GetPropertyService = getPropertyService;
			CreatePropertyService = createPropertyService;
			UpdateRentalService = updateRentalService;
		}

		[HttpGet, Route("account/rental/link-property24")]
		public ActionResult Index(string hash)
		{
			ViewBag.Title = "Link Property 24";

			string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

			int rentalId = Int32.Parse(decodedHash);

			var model = new LinkPropertyModel()
			{
				RentalId = rentalId
			};

			// Normal HTML
			return View("../Rentals/LinkProperty24", model);
		}

		[HttpGet, Route("account/rental/link-property24/ajax")]
		public ActionResult Ajax(string id)
		{
			var property = GetPropertyService.WithPropertyId(id).Get();
			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{JsonConvert.SerializeObject(property.BuildingKeyValues)}~{JsonConvert.SerializeObject(property.ExternalFeaturesKeyValues)}~{JsonConvert.SerializeObject(property.OtherFeaturesKeyValues)}~{JsonConvert.SerializeObject(property.OverviewKeyValues)}~{JsonConvert.SerializeObject(property.RoomsKeyValues)}"));
			var model = new LinkPropertyModel()
			{
				Title = property.Name,
				Description = property.Description,
				Price = property.PriceNumeric,
				Property24Id = id,
				Address = property.Address,
				PrettyPrice = property.Price,
				Images = property.Images,
				BuildingKeyValues = property.BuildingKeyValues,
				ExternalFeaturesKeyValues = property.ExternalFeaturesKeyValues,
				OtherFeaturesKeyValues = property.OtherFeaturesKeyValues,
				OverviewKeyValues = property.OverviewKeyValues,
				RoomsKeyValues = property.RoomsKeyValues,
				City = property.City,
				Province = property.Province,
				Suburb = property.Suburb,
				KeyValues = hash,
				ImageList = String.Join(";", property.Images)
			};

			// Normal HTML
			return PartialView("../Rentals/_Property24", model);
		}

		[HttpPost, Route("account/rental/link-property24")]
		[ValidateInput(false)]
		public ActionResult Index(LinkPropertyModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Suppress))
					{
						CreatePropertyService.New(Core.Domain.Properties.PropertyType.Rental, model.Title, model.Address, model.Description, model.Price, model.Property24Id);

						string[] parts = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(model.KeyValues)).Split('~');
						var buildingKeyValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(parts[0]);
						var externalFeaturesKeyValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(parts[1]);
						var otherFeaturesKeyValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(parts[2]);
						var overviewKeyValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(parts[3]);
						var roomsKeyValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(parts[4]);

						foreach (var item in externalFeaturesKeyValues) CreatePropertyService.WithExternalFeaturesKeyValues(item.Key, item.Value);
						foreach (var item in buildingKeyValues) CreatePropertyService.WithBuildingKeyValues(item.Key, item.Value);
						foreach (var item in otherFeaturesKeyValues) CreatePropertyService.WithOtherFeaturesKeyValues(item.Key, item.Value);
						foreach (var item in overviewKeyValues) CreatePropertyService.WithOverviewKeyValues(item.Key, item.Value);
						foreach (var item in roomsKeyValues) CreatePropertyService.WithRoomsKeyValues(item.Key, item.Value);

						CreatePropertyService.WithLocation(model.Suburb, model.City, model.Province);

						var images = model.ImageList.Split(';').ToList();
						CreatePropertyService.WithImages(images);

						bool success = CreatePropertyService.Create();
						int propertyId = CreatePropertyService.PropertyId;

						//link the property to the rental
						success = UpdateRentalService[model.RentalId]
							.Set(r => r.PropertyId, propertyId)
							.Set(r => r.ModifiedOn, DateTime.Now)
							.Set(r => r.RentalStatus, RentalStatus.Completed)
							.Update();

						// Success
						if (success)
						{
							//complete the scope
							transactionScope.Complete();

							// Ajax (+ Json)
							if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
							{
								Success = true,
								AgentId = 1,
							}, JsonRequestBehavior.AllowGet);

							// Default
							return Redirect("/account/rentals");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			ViewBag.Title = "Link Property 24";

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/LinkProperty24", model);

			// Default
			return View("../Rentals/LinkProperty24", model);
		}
	}
}