using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Data;
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

		//Repos
		private IReadOnlyRepository<Rental> RentalRepo { get; }
		private IReadOnlyRepository<Agent> AgentRepo { get; }
		private IReadOnlyRepository<Person> PersonRepo { get; }
		private IReadOnlyRepository<RentalWitness> RentalWitnessRepo { get; }
		private IReadOnlyRepository<RentalLandlord> RentalLandlordRepo { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public RentalEmailsController
		(
			//Repos
			IReadOnlyRepository<Rental> rentalRepo,
			IReadOnlyRepository<RentalWitness> rentalWitnessRepo,
			IReadOnlyRepository<RentalLandlord> rentalLandlordRepo,
			IReadOnlyRepository<Agent> agentRepo,
			IReadOnlyRepository<Person> personRepo,

			//Other
			IWebHelper webHelper
		)
		{
			//Repos
			RentalRepo = rentalRepo;
			RentalWitnessRepo = rentalWitnessRepo;
			RentalLandlordRepo = rentalLandlordRepo;
			AgentRepo = agentRepo;
			PersonRepo = personRepo;

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
			var witness = RentalWitnessRepo.Table.FirstOrDefault(a => a.RentalWitnessId == witnessId);
			var rental = RentalRepo.Table.FirstOrDefault(a => a.RentalId == rentalId);

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{witness.UniqueId.ToString().ToLower()};{witnessNumber}"));

			var model = new RentalWitnessEmail()
			{
				Name = witnessNumber == 1 ? witness.LandlordWitness1Name : witness.LandlordWitness2Name,
				Surname = witnessNumber == 1 ? witness.LandlordWitness1Surname : witness.LandlordWitness2Surname,
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
			var witness = RentalWitnessRepo.Table.FirstOrDefault(a => a.RentalWitnessId == witnessId);
			var rental = RentalRepo.Table.FirstOrDefault(a => a.RentalId == rentalId);

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{witness.UniqueId.ToString().ToLower()};{witnessNumber}"));

			var model = new RentalWitnessEmail()
			{
				Name = witnessNumber == 1 ? witness.AgentWitness1Name : witness.AgentWitness2Name,
				Surname = witnessNumber == 1 ? witness.AgentWitness1Surname : witness.AgentWitness2Surname,
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
		[Route("account/rental/emails/agent-signature-email")]
		public ActionResult AgentSignatureEmail(int rentalId)
		{
			var rental = RentalRepo.Table.FirstOrDefault(a => a.RentalId == rentalId);
			var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentId == rental.AgentId);
			var agentPerson = PersonRepo.Table.FirstOrDefault(a => a.PersonId == agent.PersonId);

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.UniqueId.ToString().ToLower()};{agent.AgentGuid.ToString().ToLower()}"));

			var model = new RentalAgentEmail()
			{
				Name = agentPerson.FirstName,
				Surname = agentPerson.Surname,
				Address = rental.Address,
				StandErf = rental.StandErf,
				Premises = rental.Premises,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/a/complete-signature?hash={hash}"
			};

			return View("../Emails/Rental-Agent-Signature-Email", model);
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