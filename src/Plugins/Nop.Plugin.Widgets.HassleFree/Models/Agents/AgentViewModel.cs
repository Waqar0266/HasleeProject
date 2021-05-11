using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.HassleFree.Domain;

namespace Nop.Plugin.Widgets.HassleFree.Models.Agents
{
    public class AgentViewModel
    {
        public AgentViewModel()
        {
            this.AvailableProvinces = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string IdNumber { get; set; }
        public string Mobile { get; set; }
        public string Province { get; set; }
        public AgentType AgentType { get; set; }
        public List<SelectListItem> AvailableProvinces { get; set; }
        public bool CanUpdate { get; set; }
    }
}
