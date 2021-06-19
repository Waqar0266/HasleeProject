using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Data;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Accounts
{
	public class ProfileUpdateEmailController : BaseController
	{
		/* Dependencies */
		private IDataRepository<Person> PersonRepo { get; }


		/* CTOR */
		public ProfileUpdateEmailController
		(IDataRepository<Person> personRepo)
		{
			PersonRepo = personRepo;
		}

		/// <summary>
		/// The welcome email
		/// </summary>
		/// <returns></returns>
		[HttpGet, Email, AllowAnonymous]
		[Route("account/profile-update-email")]
		public ActionResult UpdateProfileEmail()
		{
			var model = new { };
			return View("../Accounts/Profile-Update-Email", model);
		}
	}
}