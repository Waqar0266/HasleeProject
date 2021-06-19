using System;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Hasslefree.Core.Infrastructure.Email
{
	public static class Extensions
	{
		/// <summary>
		/// Converts an EmailMessage object into a System.Net.Mail.MailMessage object
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		public static MailMessage ToMailMessage(this EmailMessage msg)
		{
			//Construct the message
			MailMessage message = new MailMessage();
			message.Subject = msg.Subject;
			message.From = new MailAddress(msg.FromAddress, msg.FromName);
			
			//Add the recipients
			foreach (var recipient in msg.EmailRecipients) message.To.Add(recipient);	//Recipients
			foreach (var cc in msg.CcEmailRecipients) message.CC.Add(cc);				//CC
			foreach (var bcc in msg.BccEmailRecipients) message.Bcc.Add(bcc);			//BCC

			//Add attachments
			foreach (var att in msg.Attachments)
			{
				using(MemoryStream ms = new MemoryStream(att.File))
				{
					message.Attachments.Add(new Attachment(ms,att.Name));
				}
			}

			//Construct the HTML view for the email
			AlternateView html = AlternateView.CreateAlternateViewFromString(msg.Body.Html);
			html.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
			html.ContentType = new System.Net.Mime.ContentType("text/html");
			message.AlternateViews.Add(html);

			//Construct the TEXT view for the email
            //AlternateView txt = AlternateView.CreateAlternateViewFromString(msg.Body.Text);
            //txt.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
            //txt.ContentType = new System.Net.Mime.ContentType("text/plain");
            //message.AlternateViews.Add(txt);

			//Custom headers
			foreach (var h in msg.Headers)
			{
				message.Headers.Add(h.Key, h.Value);
			}

			return message;
		}

		public static EmailBody CreateBodyFromView(this EmailMessage email,String controllerName, String viewName, Object model,String areaName = "")
		{
			EmailBody body = new EmailBody();

			var context = new HttpContextWrapper(HttpContext.Current) as HttpContextBase;
						
			var routes = new RouteData();
			routes.Values.Add("controller", controllerName);
            if (!String.IsNullOrEmpty(areaName))
            {
                routes.Values.Add("area", areaName);
            }

			var controller = new EmailControllerFake();

			controller.ControllerContext = new ControllerContext(context, routes, controller);

			var ViewData = new ViewDataDictionary();

			var TempData = new TempDataDictionary();

			ViewData.Model = model;

			using (var sw = new StringWriter())
			{
				var viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, null);
				var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, ViewData, TempData, sw);

				viewResult.View.Render(viewContext, sw);
				body.Html = sw.GetStringBuilder().ToString();
			}

			return body;
		}
	}
}
