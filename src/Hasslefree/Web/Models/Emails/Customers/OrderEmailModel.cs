using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Emails.Customers
{
	public class OrderEmailModel
	{
		public String RefNumber { get; set; }
		public String SeqNumber { get; set; }
		public String Date { get; set; }

		public String ByName { get; set; }
		public String ByEmail { get; set; }
		public String ByPhone { get; set; }

		public String ToName { get; set; }
		public String ToEmail { get; set; }
		public String ToPhone { get; set; }

		public String SpecialInstructions { get; set; }
		public String TaxNumber { get; set; }

		public String CollectionName { get; set; }
		public String CollectionDueDate { get; set; }
		public String CollectionNotes { get; set; }
		public String CollectionPictureUrl { get; set; }

		public String ContactName { get; set; }
		public String ContactPhone { get; set; }
		public String ContactEmail { get; set; }

		public String ShippingMethod { get; set; }
		public String ShippingAddress1 { get; set; }
		public String ShippingAddress2 { get; set; }
		public String ShippingAddress3 { get; set; }
		public String ShippingTown { get; set; }
		public String ShippingRegion { get; set; }
		public String ShippingCountry { get; set; }
		public String ShippingPostalCode { get; set; }
		public String ShippingPhone { get; set; }
		public String ShippingLatitude { get; set; }
		public String ShippingLongitude { get; set; }
		public String ShippingFull
		{
			get
			{
				var fullAddress = "";
				if (!String.IsNullOrEmpty(ShippingAddress1)) fullAddress += ShippingAddress1 + ", ";
				if (!String.IsNullOrEmpty(ShippingAddress2)) fullAddress += ShippingAddress2 + ", ";
				if (!String.IsNullOrEmpty(ShippingAddress3)) fullAddress += ShippingAddress3 + ", ";
				if (!String.IsNullOrEmpty(ShippingTown)) fullAddress += ShippingTown + ", ";
				if (!String.IsNullOrEmpty(ShippingRegion)) fullAddress += ShippingRegion + ", ";
				if (!String.IsNullOrEmpty(ShippingCountry)) fullAddress += ShippingCountry + ", ";
				if (!String.IsNullOrEmpty(ShippingPostalCode)) fullAddress += ShippingPostalCode + ", ";
				if (!String.IsNullOrEmpty(ShippingPhone)) fullAddress += ShippingPhone;

				return String.IsNullOrWhiteSpace(fullAddress) ? null : fullAddress.Trim();
			}
		}

		public List<OrderItemEmailModel> Items { get; set; }

		public String SubTotalAmount { get; set; }
		public String DiscountAmount { get; set; }
		public String ShippingAmount { get; set; }
		public String TaxAmount { get; set; }
		public String TotalAmountDue { get; set; }
		public String PaidAmount { get; set; }
		public String OutstandingAmount { get; set; }

		public List<OrderBankEmailModel> Banks { get; set; }
	}

	public class OrderItemEmailModel
	{
		public String Sku { get; set; }
		public String Description { get; set; }
		public String Price { get; set; }
		public Decimal Qty { get; set; }
		public String Tax { get; set; }
		public String Discount { get; set; }
		public String Total { get; set; }
		public String SpecialInstructions { get; set; }

		public List<OrderItemEmailModel> SubItems { get; set; }
	}

	public class OrderBankEmailModel
	{
		public String Name { get; set; }
		public String BankName { get; set; }
		public String AccountNumber { get; set; }
		public String AccountType { get; set; }
		public String BranchCode { get; set; }
		public String SwiftCode { get; set; }
	}
}
