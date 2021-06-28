using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Catalog.Categories.Crud;
using Hasslefree.Services.Forms;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.Security.Groups;
using PdfSharp.Pdf.IO;
using System;
using System.Linq;
using System.Text;

namespace HasslefreeTool
{
	class Program
	{
		static void Main(string[] args)
		{
			Init();

			//GetFormFields("Individual_estate_agent_re_registration_form_1475180699");

			TestPdfForms();

			//Install();

			var d1 = CalculateDateOfBirth("9105105116089");
			var d2 = CalculateDateOfBirth("9006220255085");
			var d3 = CalculateDateOfBirth("2005265539087");
		}

		private static DateTime CalculateDateOfBirth(string idNumber)
		{
			string id = idNumber.Substring(0, 6);
			string y = id.Substring(0, 2);
			string year = $"20{y}";
			if (Int32.Parse(id.Substring(0, 1)) > 2) year = $"19{y}";

			int month = Int32.Parse(id.Substring(2, 2));
			int day = Int32.Parse(id.Substring(4, 2));

			return new DateTime(Int32.Parse(year), month, day);
		}

		private static void Init()
		{
			//Start the Hasslefree application engine
			EngineContext.Initialize(false);
		}

		private static void InstallSecurityGroups()
		{
			var createSecurityGroupService = EngineContext.Current.Resolve<ICreateSecurityGroupService>();
			var createPersonService = EngineContext.Current.Resolve<ICreatePersonService>();
			var loginRepo = EngineContext.Current.Resolve<IDataRepository<Login>>();

			if (!loginRepo.Table.Any(l => l.Email == "admin@hasslefree.sa.com"))
			{

				createPersonService.New("Admin", "Admin", "Admin", "admin@hasslefree.sa.com").WithPassword("password", "").Create();
				createSecurityGroupService.New("Admin", "Admin").WithUser(createPersonService.LoginId).Create();
			}

			if (!loginRepo.Table.Any(l => l.Email == "director@hasslefree.sa.com"))
			{
				createPersonService.New("Director", "Director", "Director", "director@hasslefree.sa.com").WithPassword("password", "").Create();
				createSecurityGroupService.New("Director", "Director").WithUser(createPersonService.LoginId).Create();
			}

			//create the agent role
			createSecurityGroupService.New("Agent", "Agent").Create();
		}

		private static void InstallTopLevelCategories()
		{
			var createCategoryService = EngineContext.Current.Resolve<ICreateCategoryService>();
			createCategoryService.New("Eastern Cape", "", false).Create();
			createCategoryService.New("Free State", "", false).Create();
			createCategoryService.New("Gauteng", "", false).Create();
			createCategoryService.New("KwaZulu-Natal", "", false).Create();
			createCategoryService.New("Limpopo", "", false).Create();
			createCategoryService.New("Mpumalanga", "", false).Create();
			createCategoryService.New("Northern Cape", "", false).Create();
			createCategoryService.New("North West", "", false).Create();
		}

		private static void Install()
		{
			InstallSecurityGroups();
			InstallTopLevelCategories();
		}

		private static void TestPdfForms()
		{
			var fillForm = EngineContext.Current.Resolve<IFillFormService>();

			var data = fillForm.Prepare("Appointment letter.pdf")
			.WithField("AgentNameSurname", "Uwan Pretorius")
			.WithField("IdNumber", "9105105116089")
			.WithField("SignedAt", "Pretoria")
			.WithField("SignedDay", DateTime.Now.ToString("dd"))
			.WithField("SignedMonthYear", DateTime.Now.ToString("MMMM") + " " + DateTime.Now.ToString("yyyy"))
			.Process();

			System.IO.File.WriteAllBytes(Environment.CurrentDirectory + "\\test.pdf", data);

		}

		private static void GetFormFields(string pdfName)
		{
			StringBuilder sb = new StringBuilder();
			var document = PdfReader.Open(Environment.CurrentDirectory + $"\\{pdfName}.pdf", PdfDocumentOpenMode.ReadOnly);
			foreach (var field in document.AcroForm.Fields.Names)
			{
				sb.AppendLine(field);
			}

			System.IO.File.WriteAllText(Environment.CurrentDirectory + "\\fields.txt", sb.ToString());
		}
	}
}
