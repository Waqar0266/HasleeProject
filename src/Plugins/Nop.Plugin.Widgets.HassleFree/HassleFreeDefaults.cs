namespace Nop.Plugin.Widgets.HassleFree
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class HassleFreeDefaults
    {
        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string AccountMenuConfigurationRouteName => "Plugin.Widgets.HassleFree.AccountMenuConfigure";

        public static class Listings
        {
            public static string ListingsRouteName => "Plugin.Widgets.HassleFree.Listings";
            public static string Property24RouteName => "Plugin.Widgets.HassleFree.Property24";
        }

        public static class Forms
        {
            public static string FormsBondDEAConsentRouteName => "Plugin.Widgets.HassleFree.FormsBondDEAConsent";
            public static string FormsSignaturePadRouteName => "Plugin.Widgets.HassleFree.FormsSignaturePad";
            public static string FormsTestRouteName => "Plugin.Widgets.HassleFree.FormsTest";
        }

        public static class Registration
        {
            public static string Title => "Title";
            public static string Race => "Race";
            public static string Nationality => "Nationality";
            public static string PostalAddress1 => "PostalAddress1";
            public static string PostalAddress2 => "PostalAddress2";
            public static string PostalAddress3 => "PostalAddress3";
            public static string PostalAddressCode => "PostalAddressCode";
            public static string PreviousEmployer => "PreviousEmployer";
            public static string FFC => "FFC";
            public static string FFCNumber => "FFCNumber";
            public static string FFCIssueDate => "FFCIssueDate";
            public static string EAABReference => "EAABReference";
            public static string Dismissed => "Dismissed";
            public static string Convicted => "Convicted";
            public static string Insolvent => "Insolvent";
            public static string Withdrawn => "Withdrawn";
            public static string FormComplete => "FormComplete";
        }

        public static string RegistrationRouteName => "Plugin.Widgets.HassleFree.Registration";

        public static class Agents
        {
            public static string ListAgentRouteName => "Plugin.Widgets.HassleFree.Agents.List";
            public static string AddAgentRouteName => "Plugin.Widgets.HassleFree.Agents.Add";
            public static string AgentDetailRouteName => "Plugin.Widgets.HassleFree.Agents.Detail";
            public static string CompleteAgentRegistrationRouteName => "Plugin.Widgets.HassleFree.Agents.CompleteAgentRegistration";
        }
    }
}
