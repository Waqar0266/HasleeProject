using System;
using System.Collections.Generic;
using Hasslefree.Web.Models.Emails;
using Hasslefree.Web.Models.Emails.Customers;

namespace Hasslefree.Services.Emails.Definitions.Customers
{
	public class CollectionInfo : IEmailDefinition
	{
		public string Title => "Collection Info";

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

					ToName = "Name Surname",
					ToEmail = "email@domain.com",
					ToPhone = null,

					SpecialInstructions = "Special instructions are entered here",
					TaxNumber = null,

					CollectionDueDate = DateTime.UtcNow.ToShortDateString(),
					CollectionNotes = "Collection notes are entered here",
					CollectionPictureUrl = null,

					ContactName = "Contact Name",
					ContactEmail = "contact@email.com",
					ContactPhone = null,

					ShippingMethod = "Collect",
					ShippingAddress1 = "1 Street",
					ShippingAddress2 = "Building",
					ShippingAddress3 = "Suburb",
					ShippingTown = "Town",
					ShippingRegion = "Western Cape",
					ShippingCountry = "South Africa",
					ShippingPostalCode = "00000",
					ShippingPhone = null,
					ShippingLatitude = null,
					ShippingLongitude = null,

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
					ShippingAmount = "R150.00",
					TotalAmountDue = "R5862.50",
					PaidAmount = "R3000.00",
					OutstandingAmount = "R2862.50"
				}
			};

		public InstallDefinition Install => new InstallDefinition()
		{
			Send = true,
			From = "",
			Subject = "Order collection info - {0}",
			Recipient = "",
			Url = "/order/collection-info-email/?documentId={0}"
		};
	}
}
