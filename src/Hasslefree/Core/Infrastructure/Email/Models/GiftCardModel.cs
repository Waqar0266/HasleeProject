using System;

namespace Hasslefree.Core.Infrastructure.Email.Models
{
    public class GiftCardModel
    {
        public Decimal Value { get; set; }
        public String FromName { get; set; }
        public String Message { get; set; }
        public String RecipientEmail { get; set; }
        public String GiftCardNumber { get; set; }
        public String StoreName { get; set; }
        public String StoreUrl { get; set; }
        public String StoreLogoUrl { get; set; }
        public String PictureUrl { get; set; }
		public string CustomRegards { get; set; }
    }
}