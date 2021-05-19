using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Widgets.HassleFree.Models.Agents
{
    public class CompleteAgentSignupViewModel
    {
        public string AgentUniqueId { get; set; }
        public List<SelectListItem> AvailableRaces { get; set; }
        public List<SelectListItem> AvailableTitles { get; set; }
        public int AgentId { get; set; }
        public string Title { get; set; }
        public string Race { get; set; }
        public string Nationality { get; set; }
        [DisplayName("Postal Address 1")]
        public string PostalAddress1 { get; set; }
        [DisplayName("Postal Address 2")]
        public string PostalAddress2 { get; set; }
        [DisplayName("Postal Address 3")]
        public string PostalAddress3 { get; set; }
        [DisplayName("Postal Address Postal Code")]
        public string PostalAddressCode { get; set; }
        [DisplayName("Previous Employer Name")]
        public string PreviousEmployer { get; set; }
        [DisplayName("Were you issued with a Fidelity Fund Certificate as an Estate Agent under this firm?")]
        public bool FFC { get; set; }
        [DisplayName("FFC Number")]
        public string FFCNumber { get; set; }
        [DisplayName("FFC Issue Date")]
        public int FFCIssueDateDay { get; set; }
        public int FFCIssueDateMonth { get; set; }
        public int FFCIssueDateYear { get; set; }
        public DateTime? ParseFFCIssueDate()
        {
            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(FFCIssueDateYear, FFCIssueDateMonth, FFCIssueDateDay);
            }
            catch { }
            return dateOfBirth;
        }
        [DisplayName("Please indicate your seven digits reference number with EAAB")]
        public string Reference { get; set; }
        [DisplayName("Have you been dismissed from a position of trust due to improper conduct?")]
        public bool Dismissed { get; set; }
        [DisplayName("Have you been convicted of an offence involving an element of dishonesty?")]
        public bool Convicted { get; set; }
        [DisplayName("Were/are you insolvent and not yet rehabilitated? (attach insolvency and rehabilitation documentations)")]
        public bool Insolvent { get; set; }
        [DisplayName("Was your Fidelity Fund Certificate ever been withdrawn?")]
        public bool Withdrawn { get; set; }
        public string Signature { get; set; }
    }
}
