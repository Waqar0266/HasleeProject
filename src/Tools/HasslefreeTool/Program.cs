using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.Security.Groups;
using System.Linq;

namespace HasslefreeTool
{
	class Program
	{
		static void Main(string[] args)
		{
			Init();

			InstallSecurityGroups();
		}

		private static void Init()
		{
			//Start the Hasslefree application engine
			EngineContext.Initialize(false);
		}

		private static void InstallSecurityGroups()
		{
			var createSecurityGroupService = EngineContext.Current.Resolve<ICreateSecurityGroupService>();
			var createPersonService = EngineContext.Current.Resolve<ICreatePersonService>();
			var loginRepo = EngineContext.Current.Resolve<IDataRepository<Login>>();

			if (!loginRepo.Table.Any(l => l.Email == "admin@hasslefree.za.com"))
			{

				createPersonService.New("Admin", "Admin", "Admin", "admin@hasslefree.za.com").WithPassword("password", "").Create();
				createSecurityGroupService.New("Admin", "Admin").WithUser(createPersonService.LoginId).Create();
			}

			if (!loginRepo.Table.Any(l => l.Email == "director@hasslefree.za.com"))
			{
				createPersonService.New("Director", "Director", "Director", "director@hasslefree.za.com").WithPassword("password", "").Create();
				createSecurityGroupService.New("Director", "Director").WithUser(createPersonService.LoginId).Create();
			}

			//create the agent role
			createSecurityGroupService.New("Agent", "Agent").Create();
		}
	}
}
