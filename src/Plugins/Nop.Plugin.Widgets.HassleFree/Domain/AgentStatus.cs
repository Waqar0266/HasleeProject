namespace Nop.Plugin.Widgets.HassleFree.Domain
{
    public enum AgentStatus
    {
        /// <summary>
        /// Active
        /// </summary>
        Active = 5,

        /// <summary>
        /// Deactive
        /// </summary>
        Deactive = 10,

        /// <summary>
        /// Pending Registration (still need to fill in form)
        /// </summary>
        PendingRegistration = 15,

        /// <summary>
        /// Pending Approval
        /// </summary>
        PendingDocumentation = 20,

        /// <summary>
        /// Pending Approval
        /// </summary>
        PendingApprovaln = 25
    }
}
