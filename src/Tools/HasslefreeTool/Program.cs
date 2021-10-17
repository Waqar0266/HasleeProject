using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Catalog.Categories.Crud;
using Hasslefree.Services.Forms;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.Security.Groups;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HasslefreeTool
{
	class Program
	{
		static void Main(string[] args)
		{
			Init();
			Install();

			//GetFormFields("Individual_estate_agent_re_registration_form_1475180699");
			//TestFormSignature();
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

			var secuityGroupRepo = EngineContext.Current.Resolve<IReadOnlyRepository<SecurityGroup>>();

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

			if (!secuityGroupRepo.Table.Any(s => s.SecurityGroupName == "Agent"))
				//create the agent role
				createSecurityGroupService.New("Agent", "Agent").Create();
		}

		private static void InstallTopLevelCategories()
		{
			var createCategoryService = EngineContext.Current.Resolve<ICreateCategoryService>();

			var categoryRepo = EngineContext.Current.Resolve<IReadOnlyRepository<Category>>();

			if (!categoryRepo.Table.Any(c => c.Name == "Eastern Cape"))
				createCategoryService.New("Eastern Cape", "", false).Create();
			if (!categoryRepo.Table.Any(c => c.Name == "Free State"))
				createCategoryService.New("Free State", "", false).Create();
			if (!categoryRepo.Table.Any(c => c.Name == "Gauteng"))
				createCategoryService.New("Gauteng", "", false).Create();
			if (!categoryRepo.Table.Any(c => c.Name == "KwaZulu-Natal"))
				createCategoryService.New("KwaZulu-Natal", "", false).Create();
			if (!categoryRepo.Table.Any(c => c.Name == "Limpopo"))
				createCategoryService.New("Limpopo", "", false).Create();
			if (!categoryRepo.Table.Any(c => c.Name == "Mpumalanga"))
				createCategoryService.New("Mpumalanga", "", false).Create();
			if (!categoryRepo.Table.Any(c => c.Name == "Northern Cape"))
				createCategoryService.New("Northern Cape", "", false).Create();
			if (!categoryRepo.Table.Any(c => c.Name == "North West"))
				createCategoryService.New("North West", "", false).Create();
		}

		/// <summary>
		/// Add the list of countries to the object context
		/// </summary>
		private static void InstallCountries()
		{
			var countryRepo = EngineContext.Current.Resolve<IDataRepository<Country>>();
			var database = EngineContext.Current.Resolve<IDataContext>();

			// Exit if countries are already installed
			if (countryRepo.Table.Any()) return;

			// Add countries
			countryRepo.Add(new List<Country>
			{
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 1, Name = "South Africa", NumericIsoCode = 710, Published = true, SubjectToVat = true, TwoLetterIsoCode = "ZA", ThreeLetterIsoCode = "ZAF" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 2, Name = "Namibia", NumericIsoCode = 516, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NA", ThreeLetterIsoCode = "NAM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 3, Name = "Botswana", NumericIsoCode = 72, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BW", ThreeLetterIsoCode = "BWA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 4, Name = "Zimbabwe", NumericIsoCode = 716, Published = true, SubjectToVat = false, TwoLetterIsoCode = "ZW", ThreeLetterIsoCode = "ZWE" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 5, Name = "Mozambique", NumericIsoCode = 508, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MZ", ThreeLetterIsoCode = "MOZ" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 6, Name = "Lesotho", NumericIsoCode = 426, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LS", ThreeLetterIsoCode = "LSO" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 7, Name = "Swaziland", NumericIsoCode = 748, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SZ", ThreeLetterIsoCode = "SWZ" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 8, Name = "United States of America", NumericIsoCode = 840, Published = true, SubjectToVat = false, TwoLetterIsoCode = "US", ThreeLetterIsoCode = "USA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 9, Name = "Aaland Islands", NumericIsoCode = 248, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AX", ThreeLetterIsoCode = "ALA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 10, Name = "Afghanistan", NumericIsoCode = 4, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AF", ThreeLetterIsoCode = "AFG" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 11, Name = "Albania", NumericIsoCode = 8, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AL", ThreeLetterIsoCode = "ALB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 12, Name = "Algeria", NumericIsoCode = 12, Published = true, SubjectToVat = false, TwoLetterIsoCode = "DZ", ThreeLetterIsoCode = "DZA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 13, Name = "American Samoa", NumericIsoCode = 16, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AS", ThreeLetterIsoCode = "ASM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 14, Name = "Andorra", NumericIsoCode = 20, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AD", ThreeLetterIsoCode = "AND" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 15, Name = "Angola", NumericIsoCode = 24, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AO", ThreeLetterIsoCode = "AGO" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 16, Name = "Anguilla", NumericIsoCode = 660, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AI", ThreeLetterIsoCode = "AIA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 17, Name = "Antarctica", NumericIsoCode = 10, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AQ", ThreeLetterIsoCode = "ATA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 18, Name = "Antigua And Barbuda", NumericIsoCode = 28, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AG", ThreeLetterIsoCode = "ATG" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 19, Name = "Argentina", NumericIsoCode = 32, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AR", ThreeLetterIsoCode = "ARG" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 20, Name = "Armenia", NumericIsoCode = 51, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AM", ThreeLetterIsoCode = "ARM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 21, Name = "Aruba", NumericIsoCode = 533, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AW", ThreeLetterIsoCode = "ABW" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 22, Name = "Australia", NumericIsoCode = 36, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AU", ThreeLetterIsoCode = "AUS" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 23, Name = "Austria", NumericIsoCode = 40, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AT", ThreeLetterIsoCode = "AUT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 24, Name = "Azerbaijan", NumericIsoCode = 31, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AZ", ThreeLetterIsoCode = "AZE" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 25, Name = "Bahamas", NumericIsoCode = 44, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BS", ThreeLetterIsoCode = "BHS" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 26, Name = "Bahrain", NumericIsoCode = 48, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BH", ThreeLetterIsoCode = "BHR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 27, Name = "Bangladesh", NumericIsoCode = 50, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BD", ThreeLetterIsoCode = "BGD" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 28, Name = "Barbados", NumericIsoCode = 52, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BB", ThreeLetterIsoCode = "BRB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 29, Name = "Belarus", NumericIsoCode = 112, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BY", ThreeLetterIsoCode = "BLR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 30, Name = "Belgium", NumericIsoCode = 56, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BE", ThreeLetterIsoCode = "BEL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 31, Name = "Belize", NumericIsoCode = 84, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BZ", ThreeLetterIsoCode = "BLZ" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 32, Name = "Benin", NumericIsoCode = 204, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BJ", ThreeLetterIsoCode = "BEN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 33, Name = "Bermuda", NumericIsoCode = 60, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BM", ThreeLetterIsoCode = "BMU" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 34, Name = "Bhutan", NumericIsoCode = 64, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BT", ThreeLetterIsoCode = "BTN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 35, Name = "Bolivia", NumericIsoCode = 68, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BO", ThreeLetterIsoCode = "BOL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 36, Name = "Bosnia And Herzegowina", NumericIsoCode = 70, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BA", ThreeLetterIsoCode = "BIH" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 37, Name = "Bouvet Island", NumericIsoCode = 74, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BV", ThreeLetterIsoCode = "BVT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 38, Name = "Brazil", NumericIsoCode = 76, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BR", ThreeLetterIsoCode = "BRA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 39, Name = "British Indian Ocean Territory", NumericIsoCode = 86, Published = true, SubjectToVat = false, TwoLetterIsoCode = "IO", ThreeLetterIsoCode = "IOT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 40, Name = "Brunei Darussalam", NumericIsoCode = 96, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BN", ThreeLetterIsoCode = "BRN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 41, Name = "Bulgaria", NumericIsoCode = 100, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BG", ThreeLetterIsoCode = "BGR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 42, Name = "Burkina Faso", NumericIsoCode = 854, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BF", ThreeLetterIsoCode = "BFA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 43, Name = "Burundi", NumericIsoCode = 108, Published = true, SubjectToVat = false, TwoLetterIsoCode = "BI", ThreeLetterIsoCode = "BDI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 44, Name = "Cambodia", NumericIsoCode = 116, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KH", ThreeLetterIsoCode = "KHM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 45, Name = "Cameroon", NumericIsoCode = 120, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CM", ThreeLetterIsoCode = "CMR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 46, Name = "Canada", NumericIsoCode = 124, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CA", ThreeLetterIsoCode = "CAN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 47, Name = "Cape Verde", NumericIsoCode = 132, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CV", ThreeLetterIsoCode = "CPV" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 48, Name = "Cayman Islands", NumericIsoCode = 136, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KY", ThreeLetterIsoCode = "CYM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 49, Name = "Central African Republic", NumericIsoCode = 140, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CF", ThreeLetterIsoCode = "CAF" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 50, Name = "Chad", NumericIsoCode = 148, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TD", ThreeLetterIsoCode = "TCD" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 51, Name = "Chile", NumericIsoCode = 152, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CL", ThreeLetterIsoCode = "CHL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 52, Name = "China", NumericIsoCode = 156, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CN", ThreeLetterIsoCode = "CHN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 53, Name = "Christmas Island", NumericIsoCode = 162, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CX", ThreeLetterIsoCode = "CXR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 54, Name = "Cocos Islands", NumericIsoCode = 166, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CC", ThreeLetterIsoCode = "CCK" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 55, Name = "Colombia", NumericIsoCode = 170, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CO", ThreeLetterIsoCode = "COL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 56, Name = "Comoros", NumericIsoCode = 174, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KM", ThreeLetterIsoCode = "COM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 57, Name = "Congo, Democratic Republic Of", NumericIsoCode = 180, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CD", ThreeLetterIsoCode = "COD" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 58, Name = "Congo, Republic Of", NumericIsoCode = 178, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CG", ThreeLetterIsoCode = "COG" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 59, Name = "Cook Islands", NumericIsoCode = 184, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CK", ThreeLetterIsoCode = "COK" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 60, Name = "Costa Rica", NumericIsoCode = 188, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CR", ThreeLetterIsoCode = "CRI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 61, Name = "Cote D'Ivoire", NumericIsoCode = 384, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CI", ThreeLetterIsoCode = "CIV" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 62, Name = "Croatia", NumericIsoCode = 191, Published = true, SubjectToVat = false, TwoLetterIsoCode = "HR", ThreeLetterIsoCode = "HRV" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 63, Name = "Cuba", NumericIsoCode = 192, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CU", ThreeLetterIsoCode = "CUB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 64, Name = "Cyprus", NumericIsoCode = 196, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CY", ThreeLetterIsoCode = "CYP" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 65, Name = "Czech Republic", NumericIsoCode = 203, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CZ", ThreeLetterIsoCode = "CZE" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 66, Name = "Denmark", NumericIsoCode = 208, Published = true, SubjectToVat = false, TwoLetterIsoCode = "DK", ThreeLetterIsoCode = "DNK" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 67, Name = "Djibouti", NumericIsoCode = 262, Published = true, SubjectToVat = false, TwoLetterIsoCode = "DJ", ThreeLetterIsoCode = "DJI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 68, Name = "Dominica", NumericIsoCode = 212, Published = true, SubjectToVat = false, TwoLetterIsoCode = "DM", ThreeLetterIsoCode = "DMA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 69, Name = "Dominican Republic", NumericIsoCode = 214, Published = true, SubjectToVat = false, TwoLetterIsoCode = "DO", ThreeLetterIsoCode = "DOM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 70, Name = "Ecuador", NumericIsoCode = 218, Published = true, SubjectToVat = false, TwoLetterIsoCode = "EC", ThreeLetterIsoCode = "ECU" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 71, Name = "Egypt", NumericIsoCode = 818, Published = true, SubjectToVat = false, TwoLetterIsoCode = "EG", ThreeLetterIsoCode = "EGY" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 72, Name = "El Salvador", NumericIsoCode = 222, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SV", ThreeLetterIsoCode = "SLV" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 73, Name = "Equatorial Guinea", NumericIsoCode = 226, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GQ", ThreeLetterIsoCode = "GNQ" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 74, Name = "Eritrea", NumericIsoCode = 232, Published = true, SubjectToVat = false, TwoLetterIsoCode = "ER", ThreeLetterIsoCode = "ERI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 75, Name = "Estonia", NumericIsoCode = 233, Published = true, SubjectToVat = false, TwoLetterIsoCode = "EE", ThreeLetterIsoCode = "EST" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 76, Name = "Ethiopia", NumericIsoCode = 231, Published = true, SubjectToVat = false, TwoLetterIsoCode = "ET", ThreeLetterIsoCode = "ETH" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 77, Name = "Falkland Islands", NumericIsoCode = 238, Published = true, SubjectToVat = false, TwoLetterIsoCode = "FK", ThreeLetterIsoCode = "FLK" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 78, Name = "Faroe Islands", NumericIsoCode = 234, Published = true, SubjectToVat = false, TwoLetterIsoCode = "FO", ThreeLetterIsoCode = "FRO" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 79, Name = "Fiji", NumericIsoCode = 242, Published = true, SubjectToVat = false, TwoLetterIsoCode = "FJ", ThreeLetterIsoCode = "FJI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 80, Name = "Finland", NumericIsoCode = 246, Published = true, SubjectToVat = false, TwoLetterIsoCode = "FI", ThreeLetterIsoCode = "FIN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 81, Name = "France", NumericIsoCode = 250, Published = true, SubjectToVat = false, TwoLetterIsoCode = "FR", ThreeLetterIsoCode = "FRA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 82, Name = "French Guiana", NumericIsoCode = 254, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GF", ThreeLetterIsoCode = "GUF" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 83, Name = "French Polynesia", NumericIsoCode = 258, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PF", ThreeLetterIsoCode = "PYF" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 84, Name = "French Southern Territories", NumericIsoCode = 260, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TF", ThreeLetterIsoCode = "ATF" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 85, Name = "Gabon", NumericIsoCode = 266, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GA", ThreeLetterIsoCode = "GAB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 86, Name = "Gambia", NumericIsoCode = 270, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GM", ThreeLetterIsoCode = "GMB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 87, Name = "Georgia", NumericIsoCode = 268, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GE", ThreeLetterIsoCode = "GEO" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 88, Name = "Germany", NumericIsoCode = 276, Published = true, SubjectToVat = false, TwoLetterIsoCode = "DE", ThreeLetterIsoCode = "DEU" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 89, Name = "Ghana", NumericIsoCode = 288, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GH", ThreeLetterIsoCode = "GHA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 90, Name = "Gibraltar", NumericIsoCode = 292, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GI", ThreeLetterIsoCode = "GIB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 91, Name = "Greece", NumericIsoCode = 300, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GR", ThreeLetterIsoCode = "GRC" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 92, Name = "Greenland", NumericIsoCode = 304, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GL", ThreeLetterIsoCode = "GRL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 93, Name = "Grenada", NumericIsoCode = 308, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GD", ThreeLetterIsoCode = "GRD" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 94, Name = "Guadeloupe", NumericIsoCode = 312, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GP", ThreeLetterIsoCode = "GLP" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 95, Name = "Guam", NumericIsoCode = 316, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GU", ThreeLetterIsoCode = "GUM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 96, Name = "Guatemala", NumericIsoCode = 320, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GT", ThreeLetterIsoCode = "GTM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 97, Name = "Guinea", NumericIsoCode = 324, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GN", ThreeLetterIsoCode = "GIN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 98, Name = "Guinea-Bissau", NumericIsoCode = 624, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GW", ThreeLetterIsoCode = "GNB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 99, Name = "Guyana", NumericIsoCode = 328, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GY", ThreeLetterIsoCode = "GUY" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 100, Name = "Haiti", NumericIsoCode = 332, Published = true, SubjectToVat = false, TwoLetterIsoCode = "HT", ThreeLetterIsoCode = "HTI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 101, Name = "Heard And Mc Donald Islands", NumericIsoCode = 334, Published = true, SubjectToVat = false, TwoLetterIsoCode = "HM", ThreeLetterIsoCode = "HMD" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 102, Name = "Honduras", NumericIsoCode = 340, Published = true, SubjectToVat = false, TwoLetterIsoCode = "HN", ThreeLetterIsoCode = "HND" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 103, Name = "Hong Kong", NumericIsoCode = 344, Published = true, SubjectToVat = false, TwoLetterIsoCode = "HK", ThreeLetterIsoCode = "HKG" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 104, Name = "Hungary", NumericIsoCode = 348, Published = true, SubjectToVat = false, TwoLetterIsoCode = "HU", ThreeLetterIsoCode = "HUN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 105, Name = "Iceland", NumericIsoCode = 352, Published = true, SubjectToVat = false, TwoLetterIsoCode = "IS", ThreeLetterIsoCode = "ISL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 106, Name = "India", NumericIsoCode = 356, Published = true, SubjectToVat = false, TwoLetterIsoCode = "IN", ThreeLetterIsoCode = "IND" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 107, Name = "Indonesia", NumericIsoCode = 360, Published = true, SubjectToVat = false, TwoLetterIsoCode = "ID", ThreeLetterIsoCode = "IDN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 108, Name = "Iran", NumericIsoCode = 364, Published = true, SubjectToVat = false, TwoLetterIsoCode = "IR", ThreeLetterIsoCode = "IRN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 109, Name = "Iraq", NumericIsoCode = 368, Published = true, SubjectToVat = false, TwoLetterIsoCode = "IQ", ThreeLetterIsoCode = "IRQ" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 110, Name = "Ireland", NumericIsoCode = 372, Published = true, SubjectToVat = false, TwoLetterIsoCode = "IE", ThreeLetterIsoCode = "IRL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 111, Name = "Israel", NumericIsoCode = 376, Published = true, SubjectToVat = false, TwoLetterIsoCode = "IL", ThreeLetterIsoCode = "ISR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 112, Name = "Italy", NumericIsoCode = 380, Published = true, SubjectToVat = false, TwoLetterIsoCode = "IT", ThreeLetterIsoCode = "ITA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 113, Name = "Jamaica", NumericIsoCode = 388, Published = true, SubjectToVat = false, TwoLetterIsoCode = "JM", ThreeLetterIsoCode = "JAM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 114, Name = "Japan", NumericIsoCode = 392, Published = true, SubjectToVat = false, TwoLetterIsoCode = "JP", ThreeLetterIsoCode = "JPN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 115, Name = "Jordan", NumericIsoCode = 400, Published = true, SubjectToVat = false, TwoLetterIsoCode = "JO", ThreeLetterIsoCode = "JOR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 116, Name = "Kazakhstan", NumericIsoCode = 398, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KZ", ThreeLetterIsoCode = "KAZ" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 117, Name = "Kenya", NumericIsoCode = 404, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KE", ThreeLetterIsoCode = "KEN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 118, Name = "Kiribati", NumericIsoCode = 296, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KI", ThreeLetterIsoCode = "KIR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 119, Name = "North Korea", NumericIsoCode = 408, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KP", ThreeLetterIsoCode = "PRK" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 120, Name = "South Korea", NumericIsoCode = 410, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KR", ThreeLetterIsoCode = "KOR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 121, Name = "Kuwait", NumericIsoCode = 414, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KW", ThreeLetterIsoCode = "KWT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 122, Name = "Kyrgyzstan", NumericIsoCode = 417, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KG", ThreeLetterIsoCode = "KGZ" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 123, Name = "Lao", NumericIsoCode = 418, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LA", ThreeLetterIsoCode = "LAO" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 124, Name = "Latvia", NumericIsoCode = 428, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LV", ThreeLetterIsoCode = "LVA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 125, Name = "Lebanon", NumericIsoCode = 422, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LB", ThreeLetterIsoCode = "LBN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 126, Name = "Liberia", NumericIsoCode = 430, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LR", ThreeLetterIsoCode = "LBR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 127, Name = "Libyan Arab Jamahiriya", NumericIsoCode = 434, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LY", ThreeLetterIsoCode = "LBY" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 128, Name = "Liechtenstein", NumericIsoCode = 438, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LI", ThreeLetterIsoCode = "LIE" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 129, Name = "Lithuania", NumericIsoCode = 440, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LT", ThreeLetterIsoCode = "LTU" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 130, Name = "Luxembourg", NumericIsoCode = 442, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LU", ThreeLetterIsoCode = "LUX" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 131, Name = "Macau", NumericIsoCode = 446, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MO", ThreeLetterIsoCode = "MAC" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 132, Name = "Macedonia", NumericIsoCode = 807, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MK", ThreeLetterIsoCode = "MKD" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 133, Name = "Madagascar", NumericIsoCode = 450, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MG", ThreeLetterIsoCode = "MDG" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 134, Name = "Malawi", NumericIsoCode = 454, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MW", ThreeLetterIsoCode = "MWI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 135, Name = "Malaysia", NumericIsoCode = 458, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MY", ThreeLetterIsoCode = "MYS" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 136, Name = "Maldives", NumericIsoCode = 462, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MV", ThreeLetterIsoCode = "MDV" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 137, Name = "Mali", NumericIsoCode = 466, Published = true, SubjectToVat = false, TwoLetterIsoCode = "ML", ThreeLetterIsoCode = "MLI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 138, Name = "Malta", NumericIsoCode = 470, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MT", ThreeLetterIsoCode = "MLT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 139, Name = "Marshall Islands", NumericIsoCode = 584, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MH", ThreeLetterIsoCode = "MHL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 140, Name = "Martinique", NumericIsoCode = 474, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MQ", ThreeLetterIsoCode = "MTQ" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 141, Name = "Mauritania", NumericIsoCode = 478, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MR", ThreeLetterIsoCode = "MRT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 142, Name = "Mauritius", NumericIsoCode = 480, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MU", ThreeLetterIsoCode = "MUS" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 143, Name = "Mayotte", NumericIsoCode = 175, Published = true, SubjectToVat = false, TwoLetterIsoCode = "YT", ThreeLetterIsoCode = "MYT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 144, Name = "Mexico", NumericIsoCode = 484, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MX", ThreeLetterIsoCode = "MEX" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 145, Name = "Micronesia", NumericIsoCode = 583, Published = true, SubjectToVat = false, TwoLetterIsoCode = "FM", ThreeLetterIsoCode = "FSM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 146, Name = "Moldova", NumericIsoCode = 498, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MD", ThreeLetterIsoCode = "MDA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 147, Name = "Monaco", NumericIsoCode = 492, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MC", ThreeLetterIsoCode = "MCO" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 148, Name = "Mongolia", NumericIsoCode = 496, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MN", ThreeLetterIsoCode = "MNG" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 149, Name = "Montserrat", NumericIsoCode = 500, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MS", ThreeLetterIsoCode = "MSR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 150, Name = "Morocco", NumericIsoCode = 504, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MA", ThreeLetterIsoCode = "MAR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 151, Name = "Myanmar", NumericIsoCode = 104, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MM", ThreeLetterIsoCode = "MMR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 152, Name = "Nauru", NumericIsoCode = 520, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NR", ThreeLetterIsoCode = "NRU" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 153, Name = "Nepal", NumericIsoCode = 524, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NP", ThreeLetterIsoCode = "NPL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 154, Name = "Netherlands", NumericIsoCode = 528, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NL", ThreeLetterIsoCode = "NLD" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 155, Name = "Netherlands Antilles", NumericIsoCode = 530, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AN", ThreeLetterIsoCode = "ANT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 156, Name = "New Caledonia", NumericIsoCode = 540, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NC", ThreeLetterIsoCode = "NCL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 157, Name = "New Zealand", NumericIsoCode = 554, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NZ", ThreeLetterIsoCode = "NZL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 158, Name = "Nicaragua", NumericIsoCode = 558, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NI", ThreeLetterIsoCode = "NIC" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 159, Name = "Niger", NumericIsoCode = 562, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NE", ThreeLetterIsoCode = "NER" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 160, Name = "Nigeria", NumericIsoCode = 566, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NG", ThreeLetterIsoCode = "NGA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 161, Name = "Niue", NumericIsoCode = 570, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NU", ThreeLetterIsoCode = "NIU" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 162, Name = "Norfolk Island", NumericIsoCode = 574, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NF", ThreeLetterIsoCode = "NFK" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 163, Name = "Northern Mariana Islands", NumericIsoCode = 580, Published = true, SubjectToVat = false, TwoLetterIsoCode = "MP", ThreeLetterIsoCode = "MNP" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 164, Name = "Norway", NumericIsoCode = 578, Published = true, SubjectToVat = false, TwoLetterIsoCode = "NO", ThreeLetterIsoCode = "NOR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 165, Name = "Oman", NumericIsoCode = 512, Published = true, SubjectToVat = false, TwoLetterIsoCode = "OM", ThreeLetterIsoCode = "OMN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 166, Name = "Pakistan", NumericIsoCode = 586, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PK", ThreeLetterIsoCode = "PAK" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 167, Name = "Palau", NumericIsoCode = 585, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PW", ThreeLetterIsoCode = "PLW" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 168, Name = "Palestine", NumericIsoCode = 275, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PS", ThreeLetterIsoCode = "PSE" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 169, Name = "Panama", NumericIsoCode = 591, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PA", ThreeLetterIsoCode = "PAN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 170, Name = "Papua New Guinea", NumericIsoCode = 598, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PG", ThreeLetterIsoCode = "PNG" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 171, Name = "Paraguay", NumericIsoCode = 600, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PY", ThreeLetterIsoCode = "PRY" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 172, Name = "Peru", NumericIsoCode = 604, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PE", ThreeLetterIsoCode = "PER" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 173, Name = "Philippines", NumericIsoCode = 608, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PH", ThreeLetterIsoCode = "PHL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 174, Name = "Pitcairn", NumericIsoCode = 612, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PN", ThreeLetterIsoCode = "PCN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 175, Name = "Poland", NumericIsoCode = 616, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PL", ThreeLetterIsoCode = "POL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 176, Name = "Portugal", NumericIsoCode = 620, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PT", ThreeLetterIsoCode = "PRT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 177, Name = "Puerto Rico", NumericIsoCode = 630, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PR", ThreeLetterIsoCode = "PRI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 178, Name = "Qatar", NumericIsoCode = 634, Published = true, SubjectToVat = false, TwoLetterIsoCode = "QA", ThreeLetterIsoCode = "QAT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 179, Name = "Reunion", NumericIsoCode = 638, Published = true, SubjectToVat = false, TwoLetterIsoCode = "RE", ThreeLetterIsoCode = "REU" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 180, Name = "Romania", NumericIsoCode = 642, Published = true, SubjectToVat = false, TwoLetterIsoCode = "RO", ThreeLetterIsoCode = "ROU" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 181, Name = "Russian Federation", NumericIsoCode = 643, Published = true, SubjectToVat = false, TwoLetterIsoCode = "RU", ThreeLetterIsoCode = "RUS" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 182, Name = "Rwanda", NumericIsoCode = 646, Published = true, SubjectToVat = false, TwoLetterIsoCode = "RW", ThreeLetterIsoCode = "RWA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 183, Name = "Saint Helena", NumericIsoCode = 654, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SH", ThreeLetterIsoCode = "SHN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 184, Name = "Saint Kitts And Nevis", NumericIsoCode = 659, Published = true, SubjectToVat = false, TwoLetterIsoCode = "KN", ThreeLetterIsoCode = "KNA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 185, Name = "Saint Lucia", NumericIsoCode = 662, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LC", ThreeLetterIsoCode = "LCA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 186, Name = "Saint Pierre And Miquelon", NumericIsoCode = 666, Published = true, SubjectToVat = false, TwoLetterIsoCode = "PM", ThreeLetterIsoCode = "SPM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 187, Name = "Saint Vincent And The Grenadines", NumericIsoCode = 670, Published = true, SubjectToVat = false, TwoLetterIsoCode = "VC", ThreeLetterIsoCode = "VCT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 188, Name = "Samoa", NumericIsoCode = 882, Published = true, SubjectToVat = false, TwoLetterIsoCode = "WS", ThreeLetterIsoCode = "WSM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 189, Name = "San Marino", NumericIsoCode = 674, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SM", ThreeLetterIsoCode = "SMR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 190, Name = "Sao Tome And Principe", NumericIsoCode = 678, Published = true, SubjectToVat = false, TwoLetterIsoCode = "ST", ThreeLetterIsoCode = "STP" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 191, Name = "Saudi Arabia", NumericIsoCode = 682, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SA", ThreeLetterIsoCode = "SAU" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 192, Name = "Senegal", NumericIsoCode = 686, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SN", ThreeLetterIsoCode = "SEN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 193, Name = "Serbia And Montenegro", NumericIsoCode = 891, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CS", ThreeLetterIsoCode = "SCG" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 194, Name = "Seychelles", NumericIsoCode = 690, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SC", ThreeLetterIsoCode = "SYC" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 195, Name = "Sierra Leone", NumericIsoCode = 694, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SL", ThreeLetterIsoCode = "SLE" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 196, Name = "Singapore", NumericIsoCode = 702, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SG", ThreeLetterIsoCode = "SGP" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 197, Name = "Slovakia", NumericIsoCode = 703, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SK", ThreeLetterIsoCode = "SVK" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 198, Name = "Slovenia", NumericIsoCode = 705, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SI", ThreeLetterIsoCode = "SVN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 199, Name = "Solomon Islands", NumericIsoCode = 90, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SB", ThreeLetterIsoCode = "SLB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 200, Name = "Somalia", NumericIsoCode = 706, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SO", ThreeLetterIsoCode = "SOM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 201, Name = "South Georgia And The South Sandwich Islands", NumericIsoCode = 239, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GS", ThreeLetterIsoCode = "SGS" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 202, Name = "Spain", NumericIsoCode = 724, Published = true, SubjectToVat = false, TwoLetterIsoCode = "ES", ThreeLetterIsoCode = "ESP" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 203, Name = "Sri Lanka", NumericIsoCode = 144, Published = true, SubjectToVat = false, TwoLetterIsoCode = "LK", ThreeLetterIsoCode = "LKA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 204, Name = "Sudan", NumericIsoCode = 736, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SD", ThreeLetterIsoCode = "SDN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 205, Name = "Suriname", NumericIsoCode = 740, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SR", ThreeLetterIsoCode = "SUR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 206, Name = "Svalbard And Jan Mayen Islands", NumericIsoCode = 744, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SJ", ThreeLetterIsoCode = "SJM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 207, Name = "Sweden", NumericIsoCode = 752, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SE", ThreeLetterIsoCode = "SWE" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 208, Name = "Switzerland", NumericIsoCode = 756, Published = true, SubjectToVat = false, TwoLetterIsoCode = "CH", ThreeLetterIsoCode = "CHE" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 209, Name = "Syria", NumericIsoCode = 760, Published = true, SubjectToVat = false, TwoLetterIsoCode = "SY", ThreeLetterIsoCode = "SYR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 210, Name = "Taiwan", NumericIsoCode = 158, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TW", ThreeLetterIsoCode = "TWN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 211, Name = "Tajikistan", NumericIsoCode = 762, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TJ", ThreeLetterIsoCode = "TJK" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 212, Name = "Tanzania", NumericIsoCode = 834, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TZ", ThreeLetterIsoCode = "TZA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 213, Name = "Thailand", NumericIsoCode = 764, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TH", ThreeLetterIsoCode = "THA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 214, Name = "Timor-Leste", NumericIsoCode = 626, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TL", ThreeLetterIsoCode = "TLS" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 215, Name = "Togo", NumericIsoCode = 768, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TG", ThreeLetterIsoCode = "TGO" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 216, Name = "Tokelau", NumericIsoCode = 772, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TK", ThreeLetterIsoCode = "TKL" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 217, Name = "Tonga", NumericIsoCode = 776, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TO", ThreeLetterIsoCode = "TON" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 218, Name = "Trinidad And Tobago", NumericIsoCode = 780, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TT", ThreeLetterIsoCode = "TTO" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 219, Name = "Tunisia", NumericIsoCode = 788, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TN", ThreeLetterIsoCode = "TUN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 220, Name = "Turkey", NumericIsoCode = 792, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TR", ThreeLetterIsoCode = "TUR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 221, Name = "Turkmenistan", NumericIsoCode = 795, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TM", ThreeLetterIsoCode = "TKM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 222, Name = "Turks And Caicos Islands", NumericIsoCode = 796, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TC", ThreeLetterIsoCode = "TCA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 223, Name = "Tuvalu", NumericIsoCode = 798, Published = true, SubjectToVat = false, TwoLetterIsoCode = "TV", ThreeLetterIsoCode = "TUV" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 224, Name = "Uganda", NumericIsoCode = 800, Published = true, SubjectToVat = false, TwoLetterIsoCode = "UG", ThreeLetterIsoCode = "UGA" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 225, Name = "Ukraine", NumericIsoCode = 804, Published = true, SubjectToVat = false, TwoLetterIsoCode = "UA", ThreeLetterIsoCode = "UKR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 226, Name = "United Arab Emirates", NumericIsoCode = 784, Published = true, SubjectToVat = false, TwoLetterIsoCode = "AE", ThreeLetterIsoCode = "ARE" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 227, Name = "United Kingdom", NumericIsoCode = 826, Published = true, SubjectToVat = false, TwoLetterIsoCode = "GB", ThreeLetterIsoCode = "GBR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 228, Name = "United States Minor Outlying Islands", NumericIsoCode = 581, Published = true, SubjectToVat = false, TwoLetterIsoCode = "UM", ThreeLetterIsoCode = "UMI" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 229, Name = "Uruguay", NumericIsoCode = 858, Published = true, SubjectToVat = false, TwoLetterIsoCode = "UY", ThreeLetterIsoCode = "URY" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 230, Name = "Uzbekistan", NumericIsoCode = 860, Published = true, SubjectToVat = false, TwoLetterIsoCode = "UZ", ThreeLetterIsoCode = "UZB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 231, Name = "Vanuatu", NumericIsoCode = 548, Published = true, SubjectToVat = false, TwoLetterIsoCode = "VU", ThreeLetterIsoCode = "VUT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 232, Name = "Vatican City State", NumericIsoCode = 336, Published = true, SubjectToVat = false, TwoLetterIsoCode = "VA", ThreeLetterIsoCode = "VAT" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 233, Name = "Venezuela", NumericIsoCode = 862, Published = true, SubjectToVat = false, TwoLetterIsoCode = "VE", ThreeLetterIsoCode = "VEN" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 234, Name = "Vietnam", NumericIsoCode = 704, Published = true, SubjectToVat = false, TwoLetterIsoCode = "VN", ThreeLetterIsoCode = "VNM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 235, Name = "Virgin Islands (British)", NumericIsoCode = 92, Published = true, SubjectToVat = false, TwoLetterIsoCode = "VG", ThreeLetterIsoCode = "VGB" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 236, Name = "Virgin Islands (U.S.)", NumericIsoCode = 850, Published = true, SubjectToVat = false, TwoLetterIsoCode = "VI", ThreeLetterIsoCode = "VIR" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 237, Name = "Wallis And Futuna Islands", NumericIsoCode = 876, Published = true, SubjectToVat = false, TwoLetterIsoCode = "WF", ThreeLetterIsoCode = "WLF" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 238, Name = "Western Sahara", NumericIsoCode = 732, Published = true, SubjectToVat = false, TwoLetterIsoCode = "EH", ThreeLetterIsoCode = "ESH" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 239, Name = "Yemen", NumericIsoCode = 887, Published = true, SubjectToVat = false, TwoLetterIsoCode = "YE", ThreeLetterIsoCode = "YEM" },
				new Country { AllowsBilling = true, AllowsShipping = true, DisplayOrder = 240, Name = "Zambia", NumericIsoCode = 894, Published = true, SubjectToVat = false, TwoLetterIsoCode = "ZM", ThreeLetterIsoCode = "ZMB" },
			});

			database.SaveChanges();
		}

		private static void Install()
		{
			InstallCountries();
			InstallSecurityGroups();
			InstallTopLevelCategories();
		}

		private static void TestFormSignature()
		{
			var fillForm = EngineContext.Current.Resolve<IFillFormService>();

			var initial = System.IO.File.ReadAllBytes(Environment.CurrentDirectory + "\\initial.png");
			var signature = System.IO.File.ReadAllBytes(Environment.CurrentDirectory + "\\signature.png");

			var data = fillForm.Prepare("NOTICE OF TERMINATION OR RENEWAL OF FIXED TERM LEASE.pdf")

			//initials page 1
			.WithImage(initial, 0, 90, 60, 20, 20, true, true)
			.WithImage(initial, 0, 110, 60, 20, 20, true, true)
			.WithImage(initial, 0, 130, 60, 20, 20, true, true)

			//initials page 2
			.WithImage(initial, 1, 90, 60, 20, 20, true, true)
			.WithImage(initial, 1, 110, 60, 20, 20, true, true)
			.WithImage(initial, 1, 130, 60, 20, 20, true, true)

			//landlord signatures and witnesses
			.WithImage(signature, 1, 80, 420, 40, 40)
			.WithImage(signature, 1, 450, 420, 40, 40)
			.WithImage(signature, 1, 450, 480, 40, 40)

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
