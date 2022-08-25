using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Sales.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Sales
{
    [AccessControlFilter(Permission = "Agent,Director")]
    [AgentFilter]
    public class AddSaleController : BaseController
    {
        //Repos
        private IReadOnlyRepository<Agent> AgentRepo { get; }

        //Helper & Managers
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }
        private ISendMail SendMail { get; }

        //Services
        private ICreateSaleService CreateSaleService { get; }
        private IGetSaleService GetSale { get; }

        public AddSaleController
        (
            //Repos
            IReadOnlyRepository<Agent> agentRepo,

            //Helper & Managers
            IWebHelper webHelper,
            ISendMail sendMail,
            ISessionManager sessionManager,

            //Services
            ICreateSaleService createSaleService,
            IGetSaleService getSale
        )
        {
            //Repos
            AgentRepo = agentRepo;

            //Helpers
            WebHelper = webHelper;
            SendMail = sendMail;
            SessionManager = sessionManager;

            //Services
            CreateSaleService = createSaleService;
            GetSale = getSale;
        }

        [HttpGet, Route("account/add-sale")]
        public ActionResult Index()
        {
            ViewBag.Title = "Add Sale";

            // Normal HTML
            return View("../Sales/Crud");
        }

        [HttpPost, Route("account/add-sale")]
        public ActionResult Index(SaleCreate model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //if existing rental
                    if (model.SaleId.HasValue)
                    {
                        //var success = CreateExistingSaleService.New(model.RentalId.Value, model.Option).Create();

                        //// Success
                        //if (success)
                        //{
                        //    var rental = GetRental[model.RentalId.Value].Get();
                        //    foreach (var landlord in rental.RentalLandlords)
                        //    {
                        //        var email = landlord.Person.Email;
                        //        SendMail.WithUrlBody($"/account/rentals/emails/existing-rental-landlord-initial-email?existingRentalId={CreateExistingRentalService.ExistingRentalId}&landlordId={landlord.RentalLandlordId}").Send("Complete Existing Rental Listing", email);
                        //    }

                        //    // Ajax (+ Json)
                        //    if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
                        //    {
                        //        Success = true,
                        //        AgentId = 1,
                        //    }, JsonRequestBehavior.AllowGet);

                        //    // Default
                        //    return Redirect("/account/rentals");
                        //}
                        return Redirect("/account/sales");
                    }
                    else
                    {
                        //new
                        CreateSaleService.New(model.SaleType);

                        //attach agent id
                        var personId = SessionManager.Login.PersonId;
                        var agent = AgentRepo.Table.FirstOrDefault(a => a.PersonId == personId);
                        CreateSaleService.WithAgentId(agent.AgentId);

                        foreach (var seller in model.Sellers) CreateSaleService.WithSeller(seller.IdNumber, seller.Name, seller.Surname, seller.Email, seller.Mobile);

                        bool success = CreateSaleService.Create();

                        // Success
                        if (success)
                        {
                            //Send the emails to the seller(s)
                            foreach (var seller in CreateSaleService.Sellers)
                            {
                                var email = GetTempData(seller.Tempdata).Split(';')[2];
                                SendMail.WithUrlBody($"/account/sales/emails/seller-initial-email?saleId={CreateSaleService.SaleId}&sellerId={seller.SellerId}").Send("Complete Sale Listing", email);
                            }


                            // Ajax (+ Json)
                            if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
                            {
                                Success = true,
                                AgentId = 1,
                            }, JsonRequestBehavior.AllowGet);

                            // Default
                            return Redirect("/account/sales");
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

            ViewBag.Title = "Add Rental";

            if (CreateSaleService.HasWarnings) CreateSaleService.Warnings.ForEach(w => ModelState.AddModelError("", w.Message));

            // Ajax (Json)
            if (WebHelper.IsJsonRequest()) return Json(new
            {
                Success = false,
                Message = CreateSaleService.Warnings.FirstOrDefault()?.Message ?? "Unexpected error has occurred."
            }, JsonRequestBehavior.AllowGet);

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Sales/Crud", model);

            // Default
            return View("../Sales/Crud", model);
        }

        #region Private Methods

        private string GetTempData(string tempData)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
        }

        #endregion
    }
}