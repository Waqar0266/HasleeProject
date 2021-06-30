using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Hasslefree.Core;
using Hasslefree.Core.AppConfig;
using Hasslefree.Core.Data;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Configuration;
using Hasslefree.Services.Downloads;
using Hasslefree.Services.Infrastructure.Email;
using Hasslefree.Services.Infrastructure.Storage;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Services.People.Implementations;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.Pictures;
using Hasslefree.Services.Security;
using Hasslefree.Services.Security.AuthorizedUsers.Crud;
using Hasslefree.Services.Security.AuthorizedUsers.Search;
using Hasslefree.Services.Security.Groups;
using Hasslefree.Services.Security.Login;
using Hasslefree.Services.Security.Permissions;
using Hasslefree.Services.Security.Sessions;
using Hasslefree.Web.Framework;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business
{
    [CoreRegistrar]
    public class DependencyConfig : IDependencyRegistrar
    {
        public void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterModule<AutofacWebTypesModule>();

            // Change controller action parameter injection by changing web.config.
            builder.RegisterType<ExtensibleActionInvoker>().As<IActionInvoker>().InstancePerRequest();

            // Register the database context
            builder.RegisterType<HasslefreeDatabaseContextMySql>().As<IDataContext>().InstancePerRequest();

            // Register generics
            builder.RegisterGeneric(typeof(DataRepository<>)).As(typeof(IDataRepository<>)).InstancePerRequest();

            // Add read-only items
            builder.Register(a => new HasslefreeDatabaseContextMySql("ReadOnlyContext")).As<IReadOnlyContext>().InstancePerRequest();
            builder.RegisterGeneric(typeof(ReadOnlyRepository<>)).As(typeof(IReadOnlyRepository<>)).InstancePerRequest();

            // Register the application configuration
            builder.Register(c => HasslefreeConfiguration.GetConfig()).As<HasslefreeConfiguration>().InstancePerRequest();

            // Register the Model Extender
            builder.RegisterType<ModelExtender>().As<IModelExtender>().InstancePerRequest();

            // HTTP Context & Related Objects
            builder.Register(c => new HttpContextWrapper(HttpContext.Current) as HttpContextBase).As<HttpContextBase>().InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>().InstancePerLifetimeScope();

            // Web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerRequest();

            //Register the almost as important UserContext
            builder.RegisterType<UserContext>().As<IUserContext>().InstancePerRequest();

            // Register the Security Manager
            builder.RegisterType<SecurityManager>().InstancePerRequest();

            // Register the Session Manager
            builder.RegisterType<SessionManager>().As<ISessionManager>().InstancePerRequest();

            //Register the Cache
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerRequest();

            #region People

            // CRUD
            builder.RegisterType<CreatePersonService>().As<ICreatePersonService>().InstancePerRequest();
            builder.RegisterType<DeletePersonService>().As<IDeletePersonService>().InstancePerRequest();
            builder.RegisterType<GetPersonService>().As<IGetPersonService>().InstancePerRequest();
            builder.RegisterType<ListPersonsService>().As<IListPersonsService>().InstancePerRequest();
            builder.RegisterType<UpdatePersonService>().As<IUpdatePersonService>().InstancePerRequest();

            #endregion People

            #region Security

            builder.RegisterType<SecurityService>().As<ISecurityService>().InstancePerRequest();

            // Authorized User
            builder.RegisterType<CreateAuthorizedUserService>().As<ICreateAuthorizedUserService>().InstancePerRequest();
            builder.RegisterType<DeleteAuthorizedUserService>().As<IDeleteAuthorizedUserService>().InstancePerRequest();
            builder.RegisterType<GetAuthorizedUsersService>().As<IGetAuthorizedUsersService>().InstancePerRequest();
            builder.RegisterType<ListAuthorizedUsersService>().As<IListAuthorizedUsersService>().InstancePerRequest();
            builder.RegisterType<UpdateAuthorizedUserService>().As<IUpdateAuthorizedUserService>().InstancePerRequest();

            // Authorized User Search
            builder.RegisterType<SearchUsersService>().As<ISearchUsersService>().InstancePerRequest();

            // Login
            builder.RegisterType<CreateLoginService>().As<ICreateLoginService>().InstancePerRequest();
            builder.RegisterType<DeleteLoginService>().As<IDeleteLoginService>().InstancePerRequest();
            builder.RegisterType<GetLoginService>().As<IGetLoginService>().InstancePerRequest();
            builder.RegisterType<ListLoginsService>().As<IListLoginsService>().InstancePerRequest();
            builder.RegisterType<UpdateLoginService>().As<IUpdateLoginService>().InstancePerRequest();

            // Permission
            builder.RegisterType<ListPermissionsService>().As<IListPermissionsService>().InstancePerRequest();

            // Security Group
            builder.RegisterType<CreateSecurityGroupService>().As<ICreateSecurityGroupService>().InstancePerRequest();
            builder.RegisterType<DeleteSecurityGroupService>().As<IDeleteSecurityGroupService>().InstancePerRequest();
            builder.RegisterType<GetSecurityGroupService>().As<IGetSecurityGroupService>().InstancePerRequest();
            builder.RegisterType<ListSecurityGroupsService>().As<IListSecurityGroupsService>().InstancePerRequest();
            builder.RegisterType<UpdateSecurityGroupService>().As<IUpdateSecurityGroupService>().InstancePerRequest();

            // Session
            builder.RegisterType<GetSessionService>().As<IGetSessionService>().InstancePerRequest();
            builder.RegisterType<ListSessionsService>().As<IListSessionsService>().InstancePerRequest();

            #endregion Security

            builder.RegisterType<PictureService>().As<IPictureService>().InstancePerRequest();
            builder.RegisterType<RemovePictureService>().As<IRemovePictureService>().InstancePerRequest();
            builder.RegisterType<SettingsService>().As<ISettingsService>().InstancePerRequest();
            builder.RegisterType<DownloadService>().As<IDownloadService>().InstancePerRequest();

            // Import
            builder.RegisterType<UploadPictureService>().As<IUploadPictureService>().InstancePerRequest();
            builder.RegisterType<UploadDownloadService>().As<IUploadDownloadService>().InstancePerRequest();
            builder.RegisterType<DefaultTransformService>().As<IDefaultTransformService>().InstancePerRequest();

            /* EMAIL */
			builder.RegisterType<SMTP>().As<IEmailService>().InstancePerRequest();

            builder.Register<ICloudStorageService>(c => new S3StorageService(
            ConfigurationManager.AppSettings["AccessKey"],
            ConfigurationManager.AppSettings["SecretKey"])).InstancePerRequest();

            /************************* Asp.Net MVC ****************************/
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToArray();

            builder.RegisterControllers(assemblies);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterFilterProvider();
        }
    }
}
