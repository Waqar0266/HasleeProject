using Autofac;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Accounts.Password;
using Hasslefree.Services.Common;
using Hasslefree.Services.Common.Addresses;
using Hasslefree.Services.Common.Countries;
using Hasslefree.Services.Profiles;
using System.Reflection;

namespace Hasslefree.Services
{
	[CoreRegistrar]
	public class DependencyConfig : IDependencyRegistrar
	{
		public void RegisterDependencies(ContainerBuilder builder)
		{
			#region Generic

			var assembly = Assembly.GetExecutingAssembly();

			builder.RegisterAssemblyTypes(assembly)
				.AssignableTo<IInstancePerRequest>()
				.AsImplementedInterfaces()
				.InstancePerRequest();

			#endregion

			#region Account

			// Account
			builder.RegisterType<UpdateProfileService>().As<IUpdateProfileService>().InstancePerRequest();
			builder.RegisterType<UpdateLoginProfileService>().As<IUpdateLoginProfileService>().InstancePerRequest();

			// Account Action
			builder.RegisterType<LoginService>().As<ILoginService>().InstancePerRequest();
			builder.RegisterType<LogoutService>().As<ILogoutService>().InstancePerRequest();

			// Account Password
			builder.RegisterType<ChangeProfilePasswordService>().As<IChangeProfilePasswordService>().InstancePerRequest();
			builder.RegisterType<ForgotPasswordService>().As<IForgotPasswordService>().InstancePerRequest();
			builder.RegisterType<ResetPasswordService>().As<IResetPasswordService>().InstancePerRequest();

			#endregion

			#region Common

			#region Addresses

			builder.RegisterType<CreateAddressService>().As<ICreateAddressService>().InstancePerRequest();
			builder.RegisterType<DeleteAddressService>().As<IDeleteAddressService>().InstancePerRequest();

			#endregion

			#region Countries

			builder.RegisterType<GetCountryService>().As<IGetCountryService>().InstancePerRequest();

			#endregion

			builder.RegisterType<CountryQueryService>().As<ICountryQueryService>().InstancePerRequest();

			#endregion
		}
	}
}
