using Hasslefree.Core;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Models.Rentals;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	public class RentalEmailsController : BaseController
	{
		#region Private Properties 

		//Services
		private IGetRentalService GetRental { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public RentalEmailsController
		(
			//Services
			IGetRentalService getRental,

			//Other
			IWebHelper webHelper
		)
		{
			//Services
			GetRental = getRental;

			// Other
			WebHelper = webHelper;
		}

		#endregion

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/rental/emails/landlord-witness-email")]
		public ActionResult LandlordWitnessEmail(int witnessNumber, int rentalId, int witnessId)
		{
			var rental = GetRental[rentalId].Get();

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalGuid.ToString()};{rental.RentalWitness.UniqueId.ToString().ToLower()};{witnessNumber}"));

			var model = new RentalWitnessEmail()
			{
				Name = witnessNumber == 1 ? rental.RentalWitness.LandlordWitness1Name : rental.RentalWitness.LandlordWitness2Name,
				Surname = witnessNumber == 1 ? rental.RentalWitness.LandlordWitness1Surname : rental.RentalWitness.LandlordWitness2Surname,
				Address = rental.Address,
				StandErf = rental.StandErf,
				ThePremises = rental.Premises,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/l/complete-witness-signature?hash={hash}"
			};

			return View("../Emails/Rental-Witness-Signature-Email", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/rental/emails/agent-witness-email")]
		public ActionResult AgentWitnessEmail(int witnessNumber, int rentalId, int witnessId)
		{
			var rental = GetRental[rentalId].Get();

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalGuid.ToString()};{rental.RentalWitness.UniqueId.ToString().ToLower()};{witnessNumber}"));

			var model = new RentalWitnessEmail()
			{
				Name = witnessNumber == 1 ? rental.RentalWitness.AgentWitness1Name : rental.RentalWitness.AgentWitness2Name,
				Surname = witnessNumber == 1 ? rental.RentalWitness.AgentWitness1Surname : rental.RentalWitness.AgentWitness2Surname,
				Address = rental.Address,
				StandErf = rental.StandErf,
				ThePremises = rental.Premises,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/a/complete-witness-signature?hash={hash}"
			};

			return View("../Emails/Rental-Witness-Signature-Email", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/rental/emails/partner-signature-email")]
		public ActionResult PartnerSignatureEmail(int rentalId, int partnerNumber)
		{
			var rental = GetRental[rentalId].Get();

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalId};{partnerNumber}"));

			string name = "";
			string surname = "";
			if (partnerNumber == 1) name = rental.RentalFica.Partner1Name;
			if (partnerNumber == 2) name = rental.RentalFica.Partner2Name;
			if (partnerNumber == 3) name = rental.RentalFica.Partner3Name;

			if (partnerNumber == 1) surname = rental.RentalFica.Partner1Surname;
			if (partnerNumber == 2) surname = rental.RentalFica.Partner2Surname;
			if (partnerNumber == 3) surname = rental.RentalFica.Partner3Surname;

			var model = new RentalPartnerEmail()
			{
				Name = name,
				Surname = surname,
				Address = rental.Address,
				StandErf = rental.StandErf,
				ThePremises = rental.Premises,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/l/complete-partner-signature?hash={hash}"
			};

			return View("../Emails/Rental-Partner-Signature-Email", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/rental/emails/agent-signature-email")]
		public ActionResult AgentSignatureEmail(int rentalId)
		{
			var rental = GetRental[rentalId].Get();

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalGuid.ToString().ToLower()};{rental.Agent.AgentGuid.ToString().ToLower()}"));

			var model = new RentalAgentEmail()
			{
				Name = rental.AgentPerson.FirstName,
				Surname = rental.AgentPerson.Surname,
				Address = rental.Address,
				StandErf = rental.StandErf,
				Premises = rental.Premises,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/a/complete?hash={hash}"
			};

			return View("../Emails/Rental-Agent-Signature-Email", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/rentals/emails/landlord-initial-email")]
		public ActionResult Email(int rentalId, int landlordId)
		{
			var rental = GetRental[rentalId].Get();
			var landlord = rental.RentalLandlords.FirstOrDefault(a => a.RentalLandlordId == landlordId);

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalGuid.ToString().ToLower()};{landlord.UniqueId.ToString().ToLower()}"));

			var model = new RentalLandlordEmail()
			{
				AgentName = rental.AgentPerson.FirstName,
				AgentSurname = rental.AgentPerson.Surname,
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