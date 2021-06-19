using System;
using Hasslefree.Web.Models.Emails.Customers;
using System.Collections.Generic;
using Hasslefree.Web.Models.Emails;

namespace Hasslefree.Services.Emails.Definitions.Customers
{
	public class Enquiry : IEmailDefinition
	{
		public string Title => "Enquiry";

		public Object Example =>
			new
			{
				Email = new SharedEmailModel
				{
					Title = "Email Title",
					Subject = "Subject",
					FromName = "The Store",
					ReplyTo = "email@domain.com"
				},
				Store = new SharedStoreModel
				{
					Name = "Store name",
					Phone = "+0 000 0000 000",
					Email = "email@domain.com",
					Url = "https://store.Hasslefree7.co.za/",
					LogoUrl = null,
					CurrencyCode = "ZAR",
					CurrencySymbol = "R"
				},
				Document = new OrderEmailModel()
				{
					RefNumber = "ABCD1234",
					SeqNumber = "ORD100001",
					Date = DateTime.UtcNow.ToShortDateString(),

					ByName = "Name Surname",
					ByEmail = "email@domain.com",
					ByPhone = null,

					ToName = null,
					ToEmail = null,
					ToPhone = null,

					SpecialInstructions = "Special instructions are entered here",
					TaxNumber = null,

					ShippingMethod = "Economy",
					ShippingAddress1 = "1 Street",
					ShippingAddress2 = "Building",
					ShippingAddress3 = "Suburb",
					ShippingTown = "Town",
					ShippingRegion = "Western Cape",
					ShippingCountry = "South Africa",
					ShippingPostalCode = "00000",
					ShippingPhone = null,

					Items = new List<OrderItemEmailModel>()
					{
						new OrderItemEmailModel()
						{
							Sku = "PRODUCT-1",
							Description = "Product One",
							Price = "R90.00",
							Qty = 50.0M,
							Discount = null,
							Tax = "R675.00",
							Total = "R4500.00"
						},

						new OrderItemEmailModel()
						{
							Sku = "BATCH-1",
							Description = "Batch One",
							Price = "R500.00",
							Qty = 1,
							Discount = null,
							Tax = "R75.00",
							Total = "R500.00",

							SubItems = new List<OrderItemEmailModel>()
							{
								new OrderItemEmailModel()
								{
									Sku = null,
									Description = "Custom Costing",
									Price = "R200.00",
									Qty = 1,
									Discount = null,
									Tax = "R30.00",
									Total = "R200.00"
								},
								new OrderItemEmailModel()
								{
									Sku = "BATCH-SUB-1",
									Description = "Batch Sub Item 1",
									Price = "R5.00",
									Qty = 10,
									Discount = null,
									Tax = "R7.50",
									Total = "R50.00"
								},
								new OrderItemEmailModel()
								{
									Sku = "BATCH-SUB-2",
									Description = "Batch Sub Item 2",
									Price = "R5.00",
									Qty = 10,
									Discount = null,
									Tax = "R7.50",
									Total = "R50.00"
								},
								new OrderItemEmailModel()
								{
									Sku = "BATCH-SUB-3",
									Description = "Batch Sub Item 3",
									Price = "R10.00",
									Qty = 20,
									Discount = null,
									Tax = "R30.00",
									Total = "R200.00"
								}
							}
						}
					},

					SubTotalAmount = "R5000.00",
					DiscountAmount = "R250.00",
					TaxAmount = "R712.50",
					ShippingAmount = null,
					TotalAmountDue = "R5712.50",
					PaidAmount = null,
					OutstandingAmount = null
				}
			};

		public InstallDefinition Install => new InstallDefinition()
		{
			Send = false,
			From = "",
			Subject = "Enquiry - {0}",
			Recipient = "",
			Url = "/order/enquiry-email/?documentId={0}"
		};
	}
}
