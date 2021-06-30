using Hasslefree.Services.Emails;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Models.Pages;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Pages
{
	public class PageController : BaseController
	{
		#region Private Poperties

		private ISendMail SendMail { get; }

		#endregion

		#region Constructor

		public PageController(ISendMail sendMail)
		{
			SendMail = sendMail;
		}

		#endregion

		[Route("company")]
		public ActionResult Company()
		{
			ViewBag.Title = "Company";
			return View("../Pages/Company");
		}

		[Route("contact")]
		public ActionResult Contact()
		{
			ViewBag.Title = "Contact";
			return View("../Pages/Contact");
		}

		[Route("faq")]
		public ActionResult Faq()
		{
			ViewBag.Title = "FAQ";
			return View("../Pages/FAQ");
		}

		[Route("contact")]
		[HttpPost]
		public ActionResult Contact(ContactModel model)
		{
			string hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(model)));
			var emailUrl = $"/contact-email?hash={hash}";

			//Send the email
			SendMail.WithUrlBody(emailUrl).Send("Contact Request", "admin@hasslefree.sa.com");

			return Json(new { success = true });
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("contact-email")]
		public ActionResult ContactEmail(string hash)
		{
			var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ContactModel>(System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash)));
			return View("../Emails/Contact-Email", model);
		}
	}
}