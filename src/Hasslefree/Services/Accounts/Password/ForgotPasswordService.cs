using Hasslefree.Core;
using Hasslefree.Core.Crypto;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Data;
using Hasslefree.Services.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;

namespace Hasslefree.Services.Accounts.Password
{
	public class ForgotPasswordService : IForgotPasswordService
	{
		/* Dependencies */
        private IDataRepository<Person> PersonRepo { get; }
        private ISendMail SendMail { get; }

        /* CTOR */
        public ForgotPasswordService
        (
            IDataRepository<Person> personRepo,
            ISendMail sendMail
        )
        {
            PersonRepo = personRepo;
            SendMail = sendMail;
        }

        /* Properties */
        public bool HasWarnings => Warnings.Any();
        public List<ForgotPasswordWarning> Warnings { get; } = new List<ForgotPasswordWarning>();

        public bool ForgotPassword(string email, out int otp, out string hash)
        {
            // Initialize
            Warnings.Clear();
            hash = Empty;
            otp = 0;

            // Trim the input
            email = email?.Trim();
            if (IsNullOrWhiteSpace(email)) throw new ArgumentNullException();

            // Fetch the person
            var person = PersonRepo.Table.FirstOrDefault(p => p.Email == email);
            if (person == null)
            {
                Warnings.Add(new ForgotPasswordWarning(ForgotPasswordWarningCode.NotFound));
                return false;
            }

            // Generate an OTP number between 1 and 9999
            otp = new Random().Next(100000, 999999);

            // Generate a hash based on GUID of the person
            hash = Hash.GetHashBase64(person.PersonGuid.ToString(), otp.ToString());

            // Send an email to the user containing the OTP
            if (SendEmail(email, otp)) return true;

            // The email failed
            Warnings.Add(new ForgotPasswordWarning(ForgotPasswordWarningCode.EmailFailed));
            return false;
        }

        /// <summary>
        /// Send an email to the user containing the OTP
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otp"></param>
        private bool SendEmail(string email, int otp)
        {
            // Get the email settings
            var url = "/account/forgot-email?otp=" + otp;
            var subject = "Hasslefree - Forgot Password";

            // Send the email
            var emailSender = SendMail
                    .WithRecipient(email)
                    .WithUrlBody(url);

            return emailSender.Send(subject);
        }
	}
}
