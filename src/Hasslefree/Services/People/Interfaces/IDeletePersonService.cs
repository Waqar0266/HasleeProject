using System.Collections.Generic;
using Hasslefree.Services.People.Warnings;

namespace Hasslefree.Services.People.Interfaces
{
	public interface IDeletePersonService
	{
		bool HasWarnings { get; }
		List<PersonWarning> Warnings { get; }

		IDeletePersonService this [int personId] { get; }
		IDeletePersonService this [List<int> personIds] { get; }

		bool Remove(bool saveChanges = true);
	}
}
