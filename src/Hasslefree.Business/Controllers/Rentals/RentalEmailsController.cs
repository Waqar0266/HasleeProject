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
		private IGetExistingRentalService GetExistingRental { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public RentalEmailsController
		(
			//Services
			IGetRentalService getRental,
			IGetExistingRentalService getExistingRental,

			//Other
			IWebHelper webHelper
		)
		{
			//Services
			GetRental = getRental;
			GetExistingRental = getExistingRental;

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
		[Route("account/rental/emails/member-signature-email")]
		public ActionResult MemberSignatureEmail(int rentalId, int rentalResolutionMemberId)
		{
			var rental = GetRental[rentalId].Get();
			var rentalResolutionMember = rental.RentalResolution.Members.FirstOrDefault(a => a.RentalResolutionMemberId == rentalResolutionMemberId);
			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalId};{rentalResolutionMemberId}"));

			string name = rentalResolutionMember.Name;
			string surname = rentalResolutionMember.Surname;

			var model = new RentalPartnerEmail()
			{
				Name = name,
				Surname = surname,
				Address = rental.Address,
				StandErf = rental.StandErf,
				ThePremises = rental.Premises,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/l/complete-member-signature?hash={hash}"
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
		[Route("account/rental/emails/agent-property-link-email")]
		public ActionResult AgentPropertyLinkEmail(int rentalId)
		{
			var rental = GetRental[rentalId].Get();

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalId}"));

			var model = new RentalAgentEmail()
			{
				Name = rental.AgentPerson.FirstName,
				Surname = rental.AgentPerson.Surname,
				Address = rental.Address,
				StandErf = rental.StandErf,
				Premises = rental.Premises,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/link-property24?hash={hash}"
			};

			return View("../Emails/Rental-Agent-Property-Link-Email", model);
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

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/rentals/emails/existing-rental-landlord-initial-email")]
		public ActionResult ExistingRentalLandlordEmail(int existingRentalId, int landlordId)
		{
			var existingRental = GetExistingRental[existingRentalId].Get();
			var rental = GetRental[existingRental.RentalId].Get();
			var landlord = rental.RentalLandlords.FirstOrDefault(a => a.RentalLandlordId == landlordId);

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{existingRental.ExistingRentalGuid.ToString().ToLower()};{landlord.UniqueId.ToString().ToLower()}"));

			var model = new RentalLandlordEmail()
			{
				AgentName = rental.AgentPerson.FirstName,
				AgentSurname = rental.AgentPerson.Surname,
				Name = GetTempData(landlord.Tempdata).Split(';')[0],
				Surname = GetTempData(landlord.Tempdata).Split(';')[1],
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/complete-existing-rental?hash={hash}",
				Premises = rental.Premises,
				StandErf = rental.StandErf
			};

			return View("../Emails/Existing-Rental-Landlord-Initial-Email", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/rental/emails/landlord-documentation-email")]
		public ActionResult LandlordDocumentationEmail(int rentalId, int landlordId)
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

			return View("../Emails/Landlord-Documentation-Email", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/existing-rental/emails/landlord-witness-email")]
		public ActionResult ExistingRentalLandlordWitnessEmail(int witnessNumber, int existingRentalId)
		{
			var existingRental = GetExistingRental[existingRentalId].Get();

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{existingRental.ExistingRentalGuid.ToString()};{witnessNumber}"));

			var model = new ExistingRentalWitnessEmail()
			{
				Rental = existingRental.Rental,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/existing-rental/l/complete-witness-signature?hash={hash}"
			};

			return View("../Emails/Existing-Rental-Witness-Signature-Email", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/existing-rental/emails/agent-signature-email")]
		public ActionResult ExistingRentalAgentSignatureEmail(int existingRentalId)
		{
			var existingRental = GetExistingRental[existingRentalId].Get();

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{existingRental.ExistingRentalGuid.ToString().ToLower()}"));

			var model = new ExistingRentalAgentEmail()
			{
				Name = existingRental.Rental.Agent.Person.FirstName,
				Surname = existingRental.Rental.Agent.Person.Surname,
				Address = existingRental.Rental.Address,
				StandErf = existingRental.Rental.StandErf,
				Premises = existingRental.Rental.Premises,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/existing-rental/a/complete?hash={hash}"
			};

			return View("../Emails/Existing-Rental-Agent-Signature-Email", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/existing-rental/emails/agent-witness-email")]
		public ActionResult ExistingRentalAgentWitnessEmail(int witnessNumber, int existingRentalId)
		{
			var existingRental = GetExistingRental[existingRentalId].Get();

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{existingRental.ExistingRentalGuid.ToString()};{witnessNumber}"));

			var model = new ExistingRentalWitnessEmail()
			{
				Name = witnessNumber == 1 ? existingRental.AgentWitness1Name : existingRental.AgentWitness2Name,
				Surname = witnessNumber == 1 ? existingRental.AgentWitness1Surname : existingRental.AgentWitness2Surname,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/existing-rental/a/complete-witness-signature?hash={hash}",
				Rental = existingRental.Rental
			};

			return View("../Emails/Existing-Rental-Witness-Signature-Email", model);
		}

		#region Private Methods

		private string GetTempData(string tempData)
		{
			return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
		}

		#endregion
	}
}