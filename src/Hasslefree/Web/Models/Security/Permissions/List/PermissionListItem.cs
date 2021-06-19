using Newtonsoft.Json;
using Hasslefree.Web.Models.Common;
using System;

namespace Hasslefree.Web.Models.Security.Permissions.List
{
	/// <summary>
	/// Login listing item
	/// </summary>
	public class PermissionListItem
	{
		/// <summary>
		/// Unique row identifier
		/// </summary>
		public int PermissionId { get; set; }

		/// <summary>
		/// UTC DateTime of when the record was created
		/// </summary>
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// UTC DateTime of when last the record as modified
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

		/// <summary>
		/// Display name
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// System name
		/// </summary>
		public string SystemName { get; set; }

		/// <summary>
		/// Group
		/// </summary>
		public string Group { get; set; }

		/// <summary></summary>
		[JsonIgnore]
		public CrudAction Action { get; set; }
	}
}
