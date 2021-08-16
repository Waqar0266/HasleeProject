using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Accounts
{
    [AccessControlFilter(Permission = "Agent,Director")]
    [AgentFilter]
    public class AddRentalController : BaseController
    {
        private IWebHelper WebHelper { get; }
        private ISendMail SendMail { get; }
        private ICreateRentalService CreateRentalService { get; }
        private ISessionManager SessionManager { get; }
        private IReadOnlyRepository<Agent> AgentRepo { get; }
        private IReadOnlyRepository<Rental> RentalRepo { get; }
        private IReadOnlyRepository<RentalLandlord> RentalLandlordRepo { get; }
        private IReadOnlyRepository<Person> PersonRepo { get; }

        public AddRentalController(
            //Repos
            IReadOnlyRepository<Rental> rentalRepo,
            IReadOnlyRepository<Agent> agentRepo,
            IReadOnlyRepository<RentalLandlord> rentalLandlordRepo,
            IReadOnlyRepository<Person> personRepo,

            //Helpers
            IWebHelper webHelper,
            ISendMail sendMail,
            ISessionManager sessionManager,

            //Services
            ICreateRentalService createRentalService)
        {
            //Repos
            AgentRepo = agentRepo;
            RentalRepo = rentalRepo;
            RentalLandlordRepo = rentalLandlordRepo;
            PersonRepo = personRepo;

            //Helpers
            WebHelper = webHelper;
            SendMail = sendMail;
            SessionManager = sessionManager;

            //Services
            CreateRentalService = createRentalService;
        }

        [HttpGet, Route("account/add-rental")]
        public ActionResult Index()
        {
            ViewBag.Title = "Add Rental";

            // Normal HTML
            return View("../Accounts/Rentals/Crud");
        }

        [HttpPost, Route("account/add-rental")]
        public ActionResult Index(RentalCreate model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //new
                    CreateRentalService.New(model.RentalType, model.LeaseType, model.Premises, model.StandErf);

                    //attach agent id
                    var personId = SessionManager.Login.PersonId;
                    var agent = AgentRepo.Table.FirstOrDefault(a => a.PersonId == personId);
                    CreateRentalService.WithAgentId(agent.AgentId);

                    foreach (var landlord in model.Landlords) CreateRentalService.WithLandlord(landlord.IdNumber, landlord.Name, landlord.Surname, landlord.Email, landlord.Mobile);

                    bool success = CreateRentalService.Create();

                    // Success
                    if (success)
                    {
                        //Send the emails to the landlord(s)
                        foreach (var landlord in CreateRentalService.Landlords)
                        {
                            var email = GetTempData(landlord.Tempdata).Split(';')[2];
                            SendMail.WithUrlBody($"/account/rentals/emails/landlord-initial-email?rentalId={CreateRentalService.RentalId}&landlordId={landlord.RentalLandlordId}").Send("Complete Rental Listing", email);
                        }


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
            catch (Exception ex)
            {
                Logger.LogError(ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                ModelState.AddModelError("", ex.Message);
            }

            ViewBag.Title = "Add Rental";

            if (CreateRentalService.HasWarnings) CreateRentalService.Warnings.ForEach(w => ModelState.AddModelError("", w.Message));

            // Ajax (Json)
            if (WebHelper.IsJsonRequest()) return Json(new
            {
                Success = false,
                Message = CreateRentalService.Warnings.FirstOrDefault()?.Message ?? "Unexpected error has occurred."
            }, JsonRequestBehavior.AllowGet);

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Accounts/Rentals/Crud", model);

            // Default
            return View("../Accounts/Rentals/Crud", model);
        }

        [HttpGet]
        [Email]
        [AllowAnonymous]
        [Route("account/rentals/emails/landlord-initial-email")]
        public ActionResult Email(int rentalId, int landlordId)
        {
            var rental = RentalRepo.Table.FirstOrDefault(a => a.RentalId == rentalId);
            var landlord = RentalLandlordRepo.Table.FirstOrDefault(a => a.RentalLandlordId == landlordId);
            var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentId == rental.AgentId);
            var person = PersonRepo.Table.FirstOrDefault(p => p.PersonId == agent.PersonId);

            var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.UniqueId.ToString().ToLower()};{landlord.UniqueId.ToString().ToLower()}"));

            var model = new RentalLandlordEmail()
            {
                AgentName = person.FirstName,
                AgentSurname = person.Surname,
                Name = GetTempData(landlord.Tempdata).Split(';')[0],
                Surname = GetTempData(landlord.Tempdata).Split(';')[1],
                Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/complete-rental?hash={hash}",
                Premises = rental.Premises,
                StandErf = rental.StandErf
            };

            return View("../Emails/Landlord-Initial-Email", model);
        }

        #region Private Methods

        private string GetTempData(string tempData)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
        }

        #endregion
    }
}